
namespace AutoEvent.Events.EventArgs
{
    public class PlaceBloodArgs
    {
        public PlaceBloodArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
