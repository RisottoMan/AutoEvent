using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Interfaces;

public class EventTranslation
{
    public EventTranslation()
    {
        
    }

    [Description("Config")]
    public string Name { get; set; }

    [Description("Config")]
    public string Description { get; set; }

    [Description("Config")]
    public string CommandName { get; set; }

    [Description("Config")]
    public string Author { get; set; }

    //[Description("DO NOT CHANGE THIS. IT WILL BREAK THINGS. AutoEvent will automatically manage this setting.")]
    //public virtual string TranslationVersion { get; set; }
}