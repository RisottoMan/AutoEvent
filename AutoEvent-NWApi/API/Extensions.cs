using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using UnityEngine;
using PlayerRoles;
using SCPSLAudioApi.AudioCore;
using VoiceChat;
using AutoEvent.API.Schematic.Objects;
using AutoEvent.API.Schematic;
using PluginAPI.Core;
using InventorySystem.Items.Pickups;
using PlayerStatsSystem;
using PluginAPI.Helpers;
using System.Reflection;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Games.Battle;
using AutoEvent.Games.Line;
using CustomPlayerEffects;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using Object = UnityEngine.Object;

namespace AutoEvent
{
    public static class Extensions
    {
        public static ReferenceHub AudioBot = new ReferenceHub();

        private static MethodInfo sendSpawnMessage;

        public static MethodInfo SendSpawnMessage => sendSpawnMessage ?? (sendSpawnMessage =
            typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Static | BindingFlags.NonPublic));

        public static void SetRole(this Player player, RoleTypeId newRole, RoleSpawnFlags spawnFlags)
        {
            player.ReferenceHub.roleManager.ServerSetRole(newRole, RoleChangeReason.RemoteAdmin, spawnFlags);
        }

        public static void GiveLoadout(this Player player, List<Loadout> loadouts, LoadoutFlags flags = LoadoutFlags.None)
        {
            Loadout loadout;
            if (loadouts.Count == 1)
            {
                loadout = loadouts[0];
                goto assignLoadout;
            }

            foreach (var loadout1 in loadouts.Where(x => x.Chance <= 0))
                loadout1.Chance = 1;
            
            int totalChance = loadouts.Sum(x => x.Chance);
            
            for (int i = 0; i < loadouts.Count - 1; i++)
            {
                if (UnityEngine.Random.Range(0, totalChance) <= loadouts[i].Chance)
                {
                    loadout = loadouts[i];
                    goto assignLoadout;
                }
            }
            loadout = loadouts[loadouts.Count - 1];
            assignLoadout:
            GiveLoadout(player, loadout, flags);
        }
        public static void GiveLoadout(this Player player, Loadout loadout, LoadoutFlags flags = LoadoutFlags.None)
        {
            if (loadout.Roles is not null && loadout.Roles.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreRole))
            {
                if (loadout.Roles.Count == 1)
                {
                    player.SetRole(loadout.Roles.First().Key);
                }
                else
                {
                    List<KeyValuePair<RoleTypeId, int>> list = loadout.Roles.ToList<KeyValuePair<RoleTypeId, int>>();
                    int roleTotalChance = list.Sum(x => x.Value);
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        if (UnityEngine.Random.Range(0, roleTotalChance) <= list[i].Value)
                        {
                            player.SetRole(list[i].Key, RoleChangeReason.Respawn, RoleSpawnFlags.None);
                            goto assignRole;
                        }
                    }
                    player.SetRole(list[list.Count - 1].Key, RoleChangeReason.Respawn, RoleSpawnFlags.None);
                }
            }
            assignRole:
            
            if (loadout.Items is not null && loadout.Items.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreItems))
            {
                foreach (var item in loadout.Items)
                {
                    if(flags.HasFlag(LoadoutFlags.IgnoreWeapons) && !item.IsWeapon())
                        player.AddItem(item);
                }
            }

            if ((loadout.InfiniteAmmo && !flags.HasFlag(LoadoutFlags.IgnoreInfiniteAmmo)) || flags.HasFlag(LoadoutFlags.ForceInfiniteAmmo))
            {
                player.AmmoBag[ItemType.Ammo9x19] = ushort.MaxValue;
                player.AmmoBag[ItemType.Ammo12gauge] = ushort.MaxValue;
                player.AmmoBag[ItemType.Ammo44cal] = ushort.MaxValue;
                player.AmmoBag[ItemType.Ammo556x45] = ushort.MaxValue;
                player.AmmoBag[ItemType.Ammo762x39] = ushort.MaxValue;
            }
            if(loadout.Health != 0 && !flags.HasFlag(LoadoutFlags.IgnoreHealth))
                player.Health = loadout.Health;
            if (loadout.Health == -1 && !flags.HasFlag(LoadoutFlags.IgnoreGodMode))
            {
                player.IsGodModeEnabled = true;
            }
            
            if(loadout.ArtificialHealth != -1 && !flags.HasFlag(LoadoutFlags.IgnoreAHP))
                player.ArtificialHealth = loadout.ArtificialHealth;
            
            if(!flags.HasFlag(LoadoutFlags.IgnoreSize))
                player.SetPlayerScale(loadout.Size);
            if (loadout.Effects is not null && loadout.Effects.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreEffects))
            {
                foreach (var effect in loadout.Effects)
                {
                    player.EffectsManager.ChangeState(effect.Type.ToString(), effect.Intensity, effect.Duration,
                        effect.AddDuration);
                }
            }

        }
        public static void SetRole(this Player player, RoleTypeId newRole, RoleChangeReason reason,
            RoleSpawnFlags spawnFlags)
        {
            player.ReferenceHub.roleManager.ServerSetRole(newRole, reason, spawnFlags);
        }

        public static void SetPlayerScale(this Player target, Vector3 scale)
        {
            if (target.GameObject.transform.localScale == scale) return;

            try
            {
                NetworkIdentity identity = target.ReferenceHub.networkIdentity;
                target.GameObject.transform.localScale = scale;
                foreach (Player player in Player.GetPlayers())
                {
                    SendSpawnMessage?.Invoke(null, new object[2] { identity, player.Connection });
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogDebug($"Scale error has occured.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{ex}", LogLevel.Debug);

            }
        }

        public static bool IsWeapon(this ItemType item) => item 
                is ItemType.GunA7      or ItemType.GunCom45    or ItemType.GunCrossvec 
                or ItemType.GunLogicer or ItemType.GunRevolver or ItemType.GunShotgun 
                or ItemType.GunAK      or ItemType.GunCOM15    or ItemType.GunCOM18 
                or ItemType.GunE11SR   or ItemType.GunFSP9     or ItemType.GunFRMG0    
                or ItemType.Jailbird   or ItemType.MicroHID    or ItemType.ParticleDisruptor  
                or ItemType.GrenadeHE  or ItemType.SCP018; // Dont add weapons
        
        
        public static void SetPlayerAhp(this Player player, float amount, float limit = 75, float decay = 1.2f,
            float efficacy = 0.7f, float sustain = 0, bool persistant = false)
        {
            if (amount > 100) amount = 100;

            player.ReferenceHub.playerStats.GetModule<AhpStat>()
                .ServerAddProcess(amount, limit, decay, efficacy, sustain, persistant);
        }

        public static void GiveEffect(this Player ply, StatusEffect effect, byte intensity, float duration = 0f, bool addIntensity = false) =>             
            ply.EffectsManager.ChangeState(effect.ToString(), intensity, duration, addIntensity);
        public static Type GetStatusEffectBaseType(this StatusEffect effect)
        {
            // I should have done this via reflection but oh well... 
            switch (effect)
            {
                case StatusEffect.Asphyxiated: return typeof(Asphyxiated); 
                case StatusEffect.Bleeding: return typeof(Bleeding);
                case StatusEffect.Blinded: return typeof(Blinded);
                case StatusEffect.Burned: return typeof(Burned);
                case StatusEffect.Concussed: return typeof(Concussed);
                case StatusEffect.Corroding: return typeof(Corroding);
                case StatusEffect.Deafened: return typeof(Deafened);
                case StatusEffect.Decontaminating: return typeof(Decontaminating);
                case StatusEffect.Disabled: return typeof(Disabled);
                case StatusEffect.Ensnared: return typeof(Ensnared);
                case StatusEffect.Exhausted: return typeof(Exhausted);
                case StatusEffect.Flashed: return typeof(Flashed);
                case StatusEffect.Hemorrhage: return typeof(Hemorrhage);
                case StatusEffect.Hypothermia: return typeof(Hypothermia);
                case StatusEffect.Invigorated: return typeof(Invigorated);
                case StatusEffect.Invisible: return typeof(Invisible);
                case StatusEffect.Poisoned: return typeof(Poisoned);
                case StatusEffect.Scanned: return typeof(Scanned);
                case StatusEffect.Scp207: return typeof(Scp207);
                case StatusEffect.Scp1853: return typeof(Scp1853);
                case StatusEffect.Stained: return typeof(Stained);
                case StatusEffect.Traumatized: return typeof(Traumatized);
                case StatusEffect.Vitality: return typeof(Vitality);
                case StatusEffect.AmnesiaItems: return typeof(AmnesiaItems);
                case StatusEffect.AmnesiaVision: return typeof(AmnesiaVision);
                case StatusEffect.AntiScp207: return typeof(AntiScp207);
                case StatusEffect.BodyshotReduction: return typeof(BodyshotReduction);
                case StatusEffect.CardiacArrest: return typeof(CardiacArrest);
                case StatusEffect.DamageReduction: return typeof(DamageReduction);
                case StatusEffect.InsufficientLighting: return typeof(InsufficientLighting);
                case StatusEffect.MovementBoost: return typeof(MovementBoost);
                case StatusEffect.PocketCorroding: return typeof(PocketCorroding);
                case StatusEffect.RainbowTaste: return typeof(RainbowTaste);
                case StatusEffect.SeveredHands: return typeof(SeveredHands);
                case StatusEffect.SinkHole: return typeof(Sinkhole);
                case StatusEffect.SoundtrackMute: return typeof(SoundtrackMute);
                case StatusEffect.SpawnProtected: return typeof(SpawnProtected);

            }

            return null;
        }

        public static void TeleportEnd()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.Tutorial, RoleChangeReason.None);
                player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
            }
        }

        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine(Path.Combine(Paths.GlobalPlugins.Plugins, "Music"), audioFile);

            var audioPlayer = AudioPlayerBase.Get(AudioBot);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
            audioPlayer.Volume = volume * (AutoEvent.Singleton.Config.Volume/100f);
            audioPlayer.Loop = loop;
            audioPlayer.Play(0);
        }

        public static void StopAudio()
        {
            var audioPlayer = AudioPlayerBase.Get(AudioBot);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }
        }

        public static ReferenceHub AddDummy()
        {
            var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(0);
            var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
            hubPlayer.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

            try
            {
                hubPlayer.nicknameSync.SetNick("MiniGames");
            }
            catch (Exception) { }

            return hubPlayer;
        }

        public static void RemoveDummy()
        {
            var audioPlayer = AudioPlayerBase.Get(AudioBot);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }

            AudioBot.OnDestroy();
            CustomNetworkManager.TypedSingleton.OnServerDisconnect(AudioBot.connectionToClient);
            Object.Destroy(AudioBot.gameObject);
        }

        public static bool IsExistsMap(string schematicName)
        {
            var data = MapUtils.GetSchematicDataByName(schematicName);
            if (data == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static SchematicObject LoadMap(string nameSchematic, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            return ObjectSpawner.SpawnSchematic(nameSchematic, pos, rot, scale);
        }

        public static void UnLoadMap(SchematicObject scheme)
        {
            scheme.Destroy();
        }

        public static void CleanUpAll()
        {
            foreach (var item in Object.FindObjectsOfType<ItemPickupBase>())
            {
                GameObject.Destroy(item.gameObject);
            }

            foreach (var ragdoll in Object.FindObjectsOfType<BasicRagdoll>())
            {
                GameObject.Destroy(ragdoll.gameObject);
            }
        }

        public static void Broadcast(string text, ushort time)
        {
            Map.ClearBroadcasts();
            Map.Broadcast(time, text);
        }

        public static void GrenadeSpawn(float fuseTime, Vector3 pos, float scale)
        {
            var identifier = new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext());
            var item = ReferenceHub.HostHub.inventory.CreateItemInstance(identifier, false) as ThrowableItem;

            TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, pos, Quaternion.identity);
            grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenade.transform.localScale = new Vector3(scale, scale, scale);

            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }
    }
}
