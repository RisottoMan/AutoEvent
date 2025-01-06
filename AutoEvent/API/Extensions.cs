using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using PlayerRoles;
using InventorySystem.Items.Pickups;
using PlayerStatsSystem;
using AutoEvent.API;
using AutoEvent.API.Enums;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Configs;
using InventorySystem.Items.Jailbird;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using PlayerRoles.Ragdolls;
using Object = UnityEngine.Object;

namespace AutoEvent;
public static class Extensions
{
    public static bool JailbirdIsInvincible { get; set; } = true;
    public static List<JailbirdItem> InvincibleJailbirds { get; set; } = new();
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
                        goto assignRole;
                    }
                }

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
            loadout.ArtificialHealth.ApplyToPlayer(player);
        }
        
        if (!flags.HasFlag(LoadoutFlags.IgnoreStamina) && loadout.Stamina != 0)
        {
            player.ReferenceHub.playerStats.GetModule<StaminaStat>().ModifyAmount(loadout.Stamina);
        }
        else
        {
            player.IsUsingStamina = false;
        }

        if (loadout.Size != Vector3.one && !flags.HasFlag(LoadoutFlags.IgnoreSize))
        {
            player.Scale = loadout.Size;
        }
        
        if (loadout.Effects is not null && loadout.Effects.Count > 0 && !flags.HasFlag(LoadoutFlags.IgnoreEffects))
        {
            foreach (var effect in loadout.Effects)
            {
                player.EnableEffect(effect.Type, effect.Intensity, effect.Duration);
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
        
        foreach (AmmoType ammoType in Enum.GetValues(typeof(AmmoType)))
        {
            if (ammoType == AmmoType.None)
                continue;
            
            player.SetAmmo(ammoType, 100);
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
            player.ClearInventory();
        }
    }

    public static bool IsExistsMER()
    {
        try
        {
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error occured at IsExistsMER.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
        
        return false;
    }
    
    public static bool IsExistsMap(string schematicName)
    {
        try
        {
            if (MapUtils.GetSchematicDataByName(schematicName) is null)
                return false;
            
            return true;
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error occured at IsExistsMap.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
        
        return false;
    }
    
    public static MapObject LoadMap(string schematicName, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        try
        {
            var schematicObject = ObjectSpawner.SpawnSchematic(schematicName, pos, rot, scale, null);

            return new MapObject()
            {
                AttachedBlocks = schematicObject.AttachedBlocks,
                GameObject = schematicObject.gameObject
            };
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error occured at LoadMap.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }

        return null;
    }

    public static void UnLoadMap(MapObject mapObject)
    {
        mapObject.Destroy();
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

    public static void GrenadeSpawn(Vector3 pos, float scale = 1f, float fuseTime = 1f)
    {
        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
        grenade.Scale = new Vector3(scale, scale, scale);
        grenade.FuseTime = fuseTime;
        grenade.SpawnActive(pos);
    }

    public static AudioPlayer PlayAudio(string fileName, byte volume, bool isLoop)
    {
        if (!AudioClipStorage.AudioClips.ContainsKey(fileName))
        {
            string filePath = Path.Combine(AutoEvent.Singleton.Config.MusicDirectoryPath, fileName);
            DebugLogger.LogDebug($"{filePath}");
            if (!AudioClipStorage.LoadClip(filePath, fileName))
            {
                DebugLogger.LogDebug($"[PlayAudio] The music file {fileName} was not found for playback");
                return null;
            }
        }
        
        AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"AutoEvent-Global", onIntialCreation: (p) =>
        {
            Speaker speaker = p.AddSpeaker($"AutoEvent-Main-{fileName}", isSpatial: false, maxDistance: 5000f);
            speaker.Volume = volume * (AutoEvent.Singleton.Config.Volume / 100f);
        });

        audioPlayer.SendSoundGlobally = true;
        audioPlayer.AddClip(fileName, loop: isLoop);
        
        return audioPlayer;
    }
    
    public static void PlayPlayerAudio(AudioPlayer audioPlayer, Player player, string fileName, byte volume)
    {
        if (audioPlayer is null)
        {
            DebugLogger.LogDebug($"[PlayPlayerAudio] The AudioPlayer is null");
        }
        
        if (!AudioClipStorage.AudioClips.ContainsKey(fileName))
        {
            string filePath = Path.Combine(AutoEvent.Singleton.Config.MusicDirectoryPath, fileName);
            DebugLogger.LogDebug($"{filePath}");
            if (!AudioClipStorage.LoadClip(filePath, fileName))
            {
                DebugLogger.LogDebug($"[PlayPlayerAudio] The music file {fileName} was not found for playback");
                return;
            }
        }
        
        audioPlayer.AddClip(fileName);
    }
    
    public static void PauseAudio(AudioPlayer audioPlayer)
    {
        if (audioPlayer is null)
        {
            DebugLogger.LogDebug($"[PauseAudio] The AudioPlayer is null");
        }
        
        int clipId = audioPlayer.ClipsById.Keys.First();
        if (audioPlayer.TryGetClip(clipId, out AudioClipPlayback clip))
        {
            clip.IsPaused = true;
        }
    }
    
    public static void ResumeAudio(AudioPlayer audioPlayer)
    {
        if (audioPlayer is null)
        {
            DebugLogger.LogDebug($"[ResumeAudio] The AudioPlayer is null");
        }
        
        int clipId = audioPlayer.ClipsById.Keys.First();
        if (audioPlayer.TryGetClip(clipId, out AudioClipPlayback clip))
        {
            clip.IsPaused = false;
        }
    }
    
    public static void StopAudio(AudioPlayer audioPlayer)
    {
        if (audioPlayer is null)
        {
            DebugLogger.LogDebug($"[StopAudio] The AudioPlayer is null");
        }
        
        try
        {
            audioPlayer.RemoveAllClips();
            audioPlayer.Destroy();
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error occured at StopAudio.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
    }
}