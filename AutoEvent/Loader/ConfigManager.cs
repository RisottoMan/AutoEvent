using System;
using System.Collections.Generic;
using System.IO;
using AutoEvent.Interfaces;

namespace AutoEvent;
public static class ConfigManager
{
    public static void RegisterConfigsAndTranslations()
    {
        string configPath = Path.Combine(AutoEvent.BaseConfigPath, "config.yml");
        string translationPath = Path.Combine(AutoEvent.BaseConfigPath, "translation.yml");

        // Read data from configs
        try
        {
            if (!File.Exists(configPath))
            {
                var configs = new Dictionary<string, EventConfig>();
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    configs.Add(ev.Name, ev.InternalConfig);
                }
            
                Save(configPath, configs);
            }
            else
            {
                var configs = Load<Dictionary<string, EventConfig>>(configPath);
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    if (configs.TryGetValue(ev.Name, out EventConfig config))
                    {
                        ev.InternalConfig = config;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[EventLoader] cannot read from config.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
        
        // Read data from translations
        try
        {
            if (!File.Exists(translationPath))
            {
                var translations = new Dictionary<string, EventTranslation>();
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    translations.Add(ev.Name, ev.InternalTranslation);
                }
            
                //check os lang
                //AutoEvent.Singleton.Config.Language = "english";
            
                Save(translationPath, translations);
            }
            else
            {
                var translations = Load<Dictionary<string, EventTranslation>>(translationPath);
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    if (translations.TryGetValue(ev.Name, out EventTranslation translation))
                    {
                        ev.InternalTranslation = translation;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[EventLoader] cannot read from translation.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
    }

    private static T Load<T>(string path)
    {
        var yaml = File.ReadAllText(path);
        return Exiled.Loader.Loader.Deserializer.Deserialize<T>(yaml);
    }
    
    private static void Save<T>(string path, T data)
    {
        var yaml = Exiled.Loader.Loader.Serializer.Serialize(data);
        File.WriteAllText(path, yaml);
    }
}