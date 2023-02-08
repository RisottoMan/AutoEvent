using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AutoEvent
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool IsDisableDonators { get; set; } = true;
        public List<string> DonatorList { get; set; } = new List<string>()
        {
            "donate4",
            "donate3",
            "donate2",
            "donate1"
        };
    }
}
