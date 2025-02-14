using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp173;

namespace AutoEvent.Games.Escape;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    
    public void OnAnnoucingScpTermination(AnnouncingScpTerminationEventArgs ev)
    {
        ev.IsAllowed = false;
    }
    
    public void OnJoined(JoinedEventArgs ev)
    {
        ev.Player.GiveLoadout(_plugin.Config.Scp173Loadout);
    }

    public void OnPlacingTantrum(PlacingTantrumEventArgs ev)
    {
        ev.IsAllowed = false;
    }
    
    public void OnUsingBreakneckSpeeds(UsingBreakneckSpeedsEventArgs ev)
    {
        ev.IsAllowed = false;
    }
}