using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoEvent.Interfaces;

namespace AutoEvent;

public class EventManager
{
    private readonly Dictionary<string, Event> _events = new();
    private Event _currentEvent;
    private bool _isMerLoaded;

    public void RegisterInternalEvents()
    {
        _isMerLoaded = true;
        if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.ToLower().Contains("mapeditorreborn")))
        {
            DebugLogger.LogDebug("MapEditorReborn was not detected. The mini-games may not be available until you install MapEditorReborn.", LogLevel.Error);
            _isMerLoaded = false;
        }

        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Type[] types = callingAssembly.GetTypes();

        foreach (Type type in types)
        {
            try
            {
                if (type.IsAbstract || type.IsEnum || type.IsInterface ||
                    type.GetInterfaces().All(x => x != typeof(IEvent)))
                    continue;

                object evBase = Activator.CreateInstance(type);
                if (evBase is null || evBase is not Event ev)
                    continue;

                if (!ev.AutoLoad)
                    continue;

                if (ev is IEventMap && !_isMerLoaded)
                    continue;

                ev.Id = _events.Count;
                _events.Add(ev.Name, ev);
            }
            catch (MissingMethodException)
            {
            }
            catch (Exception ex)
            {
                DebugLogger.LogDebug($"[EventLoader] cannot register an event.", LogLevel.Error, true);
                DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
            }
        }
    }

    /// <summary>
    /// Gets an event by it's name.
    /// </summary>
    /// <param name="type">The name of the event to search for.</param>
    /// <returns>The first event found with the same name (Case-Insensitive).</returns>
    public Event GetEvent(string type)
    {
        Event ev = null;

        if (int.TryParse(type, out int id))
            return GetEvent(id);

        if (!TryGetEventByCName(type, out ev))
            return _events.Values.FirstOrDefault(ev => ev.Name.ToLower() == type.ToLower());

        return ev;
    }

    public Event CurrentEvent
    {
        get => this._currentEvent;
        set => this._currentEvent = value;
    }

    public List<Event> Events
    {
        get => this._events.Values.ToList();
    }

    public bool IsMerLoaded
    {
        get => this._isMerLoaded;
    }

    private Event GetEvent(int id) => this._events.Values.FirstOrDefault(x => x.Id == id);
    
    private bool TryGetEventByCName(string type, out Event ev)
    {
        return (ev = this._events.Values.FirstOrDefault(x => x.CommandName == type)) != null;
    }
}