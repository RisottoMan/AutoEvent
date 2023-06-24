using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Events.Versus
{
    public sealed class VersusConfig
    {
        [Description("Enable/Disable healing player when apponent joins to area.")]
        public bool HealApponentWhenSomeoneEnterArea { get; set; } = true;
    }
}
