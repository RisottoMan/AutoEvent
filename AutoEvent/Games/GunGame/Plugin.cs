using MEC;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace AutoEvent.Games.GunGame;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Gun Game";
    public override string Description { get; set; } = "Cool GunGame on the Shipment map from MW19";
    public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
    public override string CommandName { get; set; } = "gungame";
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Shipment", 
        Position = new Vector3(0, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "ClassicMusic.ogg", 
        Volume = 5
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
    private EventHandler _eventHandler { get; set; }
    internal List<Vector3> SpawnPoints { get; private set; }
    internal Dictionary<Player, Stats> PlayerStats { get; set; }
    private Player _winner;

    protected override void RegisterEvents()
    {
        PlayerStats = new Dictionary<Player, Stats>();

        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.Joined += _eventHandler.OnJoined;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.Joined -= _eventHandler.OnJoined;
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
        foreach (Player player in Player.List)
        {
            if (!PlayerStats.ContainsKey(player))
            {
                PlayerStats.Add(player, new Stats(0));
            }

            player.ClearInventory();
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
        foreach (var player in Player.List.Where(r => r.IsAlive))
        {
            _eventHandler.GetWeaponForPlayer(player);
        }
    }

    protected override bool IsRoundDone()
    {
        // Winner is not null &&
        // Over one player is alive && 
        // Elapsed time is smaller than 10 minutes (+ countdown)
        return !(_winner == null && Enumerable.Count(Player.List, r => r.IsAlive) > 1 && EventTime.TotalSeconds < 600);
    }
        
    protected override void ProcessFrame()
    {
        var leaderStat = PlayerStats.OrderByDescending(r => r.Value.kill).FirstOrDefault();
        List<GunRole> gunsInOrder = Config.Guns.OrderByDescending(x => x.KillsRequired).ToList();
        GunRole leadersWeapon = gunsInOrder.FirstOrDefault(x => leaderStat.Value.kill >= x.KillsRequired);
        foreach (Player pl in Player.List)
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
            pl.Broadcast(1, Translation.Cycle.
                Replace("{name}", Name).
                Replace("{gun}", nextGun).
                Replace("{kills}", $"{killsNeeded}").Replace("{leadnick}", leaderStat.Key.Nickname).
                Replace("{leadgun}", $"{(leadersWeapon is null ? nextGun : leadersWeapon.Item)}"));
        }
    }

    protected override void OnFinished()
    {
        if (_winner != null)
        {
            string text = Translation.Winner.
                Replace("{name}", Name).
                Replace("{winner}", _winner.Nickname);
            Extensions.Broadcast(text, 10);
        }
        else
        {
            string text = Translation.Winner.
                Replace("{name}", Name).
                Replace("{winner}", Player.List.First(r => r.IsAlive).Nickname);
            Extensions.Broadcast(text, 10);
        }
        
        foreach (var player in Player.List)
        {
            player.ClearInventory();
        }
    }
}