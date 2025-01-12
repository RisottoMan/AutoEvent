using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.MusicalChairs;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        // The players will not die from the explosion
        if (ev.DamageHandler.Type is DamageType.Explosion)
        {
            ev.DamageHandler.Damage = 0;
        }
    }

    public void OnDied(DiedEventArgs ev)
    {
        // Remove the dead player from the dictionary
        if (_plugin.PlayerDict.ContainsKey(ev.Player))
        {
            _plugin.PlayerDict.Remove(ev.Player);
        }
        
        // If the player is dead, then remove the last platform
        int playerCount = Player.List.Count(r => r.IsAlive);
        if (playerCount > 0)
        {
            _plugin.Platforms = Functions.RearrangePlatforms(playerCount, _plugin.Platforms, _plugin.MapInfo.Position);
        }
    }
    
    public void OnLeft(LeftEventArgs ev)
    {
        // Remove the left player from the dictionary
        if (_plugin.PlayerDict.ContainsKey(ev.Player))
        {
            _plugin.PlayerDict.Remove(ev.Player);
        }
    }
}
