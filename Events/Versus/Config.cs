using System.ComponentModel;

namespace AutoEvent.Events.Versus
{
    public sealed class VersusConfig
    {
        [Description("Enable/Disable healing player when apponent joins to area.")]
        public bool HealApponentWhenSomeoneEnterArea { get; set; } = false;
    }
}
