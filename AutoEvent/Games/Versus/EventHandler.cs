using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.Versus;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnDying(DyingEventArgs ev)
    {
        ev.Player.ClearInventory();

        if (ev.Player == _plugin.ClassD)
        {
            _plugin.ClassD = null;
        }

        if (ev.Player == _plugin.Scientist)
        {
            _plugin.Scientist = null;
        }
    }
    public void OnJailbirdCharge(ChargingJailbirdEventArgs ev) => ev.IsAllowed = _plugin.Config.JailbirdCanCharge;
}
