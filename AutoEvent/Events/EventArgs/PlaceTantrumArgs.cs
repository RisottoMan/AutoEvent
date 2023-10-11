
namespace AutoEvent.Events.EventArgs
{
    public class PlaceTantrumArgs
    {
        public PlaceTantrumArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
