using AutoEvent.API.Enums;
using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
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
        player.EnableEffect<SpawnProtected>(.15f);
        player.Heal(100);
        player.ClearInventory();
        player.Position = _plugin.SpawnList.RandomItem().transform.position;
        
        if (!player.IsAlive)
        {
            player.GiveLoadout(_plugin.Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
        }

        var item = player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
        Timing.CallDelayed(.1f, () =>
        {
            if (item != null)
            {
                player.CurrentItem = item;
            }
        });
    }
}