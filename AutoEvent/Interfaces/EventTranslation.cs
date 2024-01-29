using System.ComponentModel;

namespace AutoEvent.Interfaces;

public class EventTranslation
{
    public EventTranslation()
    {

    }

    [Description("DO NOT CHANGE THIS. IT WILL BREAK THINGS. AutoEvent will automatically manage this setting.")]
    public virtual string TranslationVersion { get; set; }
}