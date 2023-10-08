
namespace AutoEvent.Events.EventArgs
{
    public class TeamRespawnArgs
    {
        public TeamRespawnArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
