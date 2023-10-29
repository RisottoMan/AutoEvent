using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using UnityEngine;
using PlayerRoles;
using SCPSLAudioApi.AudioCore;
using VoiceChat;
using MER.Lite.Objects;
using MER.Lite;
using PluginAPI.Core;
using InventorySystem.Items.Pickups;
using PlayerStatsSystem;
using PluginAPI.Helpers;
using System.Reflection;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Commands.Debug;
using AutoEvent.Events.EventArgs;
using AutoEvent.Games.Battle;
using AutoEvent.Games.Line;
using AutoEvent.Patches;
using CustomPlayerEffects;
using Exiled.API.Features.Items;
using Footprinting;
using InventorySystem.Configs;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using InventorySystem.Items.Jailbird;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using JetBrains.Annotations;
using PlayerRoles.Ragdolls;
using PluginAPI.Core.Items;
using Debug = AutoEvent.Commands.Debug.Debug;
using Item = PluginAPI.Core.Items.Item;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AutoEvent
{
    public static class Extensions
    {
        public static bool JailbirdIsInvincible { get; set; } = true;
        public static List<JailbirdItem> InvincibleJailbirds { get; set; } = new List<JailbirdItem>();
        public static ReferenceHub AudioBot = new ReferenceHub();

        private static MethodInfo sendSpawnMessage;

        public static MethodInfo SendSpawnMessage => sendSpawnMessage ?? (sendSpawnMessage =
            typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Static | BindingFlags.NonPublic));

        public static void SetRole(this Player player, RoleTypeId newRole, RoleSpawnFlags spawnFlags)
        {
            player.ReferenceHub.roleManager.ServerSetRole(newRole, RoleChangeReason.RemoteAdmin, spawnFlags);
        }

        public enum LoadoutCheckMethods
        {
            HasRole,
            HasSomeItems,
            HasAllItems,
            
        }

        public static List<Player> GetAllPlayersWithLoadout(this List<Loadout> loadout, LoadoutCheckMethods checkMethod = LoadoutCheckMethods.HasRole, [CanBeNull] List<Player> players = null)
        {
            List<Player> plyList = new List<Player>();
            foreach (Player ply in players ?? Player.GetPlayers())
            {
                if (ply.HasLoadout(loadout, checkMethod))
                    plyList.Add(ply);
            }

            return plyList;
        }

        public static List<Player> GetAllPlayersWithLoadout(this Loadout loadout, LoadoutCheckMethods checkMethod = LoadoutCheckMethods.HasRole, [CanBeNull] List<Player> players = null) =>
            GetAllPlayersWithLoadout(new List<Loadout>() { loadout }, checkMethod, players);
        public static bool HasLoadout(this Player ply, List<Loadout> loadouts, LoadoutCheckMethods checkMethod = LoadoutCheckMethods.HasRole)
        {
            switch (checkMethod)
            {
                case LoadoutCheckMethods.HasRole:
                    return loadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ply.Role));
                case LoadoutCheckMethods.HasAllItems:
                    return loadouts.Any(loadout => loadout.Items.All(item => ply.Items.Select(itm => itm.ItemTypeId).Contains(item)));
                case LoadoutCheckMethods.HasSomeItems:
                    return loadouts.Any(loadout => loadout.Items.Any(item => ply.Items.Select(itm => itm.ItemTypeId).Contains(item)));
            }

            return false;
        }

        public static void ApplyWeaponEffect(this List<WeaponEffect> effects, ref PlayerDamageArgs ev)
        {
            var globalEffect = effects.FirstOrDefault(x => x.WeaponType == ItemType.None);
            if (globalEffect is not null)
            {
                globalEffect.ApplyGunEffect(ref ev);
                return;
            }

            PlayerDamageArgs args = ev;
            var applicableWeaponEffect = effects.FirstOrDefault(effect => effect.IsCustomWeaponEffect(args));
            if (applicableWeaponEffect is null)
            {
                return;
            } 
            applicableWeaponEffect.ApplyGunEffect(ref ev);
        }
        
        public static bool HasLoadout(this Player ply, Loadout loadout, LoadoutCheckMethods checkMethod = LoadoutCheckMethods.HasRole) =>
            ply.HasLoadout(new List<Loadout>() { loadout }, checkMethod);
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
            RoleTypeId role = RoleTypeId.None;
            RoleSpawnFlags respawnFlags = RoleSpawnFlags.None;
            if (loadout.Roles is not null && loadout.Roles.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreRole))
            {
                if (flags.HasFlag(LoadoutFlags.UseDefaultSpawnPoint))
                    respawnFlags |= RoleSpawnFlags.UseSpawnpoint;
                if (flags.HasFlag(LoadoutFlags.DontClearDefaultItems))
                    respawnFlags |= RoleSpawnFlags.AssignInventory;

                if (loadout.Roles.Count == 1)
                {
                    // player.SetRole(loadout.Roles.First().Key, RoleChangeReason.Respawn, respawnFlags);
                    role = loadout.Roles.First().Key;
                    goto assignRole;
                }
                else
                {
                    List<KeyValuePair<RoleTypeId, int>> list = loadout.Roles.ToList<KeyValuePair<RoleTypeId, int>>();
                    int roleTotalChance = list.Sum(x => x.Value);
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        if (UnityEngine.Random.Range(0, roleTotalChance) <= list[i].Value)
                        {
                            role = list[i].Key;
                            // player.SetRole(list[i].Key, RoleChangeReason.Respawn, respawnFlags);
                            goto assignRole;
                        }
                    }

                    // player.SetRole(list[list.Count - 1].Key, RoleChangeReason.Respawn, respawnFlags);
                    role = list[list.Count - 1].Key;
                    goto assignRole;
                }

                assignRole:
                if (AutoEvent.Singleton.Config.IgnoredRoles.Contains(role))
                {
                    DebugLogger.LogDebug(
                        "AutoEvent is trying to set a player to a role that is apart of IgnoreRoles. This is probably an error. The plugin will instead set players to the lobby role to prevent issues.",
                        LogLevel.Error, true);
                    role = AutoEvent.Singleton.Config.LobbyRole;
                }

                player.SetRole(role, RoleChangeReason.Respawn, respawnFlags);
            }
            if (!flags.HasFlag(LoadoutFlags.DontClearDefaultItems))
            {
                player.ClearInventory();
            }

            if (loadout.Items is not null && loadout.Items.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreItems))
            {
                foreach (var item in loadout.Items)
                {
                    if (flags.HasFlag(LoadoutFlags.IgnoreWeapons) && item.IsWeapon())
                    {
                        // DebugLogger.LogDebug($"Skipping item, is weapon.");
                        continue;
                    }

                    player.AddItem(item);
                }
            }

            if ((loadout.InfiniteAmmo != AmmoMode.None && !flags.HasFlag(LoadoutFlags.IgnoreInfiniteAmmo)) || flags.HasFlag(LoadoutFlags.ForceInfiniteAmmo) || flags.HasFlag(LoadoutFlags.ForceEndlessClip))
            {
                player.GiveInfiniteAmmo(loadout.InfiniteAmmo == AmmoMode.EndlessClip || flags.HasFlag(LoadoutFlags.ForceEndlessClip) ? AmmoMode.EndlessClip : AmmoMode.InfiniteAmmo);
            }
            if(loadout.Health != 0 && !flags.HasFlag(LoadoutFlags.IgnoreHealth))
                player.Health = loadout.Health;
            if (loadout.Health == -1 && !flags.HasFlag(LoadoutFlags.IgnoreGodMode))
            {
                player.IsGodModeEnabled = true;
            }

            if (loadout.ArtificialHealth is not null && loadout.ArtificialHealth.MaxAmount > 0 && !flags.HasFlag(LoadoutFlags.IgnoreAHP))
            {
                //base.Owner.playerStats.GetModule<StaminaStat>().ModifyAmount(1f); 
                // player.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(); - decay???
                // player.ReferenceHub.playerStats.GetModule<HumeShieldStat>().TryGetHsModule(); - cant do this anymore :(

                loadout.ArtificialHealth.ApplyToPlayer(player);
                //player.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(amount: loadout.ArtificialHealth.InitialAmount, limit: loadout.ArtificialHealth.MaxAmount, decay: loadout., efficacy: 100f, sustain: 5f, persistant: true);
                // player.ArtificialHealth = loadout.ArtificialHealth;
            }
            if (!flags.HasFlag(LoadoutFlags.IgnoreStamina) && loadout.Stamina != 0)
            {
                player.ReferenceHub.playerStats.GetModule<StaminaStat>().ModifyAmount(loadout.Stamina); 
                //player.StaminaRemaining = loadout.Stamina;
            }
            if(loadout.Size != Vector3.one && !flags.HasFlag(LoadoutFlags.IgnoreSize))
                player.SetPlayerScale(loadout.Size);
            if (loadout.Effects is not null && loadout.Effects.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreEffects))
            {
                foreach (var effect in loadout.Effects)
                {
                    player.EffectsManager.ChangeState(effect.EffectType.ToString(), effect.Intensity, effect.Duration,
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

        public static Dictionary<ushort, bool> ExplodeOnCollisionList = new Dictionary<ushort, bool>();
        public static Dictionary<ushort, RockSettings> RockList = new Dictionary<ushort,RockSettings>();


        public static void MakeRock(this Item rock, RockSettings settings) => MakeRock(rock.Serial, settings);
        public static void MakeRock(this ItemBase rock, RockSettings settings) => MakeRock(rock.ItemSerial, settings);

        public static void MakeRock(ushort serial, RockSettings settings)
        {
            RockList.Add(serial, settings);
        }

        
        /// <summary>
        /// Gets the keycard permissions of a player.
        /// </summary>
        /// <param name="ply">The player to get the permissions of./param>
        /// <param name="inHandOnly">Will only cards that are in hand count. (Bypass / SCP Override as well)</param>
        /// <returns></returns>
        public static KeycardPermissions KeyCardLevel(this Player ply, bool inHandOnly = false)
        {
            KeycardPermissions perms = KeycardPermissions.None;
            
            // Add Current Keycard Only
            if (inHandOnly && ply.CurrentItem is KeycardItem keycard)
                perms = (KeycardPermissions)keycard.Permissions;

            // Add all keycards
            if (!inHandOnly)
            {
                foreach (var item in ply.Items)
                {
                    if (item is not KeycardItem keycardItem)
                        continue;
                    perms.Include((int)keycardItem.Permissions);
                }
            }
            // Add bypass mode
            if (ply.IsBypassEnabled)
                perms |= KeycardPermissions.BypassMode;

            // Add scp override
            if (ply.IsSCP)
                perms |= KeycardPermissions.ScpOverride;
            
            // Add 079 override
            if (ply.Role == RoleTypeId.Scp079)
                perms |= KeycardPermissions.Scp079Override;

            return perms;
        }
        

        /// <summary>
        /// Checks whether a player has a keycard level.
        /// </summary>
        /// <param name="ply">The player to get the permissions of.</param>
        /// <param name="inHandOnly">Will only cards that are in hand count. (Bypass / SCP Override as well)</param>
        /// <returns>True if the player has the level. False if the player does not have the level.</returns>
        public static bool HasKeycardLevel(this Player ply, KeycardPermissions perms, bool inHandOnly = false)
        {
            KeycardPermissions curPerms = ply.KeyCardLevel(inHandOnly);
            return curPerms.HasRequiredFlags(perms);
        }

        public static void ExplodeOnCollision(this Item grenade, bool giveNewGrenadeOnExplosion = false) => ExplodeOnCollision(grenade.Serial, giveNewGrenadeOnExplosion);
        public static void ExplodeOnCollision(this ItemBase grenade, bool giveNewGrenadeOnExplosion = false) => ExplodeOnCollision(grenade.ItemSerial, giveNewGrenadeOnExplosion);
        public static void ExplodeOnCollision(this ushort item, bool giveNewGrenadeOnExplosion = false)
        {
            ExplodeOnCollisionList.Add(item, giveNewGrenadeOnExplosion);
        }
        
        public static ThrowableItem CreateThrowable(ItemType type, Player player = null) => (player != null ? player.ReferenceHub : ReferenceHub.HostHub)
            .inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;
        public static ThrownProjectile SpawnThrowable(
            this ThrowableItem item,
            Vector3 position,
            float fuseTime = -1f,
            Player owner = null,
            bool activate = false
        )
        {
            TimeGrenade grenade = (TimeGrenade) Object.Instantiate(item.Projectile, position, Quaternion.identity);
            if (fuseTime >= 0)
                grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenade.PreviousOwner = new Footprint(owner != null ? owner.ReferenceHub : ReferenceHub.HostHub);
            if (grenade is Scp018Projectile scp018)
                scp018.GetComponent<Rigidbody>().velocity = activate ? new Vector3(Random.value, Random.value, Random.value) : Vector3.zero; // add some force to make the ball bounce
            NetworkServer.Spawn(grenade.gameObject);
            if(activate)
                grenade.ServerActivate();
            return grenade;
        }
        
        public static Dictionary<Player, AmmoMode> InfiniteAmmoList = new Dictionary<Player, AmmoMode>();
        public static void GiveInfiniteAmmo(this Player player, AmmoMode ammoMode)
        {
            if (ammoMode == AmmoMode.None)
            {
                if (InfiniteAmmoList is null || InfiniteAmmoList.Count < 1 || !InfiniteAmmoList.ContainsKey(player))
                    return;
                InfiniteAmmoList.Remove(player);
                return;
            }

            if (InfiniteAmmoList.ContainsKey(player))
            {
                InfiniteAmmoList[player] = ammoMode;
            }
            else
            {
                InfiniteAmmoList.Add(player, ammoMode);
            }
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
            {
                player.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
            }
        }
        public static void GiveEffect(this Player ply, Effect effect) => GiveEffect(ply, effect.EffectType, effect.Intensity,
            effect.Duration, effect.AddDuration);
        public static void GiveEffect(this Player ply, StatusEffect effect, byte intensity, float duration = 0f, bool addDuration = false) =>             
            ply.EffectsManager.ChangeState(effect.ToString(), intensity, duration, addDuration);
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
                player.SetRole(AutoEvent.Singleton.Config.LobbyRole, RoleChangeReason.None);
                player.GiveInfiniteAmmo(AmmoMode.None);
                player.IsGodModeEnabled = false;
                player.SetPlayerScale(new Vector3(1,1,1));
                player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
            }
        }

        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine(AutoEvent.Singleton.Config.MusicDirectoryPath, audioFile);

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
            if (AudioBot == null) return;

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
            hubPlayer.authManager.InstanceMode = CentralAuth.ClientInstanceMode.Unverified;
            // CharacterClassManager.instance
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
            try
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
            catch (Exception e)
            {
                DebugLogger.LogDebug("An error occured at IsExistsMap.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

            return false;
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
