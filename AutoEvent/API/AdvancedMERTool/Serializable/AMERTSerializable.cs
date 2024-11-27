using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Exiled.CustomItems.API.Features;
using Footprinting;
using PluginAPI.Core;
using Utils;
using CommandSystem;
using MER.Lite.Components;
using PluginAPI.Core.Items;
using RemoteAdmin;
using AutoEvent.API.Enums;
using CustomPlayerEffects;
using MER.Lite.Objects;

namespace AutoEvent.API.AdvancedMERTool.Serializable
{
    [Serializable]
    public class InteractablePickupSerializable : AMERTDTO
    {
        public InvokeType InvokeType;
        public IPActionType ActionType;
        public bool CancelActionWhenActive;
        public List<AnimationDTO> animationDTOs;
        public WarheadActionType warheadActionType;
        public List<MessageModule> MessageModules;
        public List<DropItem> dropItems;
        public List<CommandModule> commandings;
        public Scp914Mode Scp914Mode;
        public List<ExplodeModule> ExplodeModules;
        public List<EffectGivingModule> effectGivingModules;
    }
    
    [Serializable]
    public class CustomColliderSerializable : AMERTDTO
    {
        public ColliderActionType ColliderActionType;
        public CollisionType CollisionType;
        public DetectType DetectType;
        //public bool Invisible;
        //public float ContactOffSet;
        public float Amount;
        public List<AnimationDTO> animationDTOs;
        public WarheadActionType warheadActionType;
        public List<MessageModule> MessageModules;
        public List<DropItem> dropItems;
        public List<CommandModule> commandings;
        public List<ExplodeModule> ExplodeModules;
        public List<EffectGivingModule> effectGivingModules;
    }

    [Serializable]
    public class AMERTDTO
    {
        public string ObjectId;
    }

    [Serializable]
    public class AMERTInteractable : MonoBehaviour
    {
        public AMERTDTO Base;
    }

    [Flags]
    [Serializable]
    public enum ColliderActionType
    {
        ModifyHealth = 1,
        GiveEffect = 2,
        SendMessage = 4,
        PlayAnimation = 8,
        SendCommand = 16,
        Warhead = 32,
        Explode = 64
    }

    [Flags]
    [Serializable]
    public enum CollisionType
    {
        OnEnter = 1,
        OnStay = 2,
        OnExit = 4
    }

    [Flags]
    [Serializable]
    public enum DetectType
    {
        Pickup = 1,
        Player = 2,
        Projectile = 4
    }

    [Serializable]
    public class EffectGivingModule : RandomExecutionModule
    {
        public EffectFlag EffectFlag;
        public StatusEffect effectType;
        public SendType GivingTo;
        public byte Inensity;
        public float Duration;
        
        public override void Execute(params object[] args)
        {
            List<EffectGivingModule> modules = args[0] as List<EffectGivingModule>;
            Player hub = args[1] as Player;

            foreach (EffectGivingModule module in modules)
            {
                List<Player> list = new List<Player> { };
                if (module.GivingTo.HasFlag(SendType.Interactor))
                    list.Add(hub);
                if (module.GivingTo.HasFlag(SendType.AllExceptAboveOne))
                    list.AddRange(Player.GetPlayers().Where(x => x != hub));
                if (module.GivingTo.HasFlag(SendType.Alive))
                    list.AddRange(Player.GetPlayers().Where(x => x.IsAlive));
                if (module.GivingTo.HasFlag(SendType.Spectators))
                    list.AddRange(Player.GetPlayers().Where(x => !x.IsAlive));
                list = Player.GetPlayers().Where(x => list.Contains(x)).ToList();

                foreach (Player player in list)
                {
                    if (module.EffectFlag.HasFlag(EffectFlag.Disable))
                        Extensions.DisableEffect(player, module.effectType);
                    else if (module.EffectFlag.HasFlag(EffectFlag.Enable))
                    {
                        Extensions.GiveEffect(player, module.effectType, module.Inensity, module.Duration, module.EffectFlag.HasFlag(EffectFlag.ModifyDuration));
                    }
                }
            }
        }
    }

    [Serializable]
    public class ExplodeModule : RandomExecutionModule
    {
        public bool FFon;
        public bool EffectOnly;
        public SVector3 LocalPosition;

        public override void Execute(params object[] args)
        {
            List<ExplodeModule> modules = args[0] as List<ExplodeModule>;
            Transform transform = args[1] as Transform;
            ReferenceHub hub = args[2] as ReferenceHub;
            foreach (ExplodeModule module in modules)
            {
                if (module.EffectOnly)
                    ExplosionUtils.ServerSpawnEffect(transform.TransformPoint(module.LocalPosition), ItemType.GrenadeHE);
                else
                    ExplosionUtils.ServerExplode(transform.TransformPoint(module.LocalPosition), module.FFon ? new Footprint(ReferenceHub.LocalHub) : new Footprint(hub), ExplosionType.Custom);
            }
        }
    }

    [Serializable]
    public class MessageModule : RandomExecutionModule
    {
        public SendType SendType;
        public string MessageContent;
        public MessageType MessageType;
        public float Duration;

        /// <summary>
        /// messageModules, FuncDictionary, Interactor, transform (health)  
        /// '', '', '', pickup  
        /// '', '', '', teleporterT  
        /// '', '', '', Transform (CC)
        /// </summary>
        /// <param name="args"></param>
        public override void Execute(params object[] args)
        {
            List<MessageModule> messages = args[0] as List<MessageModule>;
            Dictionary<string, Func<object[], string>> pairs = args[1] as Dictionary<string, Func<object[], string>>;
            Player interactor = args[2] as Player;
            foreach (MessageModule module in messages)
            {
                string Content = module.MessageContent;
                foreach (string v in pairs.Keys)
                {
                    try
                    {
                        Content = Content.Replace(v, pairs[v]((object[])args.Skip(2).ToArray()));
                    }
                    catch (Exception _) { }
                }
                try
                {
                    Content = ServerConsole.singleton.NameFormatter.ProcessExpression(Content);
                }
                catch (Exception e) { }
                if (module.MessageType == MessageType.Cassie)
                {
                    Cassie.Message(Content);
                }
                else
                {
                    List<Player> targets = new List<Player> { };
                    if (module.SendType.HasFlag(SendType.AllExceptAboveOne))
                        targets.AddRange(Player.GetPlayers().Where(x => x != interactor));
                    if (module.SendType.HasFlag(SendType.Spectators))
                        targets.AddRange(Player.GetPlayers().Where(x => !x.IsAlive));
                    if (module.SendType.HasFlag(SendType.Alive))
                        targets.AddRange(Player.GetPlayers().Where(x => x.IsAlive));
                    if (module.SendType.HasFlag(SendType.Interactor))
                        targets.Add(interactor);

                    targets = Player.GetPlayers().Where(x => targets.Contains(x)).ToList();

                    foreach (Player p in targets)
                    {
                        if (module.MessageType == MessageType.BroadCast)
                        {
                            p.SendBroadcast(Content, (ushort)Math.Round(module.Duration));
                        }
                        else
                        {
                            p.ReceiveHint(Content, module.Duration);
                        }
                    }
                }
            }
        }
    }

    [Flags]
    [Serializable]
    public enum EffectFlag
    {
        Disable = 1,
        Enable = 2,
        ModifyDuration = 4,
        ForceDuration = 8,
        ModifyIntensity = 16,
        ForceIntensity = 32
    }

    [Flags]
    [Serializable]
    public enum Scp914Mode
    {
        Rough = 0,
        Coarse = 1,
        OneToOne = 2,
        Fine = 3,
        VeryFine = 4
    }

    [Flags]
    [Serializable]
    public enum TeleportInvokeType
    {
        Enter = 1,
        Exit = 2,
        Collide = 4
    }

    [Serializable]
    public class AnimationDTO : RandomExecutionModule
    {
        public string Animator;
        public string Animation;
        public AnimationType AnimationType;
    }

    [Serializable]
    public class AnimationModule : RandomExecutionModule
    {
        public Animator Animator;
        public string AnimationName;
        public AnimationType AnimationType;

        public override void Execute(params object[] args)
        {
            List<AnimationModule> modules = args[0] as List<AnimationModule>;
            foreach (AnimationModule module in modules)
            {
                if (module.Animator != null)
                {
                    if (module.AnimationType == AnimationType.Start)
                    {
                        module.Animator.Play(module.AnimationName);
                        module.Animator.speed = 1f;
                    }
                    else
                        module.Animator.speed = 0f;
                }
            }
        }

        public static List<AnimationModule> GetModules(List<AnimationDTO> list, GameObject gameObject)
        {
            List<AnimationModule> modules = new List<AnimationModule> { };
            foreach (AnimationDTO dTO in list)
            {
                if (!AdvancedMERTools.FindObjectWithPath(gameObject.GetComponentInParent<SchematicObject>().transform, dTO.Animator).TryGetComponent(out Animator animator))
                {
                    ServerConsole.AddLog("Cannot find appopriate animator!");
                    continue;
                }
                modules.Add(new AnimationModule
                {
                    Animator = animator,
                    AnimationName = dTO.Animation,
                    AnimationType = dTO.AnimationType,
                    ChanceWeight = dTO.ChanceWeight,
                    ForceExecute = dTO.ForceExecute
                });
            }
            return modules;
        }
    }

    [Flags]
    [Serializable]
    public enum DeadType
    {
        Disappear = 1,
        GetRigidbody = 2,
        DynamicDisappearing = 4,
        Explode = 8,
        ResetHP = 16,
        PlayAnimation = 32,
        Warhead = 64,
        SendMessage = 128,
        DropItems = 256,
        SendCommand = 512,
        GiveEffect = 1024
    }

    [Flags]
    [Serializable]
    public enum IPActionType
    {
        Disappear = 1,
        Explode = 2,
        PlayAnimation = 4,
        Warhead = 8,
        SendMessage = 16,
        DropItems = 32,
        SendCommand = 64,
        UpgradeItem = 128,
        GiveEffect = 256
    }

    [Flags]
    [Serializable]
    public enum InvokeType
    {
        Searching = 1,
        Picked = 2
    }

    [Serializable]
    public class RandomExecutionModule
    {
        public float ChanceWeight;
        public bool ForceExecute;

        public static RandomExecutionModule GetSingleton<T>() where T : RandomExecutionModule, new()
        {
            if (!AdvancedMERTools.Singleton.TypeSingletonPair.TryGetValue(typeof(T), out RandomExecutionModule type))
            {
                AdvancedMERTools.Singleton.TypeSingletonPair.Add(typeof(T), type = new T());
            }
            return type;
        }

        public static List<T> SelectList<T>(List<T> list) where T : RandomExecutionModule, new()
        {
            float Chance = list.Sum(x => x.ChanceWeight);
            Chance = UnityEngine.Random.Range(0f, Chance);
            List<T> output = new List<T> { };
            foreach (T element in list)
            {
                if (element.ForceExecute)
                    output.Add(element);
                else
                {
                    if (Chance <= 0)
                        continue;
                    Chance -= element.ChanceWeight;
                    if (Chance <= 0)
                    {
                        output.Add(element);
                    }
                }
            }
            return output;
        }

        public virtual void Execute(params object[] args) { }
    }

    [Serializable]
    public enum AnimationType
    {
        Start,
        Stop
    }

    [Flags]
    [Serializable]
    public enum WarheadActionType
    {
        Start = 1,
        Stop = 2,
        Lock = 4,
        UnLock = 8,
        Disable = 16,
        Enable = 32
    }

    [Serializable]
    public enum MessageType
    {
        Cassie,
        BroadCast,
        Hint
    }

    [Flags]
    [Serializable]
    public enum SendType
    {
        Interactor = 1,
        AllExceptAboveOne = 2,
        Alive = 4,
        Spectators = 8
    }

    [Serializable]
    public class DropItem : RandomExecutionModule
    {
        public ItemType ItemType;
        public uint CustomItemId;
        public int Count;
        public SVector3 DropLocalPosition;

        public override void Execute(params object[] args)
        {
            List<DropItem> items = args[0] as List<DropItem>;
            Transform transform = args[1] as Transform;

            foreach (DropItem item in items)
            {
                Vector3 vector3 = transform.TransformPoint(item.DropLocalPosition);
                if (item.CustomItemId != 0 && CustomItem.TryGet(item.CustomItemId, out CustomItem custom))
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        custom.Spawn(vector3);
                    }
                }
                else
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        ItemPickup pickup = ItemPickup.Create(item.ItemType, vector3, Quaternion.identity);
                        pickup.GameObject.AddComponent<PickupComponent>();
                    }
                }
            }
        }
    }

    [Serializable]
    public class SVector3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(SVector3 sVector) => new Vector3(sVector.x, sVector.y, sVector.z);
    }

    [Serializable]
    public class CommandModule : RandomExecutionModule
    {
        public string CommandContext;

        public override void Execute(params object[] args)
        {
            List<CommandModule> modules = args[0] as List<CommandModule>;
            Dictionary<string, Func<object[], string>> pairs = args[1] as Dictionary<string, Func<object[], string>>;

            foreach (CommandModule module in modules)
            {
                string Content = module.CommandContext;
                foreach (string v in pairs.Keys)
                {
                    try
                    {
                        Content = Content.Replace(v, pairs[v]((object[])args.Skip(2).ToArray()));
                    }
                    catch (Exception e) { }
                }
                Content = ServerConsole.singleton.NameFormatter.ProcessExpression(Content);
                ExecuteCommand(Content);
            }
        }
        
        public void ExecuteCommand(string context)
        {
            string[] array = context.Trim().Split(new char[] { ' ' }, 512, StringSplitOptions.RemoveEmptyEntries);
            ICommand command1;
            if (CommandProcessor.RemoteAdminCommandHandler.TryGetCommand(array[0], out command1))
            {
                command1.Execute(array.Segment(1), ServerConsole.Scs, out _);
            }
        }
    }
}
