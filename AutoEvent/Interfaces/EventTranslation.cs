namespace AutoEvent.Interfaces;
public class EventTranslation
{
    public EventTranslation() { }

    public virtual string Name { get; set; }
    public virtual string Description { get; set; }
    public virtual string CommandName { get; set; }
}