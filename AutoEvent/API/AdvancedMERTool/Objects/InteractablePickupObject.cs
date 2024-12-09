namespace AutoEvent.API.AdvancedMERTool.Objects
{
    using System;
    using System.Collections.Generic;
    using PluginAPI.Core;
    using UnityEngine;
    using InventorySystem.Items.Pickups;
    using Serializable;
    
    public class InteractablePickupObject : AMERTInteractable
    {
        void Start()
        {
            this.Base = base.Base as InteractablePickupSerializable;
            
            if (!this.gameObject.TryGetComponent(out Pickup))
                return;
            
            if (Pickup != null)
            {
                AdvancedMERTools.Singleton.InteractablePickups.Add(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public void RunProcess(Player player, ItemPickupBase pickup, out bool Remove)
        {
            Remove = false;
            if (pickup != this.Pickup)
            {
                return;
            }
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (Base.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case IPActionType.Disappear:
                            Remove = true;
                            break;
                        case IPActionType.Explode:
                            ExplodeModule.GetSingleton<ExplodeModule>().Execute(ExplodeModule.SelectList<ExplodeModule>(Base.ExplodeModules), this.transform, player.ReferenceHub);
                            break;
                        case IPActionType.PlayAnimation:
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
                        case IPActionType.Warhead:
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
                        case IPActionType.SendMessage:
                            MessageModule.GetSingleton<MessageModule>().Execute(MessageModule.SelectList<MessageModule>(Base.MessageModules), Formatter, player, pickup);
                            break;
                        case IPActionType.DropItems:
                            DropItem.GetSingleton<DropItem>().Execute(DropItem.SelectList<DropItem>(Base.dropItems), this.transform);
                            break;
                        case IPActionType.SendCommand:
                            CommandModule.GetSingleton<CommandModule>().Execute(CommandModule.SelectList<CommandModule>(Base.commandings), Formatter, player, pickup);
                            break;
                        case IPActionType.UpgradeItem:
                            if (player.GameObject.TryGetComponent<Collider>(out Collider col))
                            {
                                List<int> vs = new List<int> { };
                                for (int j = 0; j < 5; j++)
                                {
                                    if (Base.Scp914Mode.HasFlag((Scp914Mode)j))
                                    {
                                        vs.Add(j);
                                    }
                                }
                                Scp914.Scp914Upgrader.Upgrade(new Collider[] { col }, Scp914.Scp914Mode.Held, (Scp914.Scp914KnobSetting)vs.RandomItem());
                            }
                            break;
                        case IPActionType.GiveEffect:
                            EffectGivingModule.GetSingleton<EffectGivingModule>().Execute(EffectGivingModule.SelectList<EffectGivingModule>(Base.effectGivingModules), player);
                            break;
                    }
                }
            }
        }

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.InteractablePickups.Remove(this);
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
            { "{o_pos}", vs => { Vector3 pos = (vs[1] as ItemPickupBase).Position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{o_room}", vs => (vs[1] as ItemPickupBase).Position.ToString() },
            { "{o_zone}", vs => (vs[1] as ItemPickupBase).Position.ToString() }
        };

        public ItemPickupBase Pickup;

        public new InteractablePickupSerializable Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };
    }
}