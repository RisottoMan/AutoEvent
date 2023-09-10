using AutoEvent.API.Attributes;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MEC;
using UnityEngine;

namespace AutoEvent.Interfaces
{
    public abstract class Event : IEvent
    {
#region Static Implementations // Static tools for registering and viewing events.
        /// <summary>
        /// A list of all registered events including external events and <see cref="IInternalEvent"/> events.
        /// </summary>
        public static List<Event> Events { get; set; } = new List<Event>();


        /// <summary>
        /// Registers all of the <see cref="IInternalEvent"/> Events.
        /// </summary>
        internal static void RegisterInternalEvents()
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
                    try
                    {
                        ev.InstantiateEvent();
                    }
                    catch (Exception)
                    {
                        Log.Warning($"[EventLoader] {ev.Name} encountered an error while registering.");
                    }
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

        /// <summary>
        /// Gets an event by it's name.
        /// </summary>
        /// <param name="type">The name of the event to search for.</param>
        /// <returns>The first event found with the same name (Case-Insensitive).</returns>
        public static Event GetEvent(string type)
        {
            Event ev = null;

            if (int.TryParse(type, out int id))
                return GetEvent(id);

            if (!TryGetEventByCName(type, out ev)) 
                return Events.FirstOrDefault(ev => ev.Name.ToLower() == type.ToLower());

            return ev;
        }

        /// <summary>
        /// Gets an event by it's ID.
        /// </summary>
        /// <param name="id">The ID of the event to search for.</param>
        /// <returns>The first event found with the same ID.</returns>
        public static Event GetEvent(int id) => Events.FirstOrDefault(x => x.Id == id);
        private static bool TryGetEventByCName(string type, out Event ev)
        {
            return (ev = Events.FirstOrDefault(x => x.CommandName == type)) != null;
        }
#endregion
#region Abstract Implementations // Tools that have been abstracted into the event class.
    #region Event Information // Information that event authors can specify about the event.
        /// <summary>
        /// The name of the event.
        /// </summary>
        public abstract string Name { get; set; }
        
        /// <summary>
        /// The Id of the event. It is set by AutoEvent.
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// A description of the event.
        /// </summary>
        public abstract string Description { get; set; }
        
        /// <summary>
        /// The name of the author of the event.
        /// </summary>
        public abstract string Author { get; set; }

        /// <summary>
        /// The name of the map that is used to run the map via command.
        /// </summary> 
        public abstract string CommandName { get; set; }
    #endregion
    #region Event Settings // Settings that event authors can define to modify the abstracted implementations
        /// <summary>
        /// How long to wait after the round finishes, before the cleanup begins. Default is 10 seconds.
        /// </summary>
        protected virtual float PostRoundDelay { get; set; } = 10f;

        /// <summary>
        /// Does the plugin utilize Exiled in any way. This is used to prevent type load exceptions, if exiled isn't present.
        /// </summary>
        /// 
        public virtual bool UsesExiled { get; protected set; } = false;
        
        /// <summary>
        /// If using NwApi or Exiled as the base plugin, set this to false, and manually add your plugin to Event.Events (List[Events]).
        /// This prevents double-loading your plugin assembly.
        /// </summary>
        public virtual bool AutoLoad { get; protected set; } = true;

        /// <summary>
        /// Used to safely kill the while loop, without have to forcible kill the coroutine. <seealso cref="OnStop"/>
        /// </summary>
        protected virtual bool KillLoop { get; set; } = false;
        
        /// <summary>
        /// How many seconds the event waits after each ProcessFrame().
        /// </summary>
        protected virtual float FrameDelayInSeconds { get; set; } = 1f;
    #endregion
    #region Event Variables // Variables that the event author has access too, which are abstracted into the event system.
        
        
        /// <summary>
        /// The coroutine handle of the main event thread which calls ProcessFrame().
        /// </summary>
        protected virtual CoroutineHandle GameCoroutine { get; set; }
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// The DateTime (UTC) that the plugin started at. 
        /// </summary>
        public virtual DateTime StartTime { get; protected set; }
        
        /// <summary>
        /// The elapsed time since the plugin started.
        /// </summary>
        public virtual TimeSpan EventTime { get; protected set; }

    #endregion
    #region Event API Methods // Methods that can be used as api calls such as starting music / spawning map. 
    /// <summary>
    /// Starts the defined Audio. Can be used to trigger a late audio cue. <seealso cref="SoundInfo.StartAutomatically">SoundInfo.StartAutomatically</seealso>
    /// </summary>
    /// <param name="checkIfAutomatic">Should the audio abide by <see cref="SoundInfo.StartAutomatically"/></param>
    protected void StartAudio(bool checkIfAutomatic = false)
    {
        if (this is IEventSound sound && !string.IsNullOrEmpty(sound.SoundInfo.SoundName) &&
            (!checkIfAutomatic || sound.SoundInfo.StartAutomatically))
        {
            // play sound
            Extensions.PlayAudio(
                sound.SoundInfo.SoundName,
                sound.SoundInfo.Volume,
                sound.SoundInfo.Loop,
                Name);
        }
    }

    /// <summary>
    /// Can be used to stop the running audio.
    /// </summary>
    protected void StopAudio()
    {
        Extensions.StopAudio();
    }

    /// <summary>
    /// Spawns the defined Map. Can be used to trigger a late Map spawn. <seealso cref="MapInfo.SpawnAutomatically">MapInfo.SpawnAutomatically</seealso>
    /// </summary>
    /// <param name="checkIfAutomatic">Should the audio abide by <see cref="MapInfo.SpawnAutomatically"/></param>

    protected void SpawnMap(bool checkIfAutomatic = false)
    {
        if (this is IEventMap map && !string.IsNullOrEmpty(map.MapInfo.MapName) &&
            (!checkIfAutomatic || map.MapInfo.SpawnAutomatically))
        {
            // load map
            map.MapInfo.Map = Extensions.LoadMap(
                map.MapInfo.MapName, 
                map.MapInfo.Position, 
                map.MapInfo.Rotation, 
                map.MapInfo.Scale);
        }
    }

    /// <summary>
    /// Can be used to de-spawn the map.
    /// </summary>
    protected void DeSpawnMap()
    {
        if (this is IEventMap eventMap)
        {
            Extensions.UnLoadMap(eventMap.MapInfo.Map);
        }
    }
    
    /// <summary>
    /// Used to start the event safely.
    /// </summary>
    public void StartEvent()
    {
        OnInternalStart();
    }
    
    /// <summary>
    /// Used to stop the event safely.
    /// </summary>
    public void StopEvent()
    {
        OnInternalStop();
    }

    #endregion
    #region Event Methods // Methods that event authors can / must utilize that are abstracted into the event system.
    /// <summary>
        /// The method that is called when the event is registered. Should be used instead of a constructor to prevent type load exceptions.
        /// </summary>
        public virtual void InstantiateEvent() { }
        
        /// <summary>
        /// Called when the event is started.
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Used to register events for plugins.
        /// </summary>
        protected virtual void RegisterEvents() { }
        
        /// <summary>
        /// Called after start in a coroutine. Can be used as a countdown coroutine.
        /// </summary>
        protected virtual IEnumerator<float> BroadcastStartCountdown()
        {
            yield break;
        }
        
        /// <summary>
        /// Called after <see cref="BroadcastStartCountdown"/> is finished. Can be used to remove walls, or give players items.
        /// </summary>
        protected virtual void CountdownFinished() {  }
        
        /// <summary>
        /// Used to determine whether the event should end or not. 
        /// </summary>
        /// <returns>True if the round is finished. False if the round should continue running.</returns>
        protected abstract bool IsRoundDone();
        
        /// <summary>
        /// Called once a second.
        /// </summary>
        protected virtual void ProcessFrame() { }
        
        /// <summary>
        /// Called when the event is finished. If the event is stopped via <see cref="OnStop"/>, this won't be called, as the event never truly finishes properly.
        /// </summary>
        protected abstract void OnFinished();
        
        /// <summary>
        /// Called if the event is forcibly stopped. If this is called, <see cref="OnFinished"/> won't be called.
        /// </summary>
        protected virtual void OnStop() { }
        
        /// <summary>
        /// Used to unregister events for plugins.
        /// </summary>
        protected virtual void UnregisterEvents() { }
        
        /// <summary>
        /// The overridable class for after and event is finished / stopped and cleanup is occuring.
        /// </summary>
        protected virtual void OnCleanup() { }
    #endregion
    #region Internal Event Methods // Methods that are for the internal use by the event system to call or modify other abstracted properties or methods.
        /// <summary>
        /// Triggers internal actions to stop an event.
        /// </summary>
        private void OnInternalStop()
        {
            Timing.CallDelayed(FrameDelayInSeconds + .1f, () =>
            {
                if (GameCoroutine.IsRunning)
                {
                    Timing.KillCoroutines(new CoroutineHandle[] { GameCoroutine });
                }
            });
            
            try
            {
                OnStop();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.OnStop().");
                Log.Debug($"{e}");
            }
            EventStopped?.Invoke(Name);
            OnInternalCleanup();
        }
        /// <summary>
        /// Used to trigger plugin events in the right order.
        /// </summary>
        private void OnInternalStart()
        {
            AutoEvent.ActiveEvent = this;
            EventTime = new TimeSpan();
            StartTime = DateTime.UtcNow;
            
            SpawnMap(true);
            try
            {
                RegisterEvents();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.OnRegisterEvents().");
                Log.Debug($"{e}");
            }

            try
            {
                OnStart();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.OnStart().");
                Log.Debug($"{e}");
            }
            EventStarted?.Invoke(Name);
            StartAudio(true);
            Timing.RunCoroutine(RunTimingCoroutine(), "TimingCoroutine");
        }
        
        /// <summary>
        /// Used to prevent blocking the main game thread while triggering other coroutines.
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> RunTimingCoroutine()
        {
            var broadcastCoroutine = Timing.RunCoroutine(BroadcastStartCountdown(), "Broadcast Coroutine");
            yield return Timing.WaitUntilDone(broadcastCoroutine);
            try
            {
                CountdownFinished();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.BroadcastStartText().");
                Log.Debug($"{e}");
            }
            GameCoroutine = Timing.RunCoroutine(RunGameCoroutine(), "Event Coroutine");
            yield return Timing.WaitUntilDone(GameCoroutine);
            try
            {
                OnFinished();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.OnFinished().");
                Log.Debug($"{e}");
            }

            var handle = Timing.CallDelayed(PostRoundDelay, () => OnInternalCleanup());
            yield return Timing.WaitUntilDone(handle);
        }
        
        /// <summary>
        /// The coroutine that is called for processing frames. We recommend avoiding overrides to this, since this may mess with other logic.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator<float> RunGameCoroutine()
        {            
            while (!IsRoundDone())
            {
                if (KillLoop)
                {
                    yield break;
                }
                try
                {
                    ProcessFrame();                
                }
                catch (Exception e)
                {
                    Log.Warning($"Caught an exception at Event.ProcessFrame().");
                    Log.Debug($"{e}");
                }

                EventTime += TimeSpan.FromSeconds(FrameDelayInSeconds);
                yield return Timing.WaitForSeconds(this.FrameDelayInSeconds);
            }
            yield break;
        }
        

        /// <summary>
        /// The internal method used to trigger cleanup for maps, ragdolls, items, sounds, and teleporting players to the spawn room.
        /// </summary>
        private void OnInternalCleanup()
        {
            try
            {
                UnregisterEvents();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.OnUnregisterEvents().");
                Log.Debug($"{e}");
            }
            DeSpawnMap();

            StopAudio();
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            try
            {
                OnCleanup();
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception at Event.OnCleanup().");
                Log.Debug($"{e}");
            }

            // StartTime = null;
            // EventTime = null;
            CleanupFinished?.Invoke(Name);
            AutoEvent.ActiveEvent = null;

        }
    #endregion
    #region Event Events // These events are triggered internally and can be used by an event manager to detect when certain stages are complete.
        public delegate void EventStoppedHandler(string eventName);
        public delegate void CleanupFinishedHandler(string eventName);
        public delegate void EventStartedHandler(string eventName);

        /// <summary>
        /// Called when the event start is triggered.
        /// </summary>
        public virtual event EventStartedHandler EventStarted;
        
        /// <summary>
        /// Called when the event cleanup is finished. The event is completely finished and disposed of once this is called. 
        /// </summary>
        public virtual event CleanupFinishedHandler CleanupFinished;
        
        /// <summary>
        /// Called when the event is stopped. When the event is stopped, OnFinished() won't be called, but OnCleanup() will be called.
        /// </summary>
        public virtual event EventStoppedHandler EventStopped;
    #endregion
#endregion
    }
}
