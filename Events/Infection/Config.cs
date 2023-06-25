using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Events.Infection
{
    public sealed class InfectionConfig
    {
        [Description("Enable/Disable fall damage for people.")]
        public bool FallDamageEnabled { get; set; } = false;
    }
}
