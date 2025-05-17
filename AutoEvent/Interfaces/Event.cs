using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.API;
using MEC;
using AutoEvent.API.Season;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;

namespace AutoEvent.Interfaces
{
    public abstract class Event : IEvent
    {
#region Abstract Implementations // Tools that have been abstracted into the event class.
    #region Event Information // Information that event authors can specify about the event.
        /// <summary>
        /// The name of the event.
        /// </summary>
        public abstract string Name { get; set; }
        
        /// <summary>
        /// The Id of the event. It is set by AutoEvent.
        /// </summary>
        public int Id { get; internal set; }
        
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
        
        /// <summary>
        /// Use this to force specific settings for friendly fire. 
        /// </summary>
        protected virtual FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Default;
        
        /// <summary>
        /// Use this to force specific settings for friendly fire autoban. 
        /// </summary>
        protected virtual FriendlyFireSettings ForceEnableFriendlyFireAutoban { get; set; } = FriendlyFireSettings.Default;
        
        /// <summary>
        /// Use this to change the settings of the event handlers.
        /// </summary>
        public virtual EventFlags EventHandlerSettings { get; set; } = EventFlags.Default;
        
    #endregion
    #region Event Variables // Variables that the event author has access too, which are abstracted into the event system.
    
        /// <summary>
        /// The coroutine handle of the main event thread which calls ProcessFrame().
        /// </summary>
        protected virtual CoroutineHandle GameCoroutine { get; set; }
        
        /// <summary>
        /// The coroutine handle for the start countdown broadcast. 
        /// </summary>
        protected virtual CoroutineHandle BroadcastCoroutine { get; set; }
        
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
    #region Event Configs // Configs that can change event parameters.
    
        public EventConfig InternalConfig { get; set; }
        public EventTranslation InternalTranslation { get; set; }
    
    #endregion
    #region Event API Methods // Methods that can be used as api calls such as starting music / spawning map. 
    /// <summary>
    /// Starts the defined Audio. Can be used to trigger a late audio cue. <seealso cref="SoundInfo.StartAutomatically">SoundInfo.StartAutomatically</seealso>
    /// </summary>
    /// <param name="checkIfAutomatic">Should the audio abide by <see cref="SoundInfo.StartAutomatically"/></param>
    protected void StartAudio(bool checkIfAutomatic = false)
    {
        DebugLogger.LogDebug($"Starting Audio: " +
                                 $"{(this is IEventSound s ? "true, " + 
                                 $"{(!string.IsNullOrEmpty(s.SoundInfo.SoundName)? "true" : "false")}, " +
                                 $"{(!checkIfAutomatic ? "true" : "false")}, " +
                                 $"{(s.SoundInfo.StartAutomatically ? "true" : "false")}" : "false")}");
        if (this is IEventSound sound && !string.IsNullOrEmpty(sound.SoundInfo.SoundName) && (!checkIfAutomatic || sound.SoundInfo.StartAutomatically))
        {
            sound.SoundInfo.AudioPlayer = Extensions.PlayAudio(sound.SoundInfo.SoundName, sound.SoundInfo.Volume, sound.SoundInfo.Loop);
        }
    }

    /// <summary>
    /// Can be used to stop the running audio.
    /// </summary>
    protected void StopAudio()
    {
        DebugLogger.LogDebug("Stopping Audio");
        if (this is IEventSound sound && !string.IsNullOrEmpty(sound.SoundInfo.SoundName))
        {
            Extensions.StopAudio(sound.SoundInfo.AudioPlayer);
        }
    }

    /// <summary>
    /// Spawns the defined Map. Can be used to trigger a late Map spawn. <seealso cref="MapInfo.SpawnAutomatically">MapInfo.SpawnAutomatically</seealso>
    /// </summary>
    /// <param name="checkIfAutomatic">Should the audio abide by <see cref="MapInfo.SpawnAutomatically"/></param>

    protected void SpawnMap(bool checkIfAutomatic = false)
    {
        DebugLogger.LogDebug($"Spawning Map: " +
                             $"{(this is IEventMap m ? "true, " + 
                             $"{(!string.IsNullOrEmpty(m.MapInfo.MapName)? "true" : "false")}, " +
                             $"{(!checkIfAutomatic ? "true" : "false")}, " +
                             $"{(m.MapInfo.SpawnAutomatically ? "true" : "false")}" : "false")}");
        if (this is IEventMap map && !string.IsNullOrEmpty(map.MapInfo.MapName) && (!checkIfAutomatic || map.MapInfo.SpawnAutomatically))
        {
            map.MapInfo.Map = Extensions.LoadMap(map.MapInfo.MapName, map.MapInfo.Position,  map.MapInfo.MapRotation, map.MapInfo.Scale);
        }
    }

    /// <summary>
    /// Can be used to de-spawn the map.
    /// </summary>
    protected void DeSpawnMap()
    {
        DebugLogger.LogDebug($"DeSpawning Map. {this is IEventMap}", LogLevel.Debug);
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
        DebugLogger.LogDebug($"Starting Event {Name}", LogLevel.Debug);
        OnInternalStart();
    }
    
    /// <summary>
    /// Used to stop the event safely.
    /// </summary>
    public void StopEvent()
    {
        DebugLogger.LogDebug($"Stopping Event {Name}", LogLevel.Debug);
        OnInternalStop();
    }

    #endregion
    #region Event Methods // Methods that event authors can / must utilize that are abstracted into the event system.
        /// <summary>
        /// Base constructor for an event.
        /// </summary>
        public Event(){ }
        
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
        /// Assigns a random map.
        /// </summary>
        /// <param name="conf"></param>
        private void SetRandomMap()
        {
            if (this.InternalConfig.AvailableMaps is null || this.InternalConfig.AvailableMaps.Count == 0)
                return;

            // We get the current style and check the maps by their style
            SeasonFlags seasonFlags = SeasonMethod.GetSeasonStyle().SeasonFlag;
            
            // If there are no seasonal maps, then choose the default maps
            if (this.InternalConfig.AvailableMaps.Count(r => r.SeasonFlag == seasonFlags) == 0)
            {
                seasonFlags = 0;
            }
            
            List<MapChance> maps = new();
            foreach (var map in this.InternalConfig.AvailableMaps)
            {
                if (map.SeasonFlag == seasonFlags)
                {
                    maps.Add(map);
                }
            }
            
            if (this is IEventMap eventMap)
            {
                bool spawnAutomatically = eventMap.MapInfo.SpawnAutomatically;
                if (maps.Count == 1)
                {
                    eventMap.MapInfo = maps[0].Map;
                    eventMap.MapInfo.SpawnAutomatically = spawnAutomatically;
                    goto Message;
                }

                foreach (var mapItem in maps.Where(x => x.Chance <= 0))
                    mapItem.Chance = 1;
            
                float totalChance = maps.Sum(x => x.Chance);
            
                for (int i = 0; i < maps.Count - 1; i++)
                {
                    if (UnityEngine.Random.Range(0, totalChance) <= maps[i].Chance)
                    {
                        eventMap.MapInfo = maps[i].Map;
                        eventMap.MapInfo.SpawnAutomatically = spawnAutomatically;
                        goto Message;
                    }
                }
                eventMap.MapInfo = maps[maps.Count - 1].Map;
                eventMap.MapInfo.SpawnAutomatically = spawnAutomatically;

                Message:
                DebugLogger.LogDebug($"[{this.Name}] Map {eventMap.MapInfo.MapName} selected.", LogLevel.Debug);
            }
        
        }

        /// <summary>
        /// Triggers internal actions to stop an event.
        /// </summary>
        private void OnInternalStop()
        {
            KillLoop = true;
            Timing.KillCoroutines(new CoroutineHandle[] { BroadcastCoroutine });
            Timing.CallDelayed(FrameDelayInSeconds + .1f, () =>
            {
                if (GameCoroutine.IsRunning)
                {
                    Timing.KillCoroutines(new CoroutineHandle[] { GameCoroutine });
                }
                OnInternalCleanup();
            });
            
            try
            {
                OnStop();
            }
            catch (Exception e)
            {

                DebugLogger.LogDebug($"Caught an exception at Event.OnStop().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
            EventStopped?.Invoke(Name);
            
        }
        /// <summary>
        /// Used to trigger plugin events in the right order.
        /// </summary>
        private void OnInternalStart()
        {
            KillLoop = false;
            _cleanupRun = false;
            AutoEvent.EventManager.CurrentEvent = this;
            EventTime = new TimeSpan();
            StartTime = DateTime.UtcNow;

            try
            {
                // todo finish implementation.
                if (this.ForceEnableFriendlyFire == FriendlyFireSettings.Enable)
                {
                    FriendlyFireSystem.EnableFriendlyFire(); // this.ForceEnableFriendlyFireAutoban == FriendlyFireSettings.Enable);
                }

                if (this.ForceEnableFriendlyFire == FriendlyFireSettings.Disable)
                {
                    FriendlyFireSystem.DisableFriendlyFire();
                }

                if (this.ForceEnableFriendlyFireAutoban == FriendlyFireSettings.Enable)
                {
                    FriendlyFireSystem.EnableFriendlyFireDetector();
                }
                
                if (this.ForceEnableFriendlyFireAutoban == FriendlyFireSettings.Disable)
                {
                    FriendlyFireSystem.DisableFriendlyFireDetector();
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Could not modify friendly fire / ff autoban settings.", LogLevel.Error, true);
                DebugLogger.LogDebug($"{e}");
            }
            
            SetRandomMap();
            SpawnMap(true);
            
            try
            {
                RegisterEvents();
            }
            catch (Exception e)
            {
            
                DebugLogger.LogDebug($"Caught an exception at Event.RegisterEvents().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);

            }

            try
            {
                OnStart();
            }
            catch (Exception e)
            {
                
                DebugLogger.LogDebug($"Caught an exception at Event.OnStart().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
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
            BroadcastCoroutine = Timing.RunCoroutine(BroadcastStartCountdown(), "Broadcast Coroutine");
            yield return Timing.WaitUntilDone(BroadcastCoroutine);
            if (KillLoop)
            {
                yield break;
            }
            try
            {
                CountdownFinished();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event.CountdownFinished().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
            GameCoroutine = Timing.RunCoroutine(RunGameCoroutine(), "Event Coroutine");
            yield return Timing.WaitUntilDone(GameCoroutine);
            if (KillLoop)
            {
                yield break;
            }
            try
            {
                OnFinished();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event.OnFinished().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
            var handle = Timing.CallDelayed(PostRoundDelay, () =>
            {
                if (!_cleanupRun)
                {
                    OnInternalCleanup();
                }
            });
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
                    DebugLogger.LogDebug($"Caught an exception at Event.ProcessFrame().", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"{e}", LogLevel.Error, true);
                }

                EventTime += TimeSpan.FromSeconds(FrameDelayInSeconds);
                yield return Timing.WaitForSeconds(this.FrameDelayInSeconds);
            }
            yield break;
        }

        /// <summary>
        /// Used to prevent double cleanups.
        /// </summary>
        private bool _cleanupRun = false;
        
        /// <summary>
        /// The internal method used to trigger cleanup for maps, ragdolls, items, sounds, and teleporting players to the spawn room.
        /// </summary>
        private void OnInternalCleanup()
        {
            _cleanupRun = true;
            try
            {
                UnregisterEvents();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event.OnUnregisterEvents().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

            try
            {
                FriendlyFireSystem.RestoreFriendlyFire();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Friendly Fire was not able to be restored. Please ensure it is disabled. PLAYERS MAY BE AUTO-BANNED ACCIDENTALLY OR MAY NOT BE BANNED FOR FF.", LogLevel.Error, true);
                DebugLogger.LogDebug($"{e}");
            }

            try
            {
                DeSpawnMap();
                StopAudio();
                Extensions.CleanUpAll();
                Extensions.TeleportEnd();
                
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug("Caught an exception at Event.OnInternalCleanup().GeneralCleanup().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

            try
            {
                OnCleanup();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event.OnCleanup().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

            try
            {
                CleanupFinished?.Invoke(Name);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event.CleanupFinished.Invoke().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
            
            AutoEvent.EventManager.CurrentEvent = null;
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

public abstract class Event<TConfig, TTranslation> : Event
    where TConfig : EventConfig, new()
    where TTranslation : EventTranslation, new()
{
    public Event()
    {
        InternalConfig = new TConfig();
        InternalTranslation = new TTranslation();
    }
    public TConfig Config => (TConfig)InternalConfig;
    public TTranslation Translation => (TTranslation)InternalTranslation;
}
