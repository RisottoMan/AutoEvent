using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.Lava;
public class EventHandler
{
    private Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.DamageHandler.Type is DamageType.Falldown)
        {
            ev.IsAllowed = false;
        }
        
        if (ev.Attacker != null && ev.Player != null)
        {
            ev.Attacker.ShowHitMarker();
            ev.Amount = 10;
        }
    }
}