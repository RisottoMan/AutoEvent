using System;
using System.IO;
using YamlDotNet.Core;
using Version = System.Version;

namespace AutoEvent.Interfaces;

[AttributeUsage(AttributeTargets.Property)]
public class EventConfigAttribute : Attribute
{
    public virtual object Load(string folderPath, string configName, Type type)
    {
        string configPath = Path.Combine(folderPath, configName + ".yml");
        object conf = null;
        try
        {
            if (File.Exists(configPath))
            {
                conf = Configs.Serialization.Deserializer.Deserialize(File.ReadAllText(configPath), type);
            }

            if (conf is not null and EventConfig config)
            {
                _isLoaded = true;
                return conf;
            }
            else DebugLogger.LogDebug("Config was not serialized into an event config. It will be deleted and remade.");
        }
        catch (YamlException e)
        {
            DebugLogger.LogDebug("Caught a bad config.");
            // probably an updated config. We will just replace it.
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Caught an exception loading a config.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }

        if (File.Exists(configPath))
        {
            try
            {
                File.Delete(configPath);
            }
            catch (Exception e) { }
        }

        CreateNewConfig(ref conf, type, configPath);
        _isLoaded = true;
        return conf;
    }

    private void CreateNewConfig(ref object conf, Type type, string configPath)
    {
        conf = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
        if (conf is null)
        {
            DebugLogger.LogDebug("Config is null.");
        }

        File.WriteAllText(configPath, Configs.Serialization.Serializer.Serialize(conf));
        _isLoaded = true;
    }

    public bool IsLoaded => _isLoaded;
    protected bool _isLoaded = false;
}