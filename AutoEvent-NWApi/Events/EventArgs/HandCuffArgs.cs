
namespace AutoEvent.Events.EventArgs
{
    public class HandCuffArgs
    {
        public HandCuffArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
