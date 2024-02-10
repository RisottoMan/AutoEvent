using System;
using System.IO;
using YamlDotNet.Core;
using Version = System.Version;

namespace AutoEvent.Interfaces;

[AttributeUsage(AttributeTargets.Property)]
public class EventTranslationAttribute : Attribute
{
    public EventTranslationAttribute()
    {
        
    }
    
    public virtual object Load(string folderPath, Type type, bool isCorrectVersion)
    {
        string configPath = Path.Combine(folderPath, "Translation.yml");
        object conf = null;
        try
        {
            if (File.Exists(configPath))
            {
                conf = Configs.Serialization.Deserializer.Deserialize(File.ReadAllText(configPath), type);
            }

            if (conf is not null and EventTranslation translation)
            {
                if (isCorrectVersion is true)
                {
                    _isLoaded = true;
                    return conf;
                }
                else DebugLogger.LogDebug($"The translation version and the plugin version are not equal. It will be deleted and remade.");
            }
            else
            {
                DebugLogger.LogDebug("Translation was not serialized into an event translation. It will be deleted and remade.");
            }
        }
        catch (YamlException e)
        {
            DebugLogger.LogDebug("Caught a bad translation.");
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Caught an exception loading a translation.", LogLevel.Warn, true);
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

        CreateNewTranslation(ref conf, type, configPath, version);
        _isLoaded = true;
        return conf;
    }

    private void CreateNewTranslation(ref object conf, Type type, string configPath, Version version)
    {
        conf = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
        if (conf is null)
        {
            DebugLogger.LogDebug("Translation is null.", LogLevel.Debug);
        }

        File.WriteAllText(configPath, Configs.Serialization.Serializer.Serialize(conf));
        _isLoaded = true;
    }

    public bool IsLoaded => _isLoaded;

    protected bool _isLoaded = false;
}