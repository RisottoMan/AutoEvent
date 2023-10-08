
namespace AutoEvent.Events.EventArgs
{
    public class PlaceBulletArgs
    {
        public PlaceBulletArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
