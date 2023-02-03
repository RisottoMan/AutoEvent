using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}
