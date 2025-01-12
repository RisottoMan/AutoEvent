using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.GunGame;
public class EventHandler
{
    Plugin _plugin;

    internal Dictionary<Player, Stats> _playerStats
    {
        get => _plugin.PlayerStats;
        set => _plugin.PlayerStats = value;
    }
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
        _playerStats = plugin.PlayerStats;
    }

    public void OnJoined(JoinedEventArgs ev)
    {
        if (!_playerStats.ContainsKey(ev.Player))
        {
            _playerStats.Add(ev.Player, new Stats(0));
        }

        ev.Player.GiveLoadout(_plugin.Config.Loadouts, LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
        ev.Player.Position = _plugin.SpawnPoints.RandomItem();
        GetWeaponForPlayer(ev.Player);
    }

    public void OnDying(DyingEventArgs ev)
    {
        ev.IsAllowed = false;

        if (_playerStats is null)
        {
            _playerStats = new Dictionary<Player, Stats>();
        }
        if (ev.Attacker != null)
        {
            if (!_playerStats.TryGetValue(ev.Attacker, out Stats statsAttacker))
            {
                _playerStats.Add(ev.Attacker, new Stats(1));
                GetWeaponForPlayer(ev.Attacker, true);
            }
            else
            {
                statsAttacker.kill++;
                GetWeaponForPlayer(ev.Attacker, true);
            }
        }

        if (ev.Player != null)
        {
            ev.Player.Position = _plugin.SpawnPoints.RandomItem();
            GetWeaponForPlayer(ev.Player, true);
        }
    }

    public void GetWeaponForPlayer(Player player, bool isHeal = false)
    {
        if (player is null)
            return;
        
        if (_plugin.Config.Guns is null)
        {
            Config conf = new Config();
            _plugin.Config.Guns = conf.Guns;
        }

        if (_playerStats is null)
        {
            _playerStats = new Dictionary<Player, Stats>();
        }
        
        if (!_playerStats.ContainsKey(player))
        {
            _playerStats.Add(player, new Stats(0));
        }

        if (_playerStats[player] is null)
        {
            _playerStats[player] = new Stats(0);
        }

        var itemType = _plugin.Config.Guns.OrderByDescending(y => y.KillsRequired)
            .FirstOrDefault(x => _playerStats[player].kill >= x.KillsRequired)!.Item;
        
        if (itemType is ItemType.None)
        {
            DebugLogger.LogDebug("GetWeapon - Gun by level is null");
            itemType = ItemType.GunCOM15;
        }
        
        DebugLogger.LogDebug($"Getting player {player.Nickname} weapon.");
        player.EnableEffect<SpawnProtected>(.1f);
        player.Heal(500); // Since the player does not die, his hp goes into negative hp, so need to completely heal the player.
        player.ClearItems();
        
        if (player.CurrentItem == null)
        {
            player.CurrentItem = player.AddItem(itemType);
        }
    }
}