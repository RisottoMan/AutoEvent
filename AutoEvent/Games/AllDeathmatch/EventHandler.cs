using AutoEvent.API.Enums;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.AllDeathmatch;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnJoined(JoinedEventArgs ev)
    {
        if (!_plugin.TotalKills.ContainsKey(ev.Player))
        {
            _plugin.TotalKills.Add(ev.Player, 0);
        }

        SpawnPlayerAfterDeath(ev.Player);
    }

    public void OnLeft(LeftEventArgs ev)
    {
        if (_plugin.TotalKills.ContainsKey(ev.Player))
        {
            _plugin.TotalKills.Remove(ev.Player);
        }
    }

    public void OnPlayerDying(DyingEventArgs ev)
    {
        ev.IsAllowed = false;
        _plugin.TotalKills[ev.Attacker]++;

        SpawnPlayerAfterDeath(ev.Player);
    }

    private void SpawnPlayerAfterDeath(Player player)
    {
        player.EnableEffect<Flashed>(0.1f);
        player.EnableEffect<SpawnProtected>(.1f);
        player.Heal(500); // Since the player does not die, his hp goes into negative hp, so need to completely heal the player.
        player.ClearItems();
        
        if (!player.IsAlive)
        {
            player.GiveLoadout(_plugin.Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
        }

        if (player.CurrentItem == null)
        {
            player.CurrentItem = player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
        }
        
        player.Position = _plugin.SpawnList.RandomItem().transform.position;
    }
}