using MER.Lite.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using UnityEngine;
using Utils.NonAllocLINQ;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.GunGame;
public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
{
    public override string Name { get; set; } = "Gun Game";
    public override string Description { get; set; } = "Cool GunGame on the Shipment map from MW19";
    public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
    public override string CommandName { get; set; } = "gungame";
    public override Version Version { get; set; } = new Version(1, 0, 1);
    [EventConfig]
    public Config Config { get; set; }
    [EventConfigPreset]
    public Config EasyGunsFirst => Preset.EasyGunsFirst;
    [EventTranslation]
    public Translation Translation { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
    {
        MapName = "Shipment", 
        Position = new Vector3(93f, 1020f, -43f)
    };
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
    { 
        SoundName = "ClassicMusic.ogg", 
        Volume = 5
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
    protected override float PostRoundDelay { get; set; } = 10f;
    private EventHandler _eventHandler { get; set; }
    internal List<Vector3> SpawnPoints { get; private set; }
    internal Dictionary<Player, Stats> PlayerStats { get; set; }
    private Player _winner;
    private IEventTag _eventTagImplementation;

    protected override void RegisterEvents()
    {
        PlayerStats = new Dictionary<Player, Stats>();

        _eventHandler = new EventHandler(this);
        EventManager.RegisterEvents(_eventHandler);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
        Players.PlayerDying += _eventHandler.OnPlayerDying;
    }

    protected override void UnregisterEvents()
    {
        EventManager.UnregisterEvents(_eventHandler);
        Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
        Players.DropItem -= _eventHandler.OnDropItem;
        Players.DropAmmo -= _eventHandler.OnDropAmmo;
        Players.PlayerDying -= _eventHandler.OnPlayerDying;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _winner = null;
        SpawnPoints = new List<Vector3>();

        foreach(var point in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Spawnpoint"))
        {
            SpawnPoints.Add(point.transform.position);
        }

        var count = 0;
        foreach (Player player in Player.GetPlayers())
        {
            if (!PlayerStats.ContainsKey(player))
            {
                PlayerStats.Add(player, new Stats(0));
            }

            player.ClearInventory();
            //Extensions.SetRole(player, GunGameRandom.GetRandomRole(), RoleSpawnFlags.None);
            player.GiveLoadout(Config.Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
            player.Position = SpawnPoints.RandomItem();

            count++;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void CountdownFinished()
    {
        foreach (var player in Player.GetPlayers())
        {
            if (player is not null)
            {
                _eventHandler.GetWeaponForPlayer(player);
            }
        }
    }

    protected override bool IsRoundDone()
    {
        // Winner is not null &&
        // Over one player is alive && 
        // Elapsed time is smaller than 10 minutes (+ countdown)
        return !(_winner == null && Enumerable.Count(Player.GetPlayers(), r => r.IsAlive) > 1 && EventTime.TotalSeconds < 600);
    }
        
    protected override void ProcessFrame()
    {
        var leaderStat = PlayerStats.OrderByDescending(r => r.Value.kill).FirstOrDefault();
        List<GunRole> gunsInOrder = Config.Guns.OrderByDescending(x => x.KillsRequired).ToList();
        GunRole leadersWeapon = gunsInOrder.FirstOrDefault(x => leaderStat.Value.kill >= x.KillsRequired);
        foreach (Player pl in Player.GetPlayers())
        {
            PlayerStats.TryGetValue(pl, out Stats stats);
            if (stats.kill >= Config.Guns.OrderByDescending(x => x.KillsRequired).FirstOrDefault()!.KillsRequired)
            {
                _winner = pl;
            }

            int kills = _eventHandler._playerStats[pl].kill;
            ListExtensions.TryGetFirstIndex(gunsInOrder, x => kills >= x.KillsRequired, out int indexOfFirst);

            string nextGun = "";
            int killsNeeded = 0; 
            if (indexOfFirst <= 0)
            {
                killsNeeded = gunsInOrder[0].KillsRequired + 1 - kills;
                nextGun = "Last Weapon";
            }
            else
            {
                /*
                    * 0 Most Kill Gun
                    1 Medium Kill Gun
                    2 Current gun - get current gun - 1 for next
                    3 Lowest kill gun
                */
                killsNeeded = gunsInOrder[indexOfFirst - 1].KillsRequired - kills;
                nextGun = gunsInOrder[indexOfFirst - 1].Item.ToString();
            }
            pl.ClearBroadcasts();
            pl.SendBroadcast(
                Translation.Cycle.Replace("{name}", Name).Replace("{gun}", nextGun )
                    .Replace("{kills}", $"{killsNeeded}").Replace("{leadnick}", leaderStat.Key.Nickname)
                    .Replace("{leadgun}", $"{(leadersWeapon is null ? nextGun : leadersWeapon.Item)}"), 1);
        }
    }

    protected override void OnFinished()
    {
        if (_winner != null)
        {
            Extensions.Broadcast(
                Translation.Winner.Replace("{name}", Name).Replace("{winner}", _winner.Nickname), 10);
        }

        foreach (var player in Player.GetPlayers())
        {
            player.ClearInventory();
        }
    }
}