using AutoEvent.Interfaces;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent
{
    public class AutoEvent : Plugin<Config>
    {
        public override string Name => "Auto_Event";
        public override string Author => "Ported to Exiled [by KoToXleB]";
        public override Version Version => new Version(4, 0, 0);
        public static AutoEvent Singleton;
        public static IEvent ActiveEvent = null;
        public override void OnEnabled()
        {
            base.OnEnabled();
            Singleton = this;

            // Проверка на директорию музыки
            if (!Directory.Exists(Path.Combine(Paths.Configs, "Music"))) Directory.CreateDirectory(Path.Combine(Paths.Configs, "Music"));
        }
        public override void OnDisabled()
        {
            Singleton = null;
            base.OnDisabled();
        }
    }
}
