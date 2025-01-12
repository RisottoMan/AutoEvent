using Exiled.Events.EventArgs.Player;
using DamageType = Exiled.API.Enums.DamageType;

namespace AutoEvent.Games.Knives;
public class EventHandler
{
    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.DamageHandler.Type == DamageType.Falldown)
        {
            ev.IsAllowed = false;
        }
    }
}