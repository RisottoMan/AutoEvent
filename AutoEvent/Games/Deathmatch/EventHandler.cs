using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.Deathmatch;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnJoined(JoinedEventArgs ev)
    {
        int mtfCount = Player.List.Count(r => r.Role.Team == Team.FoundationForces);
        int chaosCount = Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency);
        if (mtfCount > chaosCount)
        {
            ev.Player.GiveLoadout(_plugin.Config.ChaosLoadouts);
        }
        else
        {
            ev.Player.GiveLoadout(_plugin.Config.NTFLoadouts);
        }
        
        ev.Player.EnableEffect<SpawnProtected>(.15f);
        ev.Player.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);

        Timing.CallDelayed(.1f, () =>
        {
            if (ev.Player.Items.First() != null)
            {
                ev.Player.CurrentItem = ev.Player.Items.First();
            }
        });
    }

    public void OnDying(DyingEventArgs ev)
    {
        ev.IsAllowed = false;

        if (ev.Player.Role.Team == Team.FoundationForces)
        {
            _plugin.ChaosKills++;
        }
        else if (ev.Player.Role.Team == Team.ChaosInsurgency)
        {
            _plugin.MtfKills++;
        }

        ev.Player.EnableEffect<Flashed>(0.1f);
        ev.Player.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);
        ev.Player.EnableEffect<SpawnProtected>(.15f);
        ev.Player.Heal(100);
        ev.Player.ClearInventory();
        
        var item = ev.Player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
        Timing.CallDelayed(.1f, () =>
        {
            if (item != null)
            {
                ev.Player.CurrentItem = item;
            }
        });
    }
}