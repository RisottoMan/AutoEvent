namespace AutoEvent.API.Schematic.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using Enums;
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration.Distributors;
    using MEC;
    using Mirror;
    using PluginAPI.Core;
    using PluginAPI.Core.Items;
    using Serializable;
    using UnityEngine;
    using Utf8Json;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class SchematicObject : MapEditorObject
    {
        public SchematicObject Init(SchematicSerializable schematicSerializable, SchematicObjectDataList data)
        {
            Log.Info($"Initializing schematic \"{schematicSerializable.SchematicName}\"");

            Base = schematicSerializable;
            SchematicData = data;
            DirectoryPath = data.Path;

            ObjectFromId = new Dictionary<int, Transform>(SchematicData.Blocks.Count + 1)
            {
                { data.RootObjectId, transform },
            };

            CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);
            AddRigidbodies();

            float delay = 0f;
            if (delay != -1f)
            {
                Timing.RunCoroutine(SpawnDelayed());
            }
            else
            {
                foreach (NetworkIdentity networkIdentity in NetworkIdentities)
                    NetworkServer.Spawn(networkIdentity.gameObject);

                AddAnimators();
                IsBuilt = true;
            }

            UpdateObject();

            return this;
        }

        public SchematicSerializable Base;

        public SchematicObjectDataList SchematicData { get; private set; }

        public string DirectoryPath { get; private set; }

        public ObservableCollection<GameObject> AttachedBlocks { get; private set; } = new();

        public Vector3 OriginalPosition { get; private set; }

        public Vector3 OriginalRotation { get; private set; }

        public string Name => Base.SchematicName;

        public bool IsRootSchematic => transform.root == transform;

        public AnimationController AnimationController => AnimationController.Get(this);

        public ReadOnlyCollection<NetworkIdentity> NetworkIdentities
        {
            get
            {
                if (_networkIdentities == null)
                {
                    List<NetworkIdentity> list = new();

                    foreach (GameObject block in AttachedBlocks)
                    {
                        if (block.TryGetComponent(out NetworkIdentity networkIdentity))
                        {
                            list.Add(networkIdentity);
                        }
                    }

                    _networkIdentities = list.AsReadOnly();
                }

                return _networkIdentities;
            }
        }

        public override void UpdateObject()
        {
            if (IsRootSchematic && Base.SchematicName != name.Split('-')[1])
            {
                SchematicObject newObject = ObjectSpawner.SpawnSchematic(Base, transform.position, transform.rotation, transform.localScale);

                if (newObject != null)
                {
                    Destroy();
                    return;
                }

                Base.SchematicName = name.Replace("CustomSchematic-", string.Empty);
            }

            OriginalPosition = RelativePosition;
            OriginalRotation = RelativeRotation;

            if (!IsRootSchematic)
                return;
        }

        private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
        {
            Transform childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; 
            int[] parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

            foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
            {
                if (parentSchematics.Contains(block.ParentId))
                    continue;

                CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform);
            }
        }

        private Transform CreateObject(SchematicBlockData block, Transform parentTransform)
        {
            if (block == null)
                return null;

            GameObject gameObject = null;
            RuntimeAnimatorController animatorController;

            switch (block.BlockType)
            {
                case BlockType.Empty:
                {
                    gameObject = new GameObject(block.Name)
                    {
                        transform =
                        {
                            parent = parentTransform,
                            localPosition = block.Position,
                            localEulerAngles = block.Rotation,
                        },
                    };

                    break;
                }

                case BlockType.Primitive:
                {
                    if (Instantiate(ObjectHelper.PrimitiveBaseObject, parentTransform).TryGetComponent(out PrimitiveObjectToy primitiveToy))
                    {
                        gameObject = primitiveToy.gameObject.AddComponent<PrimitiveObject>().Init(block).gameObject;
                    }

                    break;
                }

                case BlockType.Light:
                {
                    if (Instantiate(ObjectHelper.LightBaseObject, parentTransform).TryGetComponent(out LightSourceToy lightSourceToy))
                    {
                        gameObject = lightSourceToy.gameObject.AddComponent<LightSourceObject>().Init(block).gameObject;

                        if (TryGetAnimatorController(block.AnimatorName, out animatorController))
                            _animators.Add(lightSourceToy._light.gameObject, animatorController);
                    }

                    break;
                }

                case BlockType.Pickup:
                {
                    ItemPickup pickup = null;

                    if (block.Properties.TryGetValue("Chance", out object property) && Random.Range(0, 101) > float.Parse(property.ToString()))
                    {
                        gameObject = new("Empty Pickup")
                        {
                            transform = { parent = parentTransform, localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                        };
                        break;
                    }

                    if (block.Properties.TryGetValue("CustomItem", out property) && !string.IsNullOrEmpty(property.ToString()))
                    {
                        string customItemName = property.ToString();
                    }
                    else
                    {
                        var itemType = ((ItemType)Enum.Parse(typeof(ItemType), block.Properties["ItemType"].ToString()));
                        pickup = ItemPickup.Create(itemType, new Vector3(0, 0, 0), Quaternion.identity);
                    }

                    gameObject = pickup.GameObject;
                    gameObject.name = block.Name;

                    gameObject.transform.parent = parentTransform;
                    gameObject.transform.localPosition = block.Position;
                    gameObject.transform.localEulerAngles = block.Rotation;
                    gameObject.transform.localScale = block.Scale;

                    break;
                }

                case BlockType.Workstation:
                {
                    if (Instantiate(ObjectHelper.WorkstationObject, parentTransform).TryGetComponent(out WorkstationController workstation))
                    {
                        gameObject = workstation.gameObject.AddComponent<WorkstationObject>().Init(block).gameObject;

                        gameObject.transform.parent = null;
                        NetworkServer.Spawn(gameObject);

                        _transformProperties.Add(gameObject.transform.GetInstanceID(), block.ObjectId);
                    }

                    break;
                }
                
                case BlockType.Locker:
                {
                    if (block.Properties.TryGetValue("Chance", out object property) && Random.Range(0, 101) > float.Parse(property.ToString()))
                    {
                        gameObject = new("Empty Locker")
                        {
                            transform = { localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                        };
                    }
                    else
                    {
                        Locker locker = null; 
                        LockerType lockerType = (LockerType)Enum.Parse(typeof(LockerType), block.Properties["LockerType"].ToString());

                        switch(lockerType)
                        {
                            case LockerType.Pedestal: { locker = ObjectHelper.ScpPedestalObject; break; }
                            case LockerType.LargeGun: { locker = ObjectHelper.LargeGunLockerObject; break; }
                            case LockerType.RifleRack: { locker = ObjectHelper.StandartLockerObject; break; }
                            case LockerType.Misc: { locker = ObjectHelper.StandartLockerObject; break; }
                            case LockerType.Medkit: { locker = ObjectHelper.SmallWallCabinetObject; break; }
                            case LockerType.Adrenaline: { locker = ObjectHelper.SmallWallCabinetObject; break; }
                        }

                        gameObject = Instantiate(locker.gameObject, parentTransform).AddComponent<LockerObject>().Init(block).gameObject;
                    }
                    break;
                }
                
                case BlockType.Schematic:
                {
                    string schematicName = block.Properties["SchematicName"].ToString();

                    gameObject = ObjectSpawner.SpawnSchematic(schematicName, transform.position + block.Position, Quaternion.Euler(transform.eulerAngles + block.Rotation)).gameObject;
                    gameObject.transform.parent = parentTransform;

                    gameObject.name = schematicName;

                    break;
                }
            }

            AttachedBlocks.Add(gameObject);
            ObjectFromId.Add(block.ObjectId, gameObject.transform);

            if (block.BlockType != BlockType.Light && TryGetAnimatorController(block.AnimatorName, out animatorController))
                _animators.Add(gameObject, animatorController);

            return gameObject.transform;
        }

        private bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
        {
            animatorController = null;

            if (string.IsNullOrEmpty(animatorName))
                return false;

            Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

            if (animatorObject is null)
            {
                string path = Path.Combine(DirectoryPath, animatorName);

                if (!File.Exists(path))
                {
                    Log.Warning($"{gameObject.name} block of {name} should have a {animatorName} animator attached, but the file does not exist!");
                    return false;
                }

                animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
            }

            animatorController = (RuntimeAnimatorController)animatorObject;
            return true;
        }

        private void AddAnimators()
        {
            foreach (KeyValuePair<GameObject, RuntimeAnimatorController> pair in _animators)
                pair.Key.AddComponent<Animator>().runtimeAnimatorController = pair.Value;

            _animators = null;
            AssetBundle.UnloadAllAssetBundles(false);
        }

        private IEnumerator<float> SpawnDelayed()
        {
            foreach (NetworkIdentity networkIdentity in NetworkIdentities)
            {
                NetworkServer.Spawn(networkIdentity.gameObject);
                yield return Timing.WaitForSeconds(0f);
            }

            AddAnimators();
            IsBuilt = true;

            if (Base.CullingType != CullingType.Distance)
                yield break;

            foreach (NetworkIdentity networkIdentity in NetworkIdentities)
            {
                foreach (Player player in Player.GetPlayers())
                {
                    player.Connection.Send(new ObjectDestroyMessage
                    {
                        netId = networkIdentity.netId
                    });
                }
            }
        }

        private void AddRigidbodies()
        {
            string rigidbodyPath = Path.Combine(DirectoryPath, $"{Name}-Rigidbodies.json");
            if (!File.Exists(rigidbodyPath))
                return;

            foreach (KeyValuePair<int, SerializableRigidbody> dict in JsonSerializer.Deserialize<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
            {
                if (!ObjectFromId[dict.Key].gameObject.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody = ObjectFromId[dict.Key].gameObject.AddComponent<Rigidbody>();

                rigidbody.isKinematic = dict.Value.IsKinematic;
                rigidbody.useGravity = dict.Value.UseGravity;
                rigidbody.constraints = dict.Value.Constraints;
                rigidbody.mass = dict.Value.Mass;
            }
        }

        private void OnDestroy()
        {
            AnimationController.Dictionary.Remove(this);

            foreach (GameObject gameObject in AttachedBlocks)
            {
                if (_transformProperties.ContainsKey(gameObject.transform.GetInstanceID()))
                    NetworkServer.Destroy(gameObject);
            }
        }

        internal bool IsBuilt;
        internal Dictionary<int, Transform> ObjectFromId = new();

        private ReadOnlyCollection<NetworkIdentity> _networkIdentities;
        private Dictionary<int, int> _transformProperties = new();
        private Dictionary<GameObject, RuntimeAnimatorController> _animators = new();
    }
}