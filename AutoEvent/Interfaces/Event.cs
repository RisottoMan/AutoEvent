using AutoEvent.API.Attributes;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoEvent.API.Enums;
using AutoEvent.API;
using AutoEvent.Configs;
using HarmonyLib;
using MEC;
using UnityEngine;
using YamlDotNet.Core;
using Version = System.Version;
using System.Xml.Linq;
using AutoEvent.API.Season;
using AutoEvent.API.Season.Enum;

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
                        type.IsInterface || type.GetInterfaces().All(x => x != typeof(IEvent)))
                        continue;
                    
                    object evBase = Activator.CreateInstance(type);
                        if(evBase is null || evBase is not Event ev ||
                        type.GetCustomAttributes(typeof(DisabledFeaturesAttribute), false).Any(x => x is not null))
                        continue;

                    if (!ev.AutoLoad)
                        continue;
                    ev.Id = Events.Count;
                    try
                    {
                        ev.VerifyEventInfo();
                        ev.LoadConfigs();
                        ev.LoadTranslation();
                        ev.InstantiateEvent();
                    }
                    catch (Exception e)
                    {
                        DebugLogger.LogDebug($"[EventLoader] {ev.Name} encountered an error while registering.", LogLevel.Warn, true);
                        DebugLogger.LogDebug($"[EventLoader] {e}", LogLevel.Debug);
                    }
                    string confs = "";
                    foreach (var conf in ev.ConfigPresets)
                    {
                        confs += $"{conf.PresetName}, ";
                    }
                    Events.Add(ev);
                    DebugLogger.LogDebug($"[EventLoader] {ev.Name} has been registered. Presets: {(confs + ",").Replace(", ,", "")}", LogLevel.Info); // , true);
                }
                catch (MissingMethodException) { }
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

        public abstract Version Version { get; set; }
    #endregion
    #region Event Settings // Settings that event authors can define to modify the abstracted implementations
        /// <summary>
        /// How long to wait after the round finishes, before the cleanup begins. Default is 10 seconds.
        /// </summary>
        protected virtual float PostRoundDelay { get; set; } = 10f;

        /// <summary>
        /// Obsolete. Use <see cref="IExiledEvent"/> instead.
        /// </summary>
        [Obsolete("This is no longer supported. Inherit IExiledEvent instead.")]
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
        
        /// <summary>
        /// Use this to force specific settings for friendly fire. 
        /// </summary>
        protected virtual FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Default;
        
        /// <summary>
        /// Use this to force specific settings for friendly fire autoban. 
        /// </summary>
        protected virtual FriendlyFireSettings ForceEnableFriendlyFireAutoban { get; set; } = FriendlyFireSettings.Default;
        
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
                                 $"{(s.SoundInfo.StartAutomatically ? "true" : "false")}" : "false")}",
            LogLevel.Debug);
        if (this is IEventSound sound && !string.IsNullOrEmpty(sound.SoundInfo.SoundName) &&
            (!checkIfAutomatic || sound.SoundInfo.StartAutomatically))
        {
            sound.SoundInfo.AudioPlayerBase = Extensions.PlayAudio(
                sound.SoundInfo.SoundName,
                sound.SoundInfo.Volume,
                sound.SoundInfo.Loop);
        }
    }

    /// <summary>
    /// Can be used to stop the running audio.
    /// </summary>
    protected void StopAudio()
    {
        DebugLogger.LogDebug("Stopping Audio", LogLevel.Debug);
        Extensions.StopAudio();
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
                                                         $"{(m.MapInfo.SpawnAutomatically ? "true" : "false")}" : "false")}",
            LogLevel.Debug);
        if (this is IEventMap map && !string.IsNullOrEmpty(map.MapInfo.MapName) &&
            (!checkIfAutomatic || map.MapInfo.SpawnAutomatically))
        {
            // load map
            map.MapInfo.Map = Extensions.LoadMap(
                map.MapInfo.MapName, 
                map.MapInfo.Position, 
                map.MapInfo.MapRotation, 
                map.MapInfo.Scale,
                map.MapInfo.IsStatic);
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
    /// Can be used to get a list of each <see cref="EventConfig"/> defined in the plugin.
    /// </summary>
    /// <returns>Returns a list of the current values of each <see cref="EventConfig"/></returns>
    public List<EventConfig> GetCurrentConfigsValues()
    {
        List<EventConfig> eventConfigs = new List<EventConfig>();
        foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
        {
            var attr = propertyInfo.GetCustomAttribute<EventConfigAttribute>();
            if (attr is null)
                continue;
            object value = propertyInfo.GetValue(this);
            if(value is not EventConfig conf)
                continue;
            eventConfigs.Add(conf);
        }

        return eventConfigs;
    }
    
    public List<EventTranslation> GetCurrentTranslationsValues()
    {
        List<EventTranslation> eventConfigs = new List<EventTranslation>();
        foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
        {
            var attr = propertyInfo.GetCustomAttribute<EventTranslationAttribute>();
            if (attr is null)
                continue;
            object value = propertyInfo.GetValue(this);
            if(value is not EventTranslation conf)
                continue;
            eventConfigs.Add(conf);
        }

        return eventConfigs;
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
        private string CreateConfigFolder()
        {
            string path = GetConfigFolder();
            AutoEvent.CreateDirectoryIfNotExists(path);
            AutoEvent.CreateDirectoryIfNotExists(Path.Combine(path, "Presets"));
            return path;
        }

        private string GetConfigFolder() => Path.Combine(AutoEvent.Singleton.Config.EventConfigsDirectoryPath, this.Name);

        /// <summary>
        /// A list of available config presets. WIP
        /// </summary>
        public List<EventConfig> ConfigPresets { get; set; } = new List<EventConfig>();

        private List<Type> _confTypes { get; set; } = new List<Type>();
    
        /// <summary>
        /// Ensures that information such as the command name is valid.
        /// </summary>
        internal void VerifyEventInfo()
        {
            this.CommandName = CommandName.ToCamelCase(true);
        }
    
        /// <summary>
        /// Validates and loads any configs and presets for the given event.
        /// </summary>
        internal void LoadConfigs()
        {
            if (this.ConfigPresets is not null)
                this.ConfigPresets.Clear();
            else
                this.ConfigPresets = new List<EventConfig>();
        
            int loadedConfigs = 0;
            var path = CreateConfigFolder();
            try
            {
                loadedConfigs = _loadValidConfigs(path);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"[EventLoader] LoadConfigs()->_loadValidConfigs(path) has caught an exception while loading configs for the plugin {Name}", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

            try
            {
                _createPresets(Path.Combine(path, "Presets"));
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"[EventLoader] LoadConfigs()->_createPresets(path) has caught an exception while loading configs for the plugin {Name}", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

            try
            {
                _loadPresets(Path.Combine(path, "Presets"));
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"[EventLoader] LoadConfigs()->_loadPresets(path) has caught an exception while loading configs for the plugin {Name}", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
        }
    
        /// <summary>
        /// Loads any configs.
        /// </summary>
        /// <param name="path">The base event path.</param>
        private int _loadValidConfigs(string path)
        {
            int i = 0;
            foreach (var property in this.GetType().GetProperties())
            {
                var conf = property.GetCustomAttribute<EventConfigAttribute>();
                if (conf is EventConfigPresetAttribute)
                {
                    continue;
                }
                if (conf is null)
                {
                    continue;
                }
            
                DebugLogger.LogDebug($"Config \"{property.Name}\" found for {Name}", LogLevel.Debug);

                object config = conf.Load(path, property.Name, property.PropertyType, this.Version);
                if (config is not EventConfig evConfig)
                {
                    DebugLogger.LogDebug($"Config was found that does not inherit Event Config. It will be skipped.", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"(Event {this.Name}) Config: {property.Name}.", LogLevel.Debug);
                    continue;
                }

                if (ConfigPresets.Count > 0)
                    evConfig.PresetName = $"Default-{ConfigPresets.Count - 1}";
                else
                    evConfig.PresetName = "Default";
                _setRandomMap(evConfig);
                _setRandomSound(evConfig);
            
                property.SetValue(this, config);
                ConfigPresets.Add((EventConfig)config);
                _confTypes.Add(config.GetType());

                i++;
            }

            return i;
        }

        /// <summary>
        /// Assigns a random map.
        /// </summary>
        /// <param name="conf"></param>
        private void _setRandomMap(EventConfig conf)
        {
            if (conf.AvailableMaps is null && conf.AvailableMaps.Count == 0)
                return;

            // We get the current style and check the maps by their style
            SeasonStyle _curSeason = SeasonMethod.GetSeasonStyle();

            List<MapChance> maps = new();
            foreach (var map in conf.AvailableMaps)
            {
                if (map.SeasonFlag is SeasonFlag.None || map.SeasonFlag == _curSeason.SeasonFlag)
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
        /// Assigns a random sound.
        /// </summary>
        /// <param name="conf"></param>
        private void _setRandomSound(EventConfig conf)
        {
            if (this is IEventSound sound && conf.AvailableSounds is not null && conf.AvailableSounds.Count > 0)
            {
                bool startAutomatically = sound.SoundInfo.StartAutomatically;
                if (conf.AvailableSounds.Count == 1)
                {
                    sound.SoundInfo = conf.AvailableSounds[0].Sound;
                    sound.SoundInfo.StartAutomatically = startAutomatically;
                    goto Message;
                }

                foreach (var soundItem in conf.AvailableSounds.Where(x => x.Chance <= 0))
                    soundItem.Chance = 1;
            
                float totalChance = conf.AvailableSounds.Sum(x => x.Chance);
            
                for (int i = 0; i < conf.AvailableSounds.Count - 1; i++)
                {
                    if (UnityEngine.Random.Range(0, totalChance) <= conf.AvailableSounds[i].Chance)
                    {
                        sound.SoundInfo = conf.AvailableSounds[i].Sound;
                        sound.SoundInfo.StartAutomatically = startAutomatically;
                        goto Message;
                    }
                }
                sound.SoundInfo = conf.AvailableSounds[conf.AvailableSounds.Count - 1].Sound;
                sound.SoundInfo.StartAutomatically = startAutomatically;
                Message:
                DebugLogger.LogDebug($"[{this.Name}] Sound {sound.SoundInfo.SoundName} selected.", LogLevel.Debug);
            }
        }
        /// <summary>
        /// Creates a preset.yml file for each preset found.
        /// </summary>
        /// <param name="path">The base event path.</param>
        private void _createPresets(string path)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var conf = property.GetCustomAttribute<EventConfigPresetAttribute>();
                if (conf is null || conf.IsLoaded)
                {
                    continue;
                }
                // DebugLogger.LogDebug($"Embedded Config Preset \"{property.Name}\" found for {Name}", LogLevel.Debug);

                conf.Load(path, property, property.GetValue(this));
            }
        }

        /// <summary>
        /// Loads all config presets to the preset List.
        /// </summary>
        /// <param name="path">The base event path.</param>
        private void _loadPresets(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.yml"))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
            
                object conf = Configs.Serialization.Deserializer.Deserialize(File.ReadAllText(file), _confTypes.FirstOrDefault() ?? typeof(EventConfig));
                if (conf is not EventConfig)
                {
                    DebugLogger.LogDebug("Not Event Config.");
                    continue;
                }
                // DebugLogger.LogDebug($"Config Preset \"{file}\" loaded for {Name}", LogLevel.Debug);
                ((EventConfig)conf).PresetName = fileName;
                ConfigPresets.Add((EventConfig)conf);
                DebugLogger.LogDebug($"Config Preset: {conf.GetType().Name}, BaseType: {conf.GetType().BaseType?.Name}");
            }
        }
    
        /// <summary>
        /// Loads any translations present
        /// </summary>
        internal void LoadTranslation()
        {
            try
            {
                _loadValidTranslations(GetConfigFolder());
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"[EventLoader] LoadTranslation()->_loadValidConfigs(path) has caught an exception while loading configs for the plugin {Name}", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Loads translations.
        /// </summary>
        /// <param name="path">The base event path.</param>
        private void _loadValidTranslations(string path)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var trans = property.GetCustomAttribute<EventTranslationAttribute>();
                if (trans is null)
                {
                    continue;
                }

                DebugLogger.LogDebug($"Translation \"{property.Name}\" found for {Name}", LogLevel.Debug);
                
                object translation = trans.Load(path, property.PropertyType, this.Version);
                if (translation is not EventTranslation evTranslation)
                {
                    DebugLogger.LogDebug($"Translation was found that does not inherit Event Translation. It will be skipped.", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"(Event {this.Name}) Translation: {property.Name}.", LogLevel.Debug);
                    continue;
                }

                property.SetValue(this, evTranslation);

                // Replace all the main strings of the mini-game.
                this.Name = evTranslation.Name;
                this.Description = evTranslation.Description;
                this.CommandName = evTranslation.CommandName;
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
            AutoEvent.ActiveEvent = this;
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
            while (!IsRoundDone() || DebugLogger.AntiEnd)
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
                    DebugLogger.LogDebug($"{e}", LogLevel.Debug);
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

            // StartTime = null;
            // EventTime = null;
            try
            {
                CleanupFinished?.Invoke(Name);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event.CleanupFinished.Invoke().", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }
            AutoEvent.ActiveEvent = null;
            try
            {
                EventConfig conf = this.GetCurrentConfigsValues().FirstOrDefault();
                EventTranslation trans = this.GetCurrentTranslationsValues().FirstOrDefault();
                if (conf is not null)
                {
                    this._setRandomMap(conf); 
                    this._setRandomSound(conf);
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Event._setMap.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

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
