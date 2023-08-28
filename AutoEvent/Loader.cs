using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.Commands.Reload;

namespace AutoEvent;

public class Loader
{
    /// <summary>
    /// Overrides the Exiled check for the version of the plugin that is exiled exclusive.
    /// </summary>
    private const bool IsExiledPlugin = true;

    /// <summary>
    /// If enabled, a debug log is output everytime a plugin is loaded. Not necessary for players.
    /// </summary>
    private const bool LogAllPluginsOnRegister = false;
    
    /// <summary>
    /// Debug logging only
    /// </summary>
    private const bool Debug = false;
    
    /// <summary>
    /// Checks to see if exiled is present on this server.
    /// </summary>
    /// <returns></returns>
    private static bool isExiledPresent()
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
        foreach (string assemblyPath in Directory.GetFiles(Path.Combine(Paths.Configs, "Events"), "*.dll"))
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
                Log.Warn("[ExternalEventLoader] Could not load a plugin. Check to make sure it does not require exiled or that you have exiled installed.");
                if (Debug)
                {  
                    Log.Debug(e.ToString());
                }
            }
            catch (Exception e)
            {
                if (Debug)
                {  
                    Log.Debug(e.ToString());
                }
                Log.Warn($"[ExternalEventLoader] Could not load a plugin due to an error.");
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
                        if (eventPlugin.UsesExiled && !isExiledPresent())
                        {
                            Log.Warn(
                                $"[ExternalEventLoader] Cannot register plugin {eventPlugin.Name} because it requires exiled to work. Exiled has not loaded yet, or is not present at all.");
                            continue;
                        }

                        AssemblyInformationalVersionAttribute attribute =
                            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                        if (LogAllPluginsOnRegister || Debug)
                        {
                            Log.Info(
                                $"[ExternalEventLoader] Loaded Event {eventPlugin.Name} by {(eventPlugin.Author is not null ? $"{eventPlugin.Author}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}");
                        }

                        Events.Add(eventPlugin);
                    }
                    catch (Exception e)
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
            Log.Error($"[ExternalEventLoader] Error while loading an assembly at {path}! {exception}");
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
                        if (Debug)
                        {
                            Log.Debug(
                                $"[ExternalEventLoader] \"{type.FullName}\" is an interface or abstract class, skipping.");
                        }


                        continue;
                    }

                    if (!IsDerivedFromPlugin(type))
                    {
                        if (Debug)
                        {
                            Log.Debug(
                                $"[ExternalEventLoader] \"{type.FullName}\" does not inherit from Event, skipping.");
                        }

                        continue;
                    }

                    if (Debug)
                    {
                        Log.Debug($"[ExternalEventLoader] Loading type {type.FullName}");
                    }

                    Event plugin = null;

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor is not null)
                    {
                        if (Debug)
                        {
                            Log.Debug("[ExternalEventLoader] Public default constructor found, creating instance...");
                        }

                        plugin = constructor.Invoke(null) as Event;
                    }
                    else
                    {
                        if (Debug)
                        {
                            Log.Debug(
                                $"[ExternalEventLoader] Constructor wasn't found, searching for a property with the {type.FullName} type...");
                        }

                        object value = Array
                            .Find(
                                type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                                property => property.PropertyType == type)?.GetValue(null);

                        if (value is not null)
                            plugin = value as Event;
                    }

                    if (plugin is null)
                    {
                        Log.Error(
                            $"[ExternalEventLoader] {type.FullName} is a valid event, but it cannot be instantiated! It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                        continue;
                    }

                    if (Debug)
                    {
                        Log.Debug($"[ExternalEventLoader] Instantiated type {type.FullName}");
                    }

                    eventsFound.Add(plugin);
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    Log.Warn("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.");

                    if (Debug)
                    {

                        Log.Error(
                            $"[ExternalEventLoader] Error while initializing event {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

                        foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
                        {
                            Log.Error($"[ExternalEventLoader] {loaderException}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Warn("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.");
                    if (Debug)
                    {
                        Log.Error(
                            $"[ExternalEventLoader] Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}");
                    }
                }
            }
            return eventsFound;
        }
        
        catch (ReflectionTypeLoadException reflectionTypeLoadException)
        {
            Log.Warn("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.");

            if (Debug)
            {

                Log.Error(
                    $"[ExternalEventLoader] Error while initializing event {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

                foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    Log.Error($"[ExternalEventLoader] {loaderException}");
                }
            }
        }
        catch (Exception exception)
        {
            Log.Warn("[ExternalEventLoader] An external event has failed to load! Ensure that you have Exiled installed, or that all of the plugins don't require Exiled.");
            if (Debug)
            {
                Log.Error(
                    $"[ExternalEventLoader] Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}");
            }
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
                if (Debug)
                {
                    Log.Debug($"[ExternalEventLoader] Generic type {genericTypeDef}");
                }

                if (genericTypeDef == typeof(Event))
                    return true;
            }
            else if (Debug)
            {
                Log.Debug($"[ExternalEventLoader] Not Generic Type {type?.Name}.");
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
                if (Debug)
                {
                    Log.Debug($"[ExternalEventLoader] Attempting to load embedded resources for {target.FullName}");
                }

                string[] resourceNames = target.GetManifestResourceNames();

                foreach (string name in resourceNames)
                {
                    if (Debug)
                    {
                        Log.Debug($"[ExternalEventLoader] Found resource {name}");
                    }

                    if (name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        using MemoryStream stream = new();

                        // Log.Debug($"[ExternalEventLoader] Loading resource {name}");

                        Stream dataStream = target.GetManifestResourceStream(name);

                        if (dataStream == null)
                        {
                            Log.Error($"[ExternalEventLoader] Unable to resolve resource {name} Stream was null");
                            continue;
                        }

                        dataStream.CopyTo(stream);

                        Dependencies.Add(Assembly.Load(stream.ToArray()));

                        if (Debug)
                        {
                            Log.Debug($"[ExternalEventLoader] Loaded resource {name}");
                        }
                    }
                    else if (name.EndsWith(".dll.compressed", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream dataStream = target.GetManifestResourceStream(name);

                        if (dataStream == null)
                        {
                            Log.Error($"[ExternalEventLoader] Unable to resolve resource {name} Stream was null");
                            continue;
                        }

                        using DeflateStream stream = new(dataStream, CompressionMode.Decompress);
                        using MemoryStream memStream = new();

                        if (Debug)
                        {
                            Log.Debug($"[ExternalEventLoader] Loading resource {name}");
                        }

                        stream.CopyTo(memStream);

                        Dependencies.Add(Assembly.Load(memStream.ToArray()));

                        if (Debug)
                        {
                            Log.Debug($"[ExternalEventLoader] Loaded resource {name}");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error($"[ExternalEventLoader] Failed to load embedded resources from {target.FullName}: {exception}");
            }
        }
}