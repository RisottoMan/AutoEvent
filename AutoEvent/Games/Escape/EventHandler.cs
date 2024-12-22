using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp173;
using PlayerRoles;

namespace AutoEvent.Games.Escape;
public class EventHandler
{
    public void OnAnnoucingScpTermination(AnnouncingScpTerminationEventArgs ev)
    {
        ev.IsAllowed = false;
    }
    
    public void OnJoined(JoinedEventArgs ev)
    {
        ev.Player.Role.Set(RoleTypeId.Scp173);
    }

    public void OnPlacingTantrum(PlacingTantrumEventArgs ev)
    {
        ev.IsAllowed = false;
    }
}