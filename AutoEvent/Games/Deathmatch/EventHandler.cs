using CustomPlayerEffects;
using PlayerRoles;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

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
        
        ev.Player.EnableEffect<SpawnProtected>(.1f);
        
        if (ev.Player.CurrentItem == null)
        {
            ev.Player.CurrentItem = ev.Player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
        }
        
        ev.Player.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);
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
        ev.Player.EnableEffect<SpawnProtected>(0.1f);
        ev.Player.Heal(500); // Since the player does not die, his hp goes into negative hp, so need to completely heal the player.
        ev.Player.ClearItems();
        
        if (ev.Player.CurrentItem == null)
        {
            ev.Player.CurrentItem = ev.Player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
        }
        
        ev.Player.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);
    }
}