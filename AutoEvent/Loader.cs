using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using AutoEvent.Interfaces;
using PluginAPI.Helpers;
using PluginAPI.Core;

namespace AutoEvent;

public class Loader
{
    /// <summary>
    /// Overrides the Exiled check for the version of the plugin that is exiled exclusive.
    /// </summary>
#if EXILED
    public const bool IsExiledPlugin = true;
#else
    public const bool IsExiledPlugin = false;
#endif    
    /// <summary>
    /// If enabled, a debug log is output everytime a plugin is loaded. Not necessary for players.
    /// </summary>
    private const bool LogAllPluginsOnRegister = false;

    /// <summary>
    /// Debug logging only
    /// </summary>
    private static bool Debug => AutoEvent.Debug;
    
    /// <summary>
    /// Checks to see if exiled is present on this server.
    /// </summary>
    /// <returns></returns>
    internal static bool isExiledPresent()
    {
        if (IsExiledPlugin)
        {
            return true;
        }
        foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (ass.GetName().Name == "Exiled.Loader")
            {
                return true;
            }
        }
        return false;

    }   
    /// <summary>
    /// Gets plugin dependencies.
    /// </summary>
    public static List<Assembly> Dependencies { get; } = new();
    
    /// <summary>
    /// A list of additional events found.
    /// </summary>
    public static List<Event> Events = new List<Event>();

    /// <summary>
    /// Loads all plugins.
    /// </summary>
    public static void LoadEvents()
    {
        Dictionary<Assembly, string> locations = new Dictionary<Assembly, string>();
#if !EXILED
        string filepath = AutoEvent.Singleton.Config.ExternalEventsDirectoryPath;
#else
        string filepath = Path.Combine(AutoEvent.BaseConfigPath, "Events");
#endif
        foreach (string assemblyPath in Directory.GetFiles(filepath, "*.dll"))
        {
            try
            {

                Assembly assembly = LoadAssembly(assemblyPath);

                if (assembly is null)
                    continue;

                locations[assembly] = assemblyPath;
            }
            catch (TargetInvocationException e)
            {
                
                DebugLogger.LogDebug("[ExternalEventLoader] Could not load a plugin. Check to make sure it does not require exiled or that you have exiled installed.", LogLevel.Warn, true);
                DebugLogger.LogDebug(e.ToString(),LogLevel.Debug);
            }
            catch (Exception e)
            {
                
                DebugLogger.LogDebug($"[ExternalEventLoader] Could not load a plugin due to an error.", LogLevel.Warn, true);
                DebugLogger.LogDebug(e.ToString(),LogLevel.Debug);
            }
        }

        foreach (Assembly assembly in locations.Keys)
        {
            if (locations[assembly].Contains("dependencies"))
                continue;
            try
            {

                List<Event> eventList = CreateEventPlugin(assembly);
                foreach (Event eventPlugin in eventList)
                {
                    try
                    {

                        if (eventPlugin is null)
                            continue;

                        if (!eventPlugin.AutoLoad)
                        {
                            continue;
                        }
                        AssemblyInformationalVersionAttribute attribute =
                            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                        DebugLogger.LogDebug($"[ExternalEventLoader] Loaded Event {eventPlugin.Name} by {(eventPlugin.Author is not null ? $"{eventPlugin.Author}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}", LogLevel.Info, LogAllPluginsOnRegister);

                        try
                        {
                            eventPlugin.VerifyEventInfo();
                            eventPlugin.LoadConfigs();
                            eventPlugin.LoadTranslation();
                            eventPlugin.InstantiateEvent();
                        }
                        catch (Exception e)
                        {
                            DebugLogger.LogDebug($"[EventLoader] {eventPlugin.Name} encountered an error while registering.",LogLevel.Warn, true);
                            DebugLogger.LogDebug(e.ToString(),LogLevel.Debug);
                        }
                        Events.Add(eventPlugin);
                    }
                    catch (Exception)
                    {
                        //unused
                    }
                }
            }
            catch (Exception)
            {
                // unused
            }
        }
    }

    /// <summary>
    /// Loads an assembly.
    /// </summary>
    /// <param name="path">The path to load the assembly from.</param>
    /// <returns>Returns the loaded assembly or <see langword="null"/>.</returns>
    public static Assembly LoadAssembly(string path)
    {
        try
        {
            Assembly assembly = Assembly.Load(File.ReadAllBytes(path));

            ResolveAssemblyEmbeddedResources(assembly);

            return assembly;
        }
        catch (Exception exception)
        {
            DebugLogger.LogDebug($"[ExternalEventLoader] Error while loading an assembly at {path}! {exception}", LogLevel.Warn);
        }

        return null;
    }

    /// <summary>
    /// Create a plugin instance.
    /// </summary>
    /// <param name="assembly">The event assembly.</param>
    /// <returns>Returns the created plugin instance or <see langword="null"/>.</returns>
    public static List<Event> CreateEventPlugin(Assembly assembly)
    {
        List<Event> eventsFound = new List<Event>();
        try
        {
            foreach (Type type in assembly.GetTypes())
            {
                try
                {
                    if (type.IsAbstract || type.IsInterface)
                    {
                        DebugLogger.LogDebug(
                            $"[ExternalEventLoader] \"{type.FullName}\" is an interface or abstract class, skipping.",
                            LogLevel.Debug);

                        continue;
                    }
                    
                    if (!IsDerivedFromPlugin(type))
                    {
                        DebugLogger.LogDebug(
                            $"[ExternalEventLoader] \"{type.FullName}\" does not inherit from Event, skipping.",
                            LogLevel.Debug);

                        continue;
                    }
                    
                    if(type.GetInterface(nameof(IExiledEvent)) is not null && !isExiledPresent())
                    {
                        DebugLogger.LogDebug($"[ExternalEventLoader] Cannot register plugin {type.Name} because it requires exiled to work. Exiled has not loaded yet, or is not present at all.",LogLevel.Warn, true);
                        continue;
                    }

                    DebugLogger.LogDebug($"[ExternalEventLoader] Loading type {type.FullName}", LogLevel.Debug);

                    Event plugin = null;

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor is not null)
                    {

                        DebugLogger.LogDebug("[ExternalEventLoader] Public default constructor found, creating instance...", LogLevel.Debug);

                        plugin = constructor.Invoke(null) as Event;
                    }
                    else
                    {

                        DebugLogger.LogDebug($"[ExternalEventLoader] Constructor wasn't found, searching for a property with the {type.FullName} type...", LogLevel.Debug);
                      

                        object value = Array
                            .Find(
                                type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                                property => property.PropertyType == type)?.GetValue(null);

                        if (value is not null)
                            plugin = value as Event;
                    }

                    if (plugin is null)
                    {
                        DebugLogger.LogDebug($"[ExternalEventLoader] {type.FullName} is a valid event, but it cannot be instantiated! It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!", LogLevel.Error, true);
                        continue;
                    }

                    DebugLogger.LogDebug($"[ExternalEventLoader] Instantiated type {type.FullName}", LogLevel.Debug);

                    eventsFound.Add(plugin);
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    DebugLogger.LogDebug("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"[ExternalEventLoader] Error while initializing event {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}", LogLevel.Debug);

                    foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
                    {
                        DebugLogger.LogDebug($"[ExternalEventLoader] {loaderException}", LogLevel.Warn);
                    }
                }
                catch (Exception exception)
                {
                    DebugLogger.LogDebug("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"[ExternalEventLoader] Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}", LogLevel.Debug);
                }
            }
            return eventsFound;
        }
        catch (ReflectionTypeLoadException reflectionTypeLoadException)
        {
            DebugLogger.LogDebug("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"[ExternalEventLoader] Error while initializing event {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}", LogLevel.Debug);



            foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions) 
            {
                DebugLogger.LogDebug($"[ExternalEventLoader] {loaderException}", LogLevel.Error);
            }
        }
        catch (Exception exception)
        {
            DebugLogger.LogDebug("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"[ExternalEventLoader] Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}", LogLevel.Error);
            
        }

        return eventsFound;
    }
    
    /// <summary>
    /// Indicates that the passed type is derived from the plugin type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns><see langword="true"/> if passed type is derived from <see cref="Event"/>, otherwise <see langword="false"/>.</returns>
    private static bool IsDerivedFromPlugin(Type type)
    {
        while (type is not null)
        {
            type = type.BaseType;

            if (type == typeof(Event))
                return true;
            
            if (type is { IsGenericType: true })
            {
                Type genericTypeDef = type.GetGenericTypeDefinition();
                DebugLogger.LogDebug($"[ExternalEventLoader] Generic type {genericTypeDef}", LogLevel.Debug);


                if (genericTypeDef == typeof(Event))
                    return true;
            }
            else
            {
                DebugLogger.LogDebug($"[ExternalEventLoader] Not Generic Type {type?.Name}.", LogLevel.Debug);
            }
        }

        return false;
    }
    
     /// <summary>
        /// Attempts to load Embedded (compressed) assemblies from specified Assembly.
        /// </summary>
        /// <param name="target">Assembly to check for embedded assemblies.</param>
        private static void ResolveAssemblyEmbeddedResources(Assembly target)
        {
            try
            {
                
                DebugLogger.LogDebug($"[ExternalEventLoader] Attempting to load embedded resources for {target.FullName}", LogLevel.Debug);


                string[] resourceNames = target.GetManifestResourceNames();

                foreach (string name in resourceNames)
                {
                    
                    DebugLogger.LogDebug($"[ExternalEventLoader] Found resource {name}", LogLevel.Debug);

                  
                    if (name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        using MemoryStream stream = new();

                        
                        DebugLogger.LogDebug($"[ExternalEventLoader] Loading resource {name}", LogLevel.Debug);


                        Stream dataStream = target.GetManifestResourceStream(name);

                        if (dataStream == null)
                        {
                            
                            DebugLogger.LogDebug($"[ExternalEventLoader] Unable to resolve resource {name} Stream was null", LogLevel.Error, true);
                            continue;
                        }

                        dataStream.CopyTo(stream);

                        Dependencies.Add(Assembly.Load(stream.ToArray()));

                        
                        
                        DebugLogger.LogDebug($"[ExternalEventLoader] Loaded resource {name}", LogLevel.Debug);
                    }
                    else if (name.EndsWith(".dll.compressed", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream dataStream = target.GetManifestResourceStream(name);

                        if (dataStream == null)
                        {
                            
                            DebugLogger.LogDebug($"[ExternalEventLoader] Unable to resolve resource {name} Stream was null", LogLevel.Error, true);
                            continue;
                        }

                        using DeflateStream stream = new(dataStream, CompressionMode.Decompress);
                        using MemoryStream memStream = new();

                        
                        DebugLogger.LogDebug($"[ExternalEventLoader] Loading resource {name}", LogLevel.Debug);
                
                        stream.CopyTo(memStream);

                        Dependencies.Add(Assembly.Load(memStream.ToArray()));


                        DebugLogger.LogDebug($"[ExternalEventLoader] Loaded resource {name}", LogLevel.Debug);
                    }
                }
            }
            catch (Exception exception)
            {
                
                DebugLogger.LogDebug($"[ExternalEventLoader] Failed to load embedded resources from {target.FullName}: {exception}", LogLevel.Error, true);
            }
        }
}