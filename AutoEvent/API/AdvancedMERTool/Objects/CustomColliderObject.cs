namespace AutoEvent.API.AdvancedMERTool.Objects
{
    using System;
    using System.Collections.Generic;
    using InventorySystem.Items.ThrowableProjectiles;
    using MapGeneration;
    using Mirror;
    using PluginAPI.Core;
    using UnityEngine;
    using InventorySystem.Items.Pickups;
    using Serializable;
    
    public class CustomColliderObject : AMERTInteractable
    {
        void Start()
        {
            this.Base = base.Base as CustomColliderSerializable;
            AdvancedMERTools.Singleton.CustomColliders.Add(this);
            CustomColliderObject[] customColliders = gameObject.GetComponents<CustomColliderObject>();
            if (customColliders.Length > 1 && customColliders[0] != this)
            {
                MEC.Timing.CallDelayed(0.1f, () => 
                {
                    meshCollider = customColliders[0].meshCollider;
                });
                return;
            }
            Vector3[] vs = new Vector3[] { transform.position, transform.eulerAngles };
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;

            MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combineInstances);
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            
            transform.GetComponentsInChildren<AdminToys.PrimitiveObjectToy>().ForEach(x => 
            {
                x.gameObject.SetActive(false);
                NetworkServer.Destroy(x.gameObject);
            });
            
            transform.position = vs[0];
            transform.eulerAngles = vs[1];
        }

        void OnTriggerEnter(Collider collider)
        {
            if (Base.CollisionType.HasFlag(CollisionType.OnEnter))
                RunProcess(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            if (Base.CollisionType.HasFlag(CollisionType.OnExit))
                RunProcess(collider);
        }

        void OnTriggerStay(Collider collider)
        {
            if (Base.CollisionType.HasFlag(CollisionType.OnStay))
                RunProcess(collider);
        }

        void RunProcess(Collider collider)
        {
            bool flag = false;
            Player target = null;
            
            if (collider.gameObject.TryGetComponent(out ItemPickupBase ipb))
            {
                if (Base.DetectType.HasFlag(DetectType.Pickup) && ipb != null)
                {
                    target = Player.Get(ipb.PreviousOwner.PlayerId);
                    flag = true;
                }
            }
            if (Base.DetectType.HasFlag(DetectType.Player) && Player.TryGet(collider.gameObject, out target))
            {
                flag = target.RoleBase.ActiveTime > 0.25f;
            }
            ThrownProjectile projectile = collider.GetComponentInParent<ThrownProjectile>();
            if (Base.DetectType.HasFlag(DetectType.Projectile) && projectile != null)
            {
                target = Player.Get(projectile.PreviousOwner.PlayerId);
                flag = true;
            }
            if (!flag)
                return;
            foreach (ColliderActionType type in Enum.GetValues(typeof(ColliderActionType)))
            {
                if (Base.ColliderActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case ColliderActionType.ModifyHealth:
                            if (target != null)
                            {
                                if (Base.Amount > 0)
                                    target.Heal(Base.Amount);
                                else
                                    target.Damage(-1 * Base.Amount, "Test");
                            }
                            break;
                        case ColliderActionType.Explode:
                            ExplodeModule.GetSingleton<ExplodeModule>().Execute(ExplodeModule.SelectList<ExplodeModule>(Base.ExplodeModules), this.transform, target.ReferenceHub);
                            break;
                        case ColliderActionType.PlayAnimation:
                            if (modules.Count == 0)
                            {
                                modules = AnimationModule.GetModules(Base.animationDTOs, this.gameObject);
                                if (modules.Count == 0)
                                {
                                    break;
                                }
                            }
                            AnimationModule.GetSingleton<AnimationModule>().Execute(AnimationModule.SelectList<AnimationModule>(modules));
                            break;
                        case ColliderActionType.Warhead:
                            foreach (WarheadActionType warhead in Enum.GetValues(typeof(WarheadActionType)))
                            {
                                if (Base.warheadActionType.HasFlag(warhead))
                                {
                                    switch (warhead)
                                    {
                                        case WarheadActionType.Start:
                                            Warhead.Start();
                                            break;
                                        case WarheadActionType.Stop:
                                            Warhead.Stop();
                                            break;
                                        case WarheadActionType.Lock:
                                            Warhead.IsLocked = true;
                                            break;
                                        case WarheadActionType.UnLock:
                                            Warhead.IsLocked = false;
                                            break;
                                        case WarheadActionType.Disable:
                                            Warhead.LeverStatus = false;
                                            break;
                                        case WarheadActionType.Enable:
                                            Warhead.LeverStatus = true;
                                            break;
                                    }
                                }
                            }
                            break;
                        case ColliderActionType.SendMessage:
                            MessageModule.GetSingleton<MessageModule>().Execute(MessageModule.SelectList<MessageModule>(Base.MessageModules), Formatter, target, gameObject);
                            break;
                        case ColliderActionType.SendCommand:
                            CommandModule.GetSingleton<CommandModule>().Execute(CommandModule.SelectList<CommandModule>(Base.commandings), Formatter, target, gameObject);
                            break;
                        case ColliderActionType.GiveEffect:
                            EffectGivingModule.GetSingleton<EffectGivingModule>().Execute(EffectGivingModule.SelectList<EffectGivingModule>(Base.effectGivingModules), target);
                            break;
                    }
                }
            }
        }

        static readonly Dictionary<string, Func<object[], string>> Formatter = new Dictionary<string, Func<object[], string>>
        {
            { "{p_i}", vs => (vs[0] as Player).PlayerId.ToString() },
            { "{p_name}", vs => (vs[0] as Player).Nickname.ToString() },
            { "{p_pos}", vs => { Vector3 pos = (vs[0] as Player).Position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{p_room}", vs => (vs[0] as Player).Room.Name.ToString() },
            { "{p_zone}", vs => (vs[0] as Player).Zone.ToString() },
            { "{p_role}", vs => (vs[0] as Player).Role.GetType().ToString() },
            { "{p_item}", vs => (vs[0] as Player).CurrentItem.ItemTypeId.ToString() },
            { "{o_pos}", vs => { Vector3 pos = (vs[1] as GameObject).transform.position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{o_room}", vs => RoomIdUtils.RoomAtPosition((vs[1] as GameObject).transform.position).Name.ToString() },
            { "{o_zone}", vs => RoomIdUtils.RoomAtPosition((vs[1] as GameObject).transform.position).Zone.ToString() }
        };

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.CustomColliders.Remove(this);
        }

        public MeshCollider meshCollider;

        public new CustomColliderSerializable Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };

        public Transform originalT;
    }
}