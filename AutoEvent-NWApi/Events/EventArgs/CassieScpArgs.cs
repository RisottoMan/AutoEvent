
namespace AutoEvent.Events.EventArgs
{
    public class CassieScpArgs
    {
        public CassieScpArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
}
