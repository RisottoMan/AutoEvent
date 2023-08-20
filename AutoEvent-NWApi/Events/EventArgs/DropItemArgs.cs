
namespace AutoEvent.Events.EventArgs
{
    public class DropItemArgs
    {
        public DropItemArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
