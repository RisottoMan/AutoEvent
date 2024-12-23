using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mirror;
using UnityEngine;
using PlayerRoles;
using SCPSLAudioApi.AudioCore;
using VoiceChat;
using InventorySystem.Items.Pickups;
using PlayerStatsSystem;
using System.Reflection;
using AutoEvent.API;
using AutoEvent.API.Enums;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using InventorySystem.Configs;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using InventorySystem.Items.Jailbird;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using PlayerRoles.Ragdolls;
using Item = PluginAPI.Core.Items.Item;
using Object = UnityEngine.Object;

namespace AutoEvent;
public static class Extensions
{
    public static bool JailbirdIsInvincible { get; set; } = true;
    public static List<JailbirdItem> InvincibleJailbirds { get; set; } = new List<JailbirdItem>();
    public enum LoadoutCheckMethods
    {
        HasRole,
        HasSomeItems,
        HasAllItems,
    }

    public static bool HasLoadout(this Player ply, List<Loadout> loadouts, LoadoutCheckMethods checkMethod = LoadoutCheckMethods.HasRole)
    {
        switch (checkMethod)
        {
            case LoadoutCheckMethods.HasRole:
                return loadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ply.Role));
            case LoadoutCheckMethods.HasAllItems:
                return loadouts.Any(loadout => loadout.Items.All(item => ply.Items.Select(itm => itm.Type).Contains(item)));
            case LoadoutCheckMethods.HasSomeItems:
                return loadouts.Any(loadout => loadout.Items.Any(item => ply.Items.Select(itm => itm.Type).Contains(item)));
        }

        return false;
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

            player.Role.Set(role, respawnFlags);
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
                    continue;

                player.AddItem(item);
            }
        }

        if ((loadout.InfiniteAmmo != AmmoMode.None && !flags.HasFlag(LoadoutFlags.IgnoreInfiniteAmmo)) || flags.HasFlag(LoadoutFlags.ForceInfiniteAmmo) || flags.HasFlag(LoadoutFlags.ForceEndlessClip))
        {
            player.GiveInfiniteAmmo(AmmoMode.InfiniteAmmo);
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

        if (loadout.Size != Vector3.one && !flags.HasFlag(LoadoutFlags.IgnoreSize))
        {
            player.Scale = loadout.Size;
        }
        
        if (loadout.Effects is not null && loadout.Effects.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreEffects))
        {
            foreach (var effect in loadout.Effects)
            {
                player.EffectsManager.ChangeState(effect.EffectType.ToString(), effect.Intensity, effect.Duration,
                    effect.AddDuration);
            }
        }

    }
    
    public static void SetPlayerAhp(this Player player, float amount, float limit = 75, float decay = 1.2f,
        float efficacy = 0.7f, float sustain = 0, bool persistant = false)
    {
        if (amount > 100) amount = 100;

        player.ReferenceHub.playerStats.GetModule<AhpStat>()
            .ServerAddProcess(amount, limit, decay, efficacy, sustain, persistant);
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
        
        foreach (var ammoLimit in InventoryLimits.Config.AmmoLimitsSync)
        {
            player.SetAmmo((AmmoType)ammoLimit.AmmoType, ammoLimit.Limit);
        }
    }

    public static void TeleportEnd()
    {
        foreach (Player player in Player.List)
        {
            player.Role.Set(AutoEvent.Singleton.Config.LobbyRole);
            player.GiveInfiniteAmmo(AmmoMode.None);
            player.IsGodModeEnabled = false;
            player.Scale = new Vector3(1, 1, 1);
            player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
        }
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
            DebugLogger.LogDebug($"{e}");
        }

        return false;
    }

    public static SchematicObject LoadMap(string nameSchematic, Vector3 pos, Quaternion rot, Vector3 scale, bool isStatic)
    {
        return ObjectSpawner.SpawnSchematic(nameSchematic, pos, rot, scale, isStatic);
    }

    public static void UnLoadMap(SchematicObject scheme)
    {
        scheme.Destroy();
    }

    public static void CleanUpAll()
    {
        foreach (var item in Object.FindObjectsOfType<ItemPickupBase>())
        {
            Object.Destroy(item.gameObject);
        }

        foreach (var ragdoll in Object.FindObjectsOfType<BasicRagdoll>())
        {
            Object.Destroy(ragdoll.gameObject);
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