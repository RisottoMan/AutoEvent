
namespace AutoEvent.Events.EventArgs
{
    public class SpawnRagdollArgs
    {
        public SpawnRagdollArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
