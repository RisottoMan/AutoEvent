
namespace AutoEvent.Events.EventArgs
{
    public class DropAmmoArgs
    {
        public DropAmmoArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
