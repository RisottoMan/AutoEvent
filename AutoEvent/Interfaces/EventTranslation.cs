using System.ComponentModel;

namespace AutoEvent.Interfaces;
public abstract class EventTranslation : IEventTranslation
{
    public EventTranslation()
    {
        
    }

    public abstract string Name { get; set; }
    public abstract string Description { get; set; }
    public abstract string CommandName { get; set; }
    [Description("DO NOT CHANGE THIS. IT WILL BREAK THINGS. AutoEvent will automatically manage these settings.")]
    public virtual string Version { get; set; }
    public virtual string Language { get; set; } = "EN";
}