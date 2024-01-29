namespace MER.Lite.Objects
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
    using MER.Lite.Components;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class SchematicObject : MapEditorObject
    {
        public SchematicObject Init(SchematicSerializable schematicSerializable, SchematicObjectDataList data, bool isStatic)
        {
            Base = schematicSerializable;
            SchematicData = data;
            DirectoryPath = data.Path;

            ObjectFromId = new Dictionary<int, Transform>(SchematicData.Blocks.Count + 1)
            {
                { data.RootObjectId, transform },
            };

            CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);
            AddRigidbodies();

            // There is no way to configure delay.
            float delay = 0f;
            if (delay >= 0f)
            {
                Timing.RunCoroutine(SpawnDelayed());
            }
            else
            {
                foreach (NetworkIdentity networkIdentity in NetworkIdentities)
                    NetworkServer.Spawn(networkIdentity.gameObject);

                //AddAnimators();
                IsBuilt = true;
            }

            bool animated = AddAnimators();
            bool value = !animated && isStatic;
            IsStatic = value;
            if (value)
                Log.Debug($"Schematic {Name} has no animators, making it static...");

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The base config of the object which contains its properties.
        /// </summary>
        public SchematicSerializable Base;

        /// <summary>
        /// Gets a <see cref="SchematicObjectDataList"/> used to build a schematic.
        /// </summary>
        public SchematicObjectDataList SchematicData { get; private set; }

        /// <summary>
        /// Gets a schematic directory path.
        /// </summary>
        public string DirectoryPath { get; private set; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="GameObject"/> which contains all attached blocks.
        /// </summary>
        public ObservableCollection<GameObject> AttachedBlocks { get; private set; } = new();

        /// <summary>
        /// Gets the schematic name.
        /// </summary>
        public string Name => Base.SchematicName;

        public AnimationController AnimationController => AnimationController.Get(this);

        /// <summary>
        /// Gets a value indicating whether this schematic is at top of transform hierarchy.
        /// </summary>
        public bool IsRootSchematic => transform.root == transform;

        /// <summary>
        /// Gets the read-only collections of <see cref="NetworkIdentity"/> in this schematic.
        /// </summary>
        public ReadOnlyCollection<NetworkIdentity> NetworkIdentities
        {
            get
            {
                if (_networkIdentities != null)
                    return _networkIdentities;

                List<NetworkIdentity> list = new();
                foreach (GameObject block in AttachedBlocks)
                {
                    if (block.TryGetComponent(out NetworkIdentity networkIdentity))
                        list.Add(networkIdentity);
                }

                _networkIdentities = list.AsReadOnly();
                return _networkIdentities;
            }
        }

        public ReadOnlyCollection<AdminToyBase> AdminToyBases
        {
            get
            {
                if (_adminToyBases != null)
                    return _adminToyBases;

                List<AdminToyBase> list = new();
                foreach (NetworkIdentity netId in NetworkIdentities)
                {
                    if (netId.TryGetComponent(out AdminToyBase adminToyBase))
                        list.Add(adminToyBase);
                }

                _adminToyBases = list.AsReadOnly();
                return _adminToyBases;
            }
        }

        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                foreach (AdminToyBase toy in AdminToyBases)
                {
                    if (toy.TryGetComponent(out PrimitiveObject primitiveObject))
                    {
                        primitiveObject.IsStatic = value;
                        continue;
                    }

                    if (toy.TryGetComponent(out LightSourceObject lightSourceObject))
                    {
                        // lightSourceObject.IsStatic = value;
                        lightSourceObject.IsStatic = false;
                    }
                }

                _isStatic = value;
            }
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (IsRootSchematic && Base.SchematicName != name.Split('-')[1])
            {
                SchematicObject newObject = ObjectSpawner.SpawnSchematic(Base, transform.position, transform.rotation, transform.localScale, true, null);

                if (newObject != null)
                {
                    //API.SpawnedObjects[API.SpawnedObjects.IndexOf(this)] = newObject;
                    Destroy();
                    return;
                }

                Base.SchematicName = name.Replace("CustomSchematic-", string.Empty);
            }

            foreach (GameObject gameObject in AttachedBlocks)
            {
                if (gameObject.TryGetComponent(out WorkstationObject _))
                {
                    NetworkServer.UnSpawn(gameObject);

                    SchematicBlockData block = SchematicData.Blocks.Find(c => c.ObjectId == _transformProperties[gameObject.transform.GetInstanceID()]);
                    gameObject.transform.position = transform.position + block.Position;
                    gameObject.transform.eulerAngles = transform.eulerAngles + block.Rotation;
                    gameObject.transform.localScale = Vector3.Scale(transform.localScale, block.Scale);

                    NetworkServer.Spawn(gameObject);

                    continue;
                }

                if (gameObject.TryGetComponent(out LockerObject locker))
                {
                    locker.UpdateObject();
                    continue;
                }
            }

            if (IsStatic)
            {
                foreach (AdminToyBase adminToyBase in AdminToyBases)
                {
                    if (adminToyBase.TryGetComponent(out PrimitiveObject primitiveObject))
                        primitiveObject.UpdateObject();
                }
            }

            if (!IsRootSchematic)
                return;
        }

        private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
        {
            Transform childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.
            int[] parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

            // Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
            foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
            {
                if (parentSchematics.Contains(block.ParentId)) // The block is a child of some schematic inside "parentSchematics" array, therefore it will be skipped to avoid spawning it and its children twice.
                    continue;

                CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
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
                        pickup.GameObject.AddComponent<PickupComponent>();
                    }

                    gameObject = pickup.GameObject;
                    gameObject.name = block.Name;

                    NetworkServer.UnSpawn(gameObject);

                    gameObject.transform.parent = parentTransform;
                    gameObject.transform.localPosition = block.Position;
                    gameObject.transform.localEulerAngles = block.Rotation;
                    gameObject.transform.localScale = block.Scale;

                    NetworkServer.Spawn(gameObject);
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
                            case LockerType.RifleRack: { locker = ObjectHelper.RifleRackObject; break; }
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

                    gameObject = ObjectSpawner.SpawnSchematic(schematicName, transform.position + block.Position, Quaternion.Euler(transform.eulerAngles + block.Rotation), null, true, null).gameObject;
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
                    Log.Warning($"{gameObject.name} block of {Name} should have a {animatorName} animator attached, but the file does not exist!");
                    return false;
                }

                animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
            }

            animatorController = (RuntimeAnimatorController)animatorObject;
            return true;
        }

        private bool AddAnimators()
        {
            bool isAnimated = false;
            if (!_animators.IsEmpty())
            {
                isAnimated = true;
                foreach (KeyValuePair<GameObject, RuntimeAnimatorController> pair in _animators)
                    pair.Key.AddComponent<Animator>().runtimeAnimatorController = pair.Value;
            }

            _animators = null;
            AssetBundle.UnloadAllAssetBundles(false);
            return isAnimated;
        }

        private IEnumerator<float> SpawnDelayed()
        {
            foreach (NetworkIdentity networkIdentity in NetworkIdentities)
            {
                NetworkServer.Spawn(networkIdentity.gameObject);
                yield return Timing.WaitForSeconds(0f); // from config
            }

            IsBuilt = true;
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

        private bool _isStatic;
        private ReadOnlyCollection<NetworkIdentity>? _networkIdentities;
        private ReadOnlyCollection<AdminToyBase>? _adminToyBases;
        private Dictionary<int, int> _transformProperties = new();
        private Dictionary<GameObject, RuntimeAnimatorController> _animators = new();
    }
}