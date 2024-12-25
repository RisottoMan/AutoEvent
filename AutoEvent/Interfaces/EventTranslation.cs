namespace AutoEvent.Interfaces;
public abstract class EventTranslation : IEventTranslation
{
    public EventTranslation() { }

    public abstract string Name { get; set; }
    public abstract string Description { get; set; }
    public abstract string CommandName { get; set; }
}