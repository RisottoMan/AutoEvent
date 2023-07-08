using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Events.Jail
{
    public sealed class JailConfig
    {
        [Description("Enable/Disable fall damage for people.")]
        public List<string> AdminRoles { get; set; } = new List<string>
        {
            "king",
            "event"
        };
    }
}