using AutoEvent.API.Attributes;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoEvent.Interfaces
{
    public abstract class Event : IEvent
    {
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public abstract string Author { get; set; }
        public abstract string MapName { get; set; }
        public abstract string CommandName { get; set; }
        public virtual bool UsesExiled => false;
        /// <summary>
        /// If using NwApi or Exiled as the base plugin, set this to false, and manually add your plugin to Event.Events (List[Events]).
        /// This prevents double-loading your plugin assembly.
        /// </summary>
        public virtual bool AutoLoad => true;
        public virtual void OnStart() => throw new NotImplementedException("cannot start event because OnStart method has not implemented");
        public virtual void OnStop() => throw new NotImplementedException("cannot start event because OnStop method has not implemented");
        public int Id { get; private set; }
        public static List<Event> Events { get; set; } = new List<Event>();

        public static void RegisterEvents()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            Type[] types = callingAssembly.GetTypes();

            foreach (Type type in types)
            {
                try
                {
                    if (type.IsAbstract ||
                        type.IsEnum ||
                        type.IsInterface ||
                        Activator.CreateInstance(type) is not Event ev ||
                        type.GetCustomAttributes(typeof(DisabledFeaturesAttribute), false).Any())
                        continue;

                    ev.Id = Events.Count;
                    Events.Add(ev);

                    Log.Info($"[EventLoader] {ev.Name} has been registered!");
                }
                catch (MissingMethodException) { }
                catch (Exception ex)
                {
                    Log.Error($"[EventLoader] cannot register an event: {ex}");
                }
            }
        }

        public static Event GetEvent(string type)
        {
            Event ev = null;

            if (int.TryParse(type, out int id))
                return GetEvent(id);

            if (!TryGetEventByCName(type, out ev)) 
                return Events.FirstOrDefault(ev => ev.Name == type);

            return ev;
        }

        public static Event GetEvent(int id) => Events.FirstOrDefault(x => x.Id == id);
        private static bool TryGetEventByCName(string type, out Event ev)
        {
            return (ev = Events.FirstOrDefault(x => x.CommandName == type)) != null;
        }
    }
}
