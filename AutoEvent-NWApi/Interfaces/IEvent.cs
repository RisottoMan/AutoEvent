namespace AutoEvent.Interfaces
{
    public interface IEvent
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string CommandName { get; }
        void StartEvent(); 
        void StopEvent();
    }
}
