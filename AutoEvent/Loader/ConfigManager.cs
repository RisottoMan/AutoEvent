using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoEvent.Interfaces;
using Exiled.API.Extensions;
using Exiled.Loader;
using PluginAPI.Core;

namespace AutoEvent;
public static class ConfigManager
{
    private static string _configPath { get; } = Path.Combine(AutoEvent.BaseConfigPath, "configs.yml");
    private static string _translationPath { get; } = Path.Combine(AutoEvent.BaseConfigPath, "translation.yml");
    public static void LoadConfigsAndTranslations()
    {
        LoadConfigs();
        LoadTranslations();
    }

    private static void LoadConfigs()
    {
        try
        {
            var configs = new Dictionary<string, object>();
            
            if (!File.Exists(_configPath))
            {
                foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
                {
                    configs.Add(ev.Name, ev.InternalConfig);
                }
                
                // Save the config file
                File.WriteAllText(_configPath, Loader.Serializer.Serialize(configs));
            }
            else
            {
                configs = Loader.Deserializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(_configPath));
            }
            
            // Move translations to each mini-games
            foreach (var ev in AutoEvent.EventManager.Events)
            {
                if (configs is null)
                    continue;
                
                if (!configs.TryGetValue(ev.Name, out object rawDeserializedConfig))
                {
                    DebugLogger.LogDebug($"[ConfigManager] {ev.Name} doesn't have configs");
                    continue;
                }
                
                EventConfig translation = (EventConfig)Loader.Deserializer.Deserialize(Loader.Serializer.Serialize(rawDeserializedConfig), ev.InternalConfig.GetType());
                ev.InternalConfig.CopyProperties(translation);
            }
            
            DebugLogger.LogDebug($"[ConfigManager] The configs of the mini-games are loaded.");
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] cannot read from the config.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
    }
    
    private static void LoadTranslations()
    { 
        try
        {
            var translations = new Dictionary<string, object>();
            
            // If the translation file is not found, then create a new one.
            if (!File.Exists(_translationPath))
            {
                string countryCode = new WebClient().DownloadString($"http://ipinfo.io/{Server.ServerIpAddress}/country").Trim();
                string systemLanguage = new RegionInfo(countryCode).EnglishName.ToLower();
                DebugLogger.LogDebug($"[ConfigManager] The translation.yml file was not found. Creating a new translation for {systemLanguage} language...");
                translations = LoadTranslationFromAssembly(systemLanguage);
            }
            // Otherwise, check language of the translation with the language of the config.
            else
            {
                translations = Loader.Deserializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(_translationPath));
            }
            
            // Move translations to each mini-games
            foreach (var ev in AutoEvent.EventManager.Events)
            {
                if (translations is null)
                    continue;
                
                if (!translations.TryGetValue(ev.Name, out object rawDeserializedTranslation))
                {
                    DebugLogger.LogDebug($"[ConfigManager] {ev.Name} doesn't have translations");
                    continue;
                }
                
                EventTranslation translation = (EventTranslation)Loader.Deserializer.Deserialize(Loader.Serializer.Serialize(rawDeserializedTranslation), ev.InternalTranslation.GetType());
                ev.InternalTranslation.CopyProperties(translation);

                if (ev.Name is not null)
                    ev.Name = translation.Name;
                
                if (ev.Description is not null)
                    ev.Description = translation.Description;
                
                if (ev.CommandName is not null)
                    ev.CommandName = translation.CommandName;
            }
            
            DebugLogger.LogDebug($"[ConfigManager] The translations of the mini-games are loaded.");
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] Cannot read from the translation.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
    }

    private static Dictionary<string, object> LoadTranslationFromAssembly(string language)
    {
        Dictionary<string, object> translations;
        
        // Try to get a translation from an assembly
        if (!TryGetTranslationFromAssembly(language, _translationPath, out translations))
        {
            // Otherwise, create default translations from all mini-games.
            translations = new Dictionary<string, object>();
            
            foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
            {
                ev.InternalTranslation.Name = ev.Name;
                ev.InternalTranslation.Description = ev.Description;
                ev.InternalTranslation.CommandName = ev.CommandName;
                
                translations.Add(ev.Name, ev.InternalTranslation);
            }

            // Save the translation file
            File.WriteAllText(_translationPath, Loader.Serializer.Serialize(translations));
        }
        
        return translations;
    }

    private static bool TryGetTranslationFromAssembly<T>(string language, string path, out T translationFile)
    {
        if (language == "english")
        {
            translationFile = default;
            return false;
        }
        
        string resourceName = $"AutoEvent.Translations.{language}.yml";
        
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    DebugLogger.LogDebug($"[ConfigManager] The language '{language}' was not found in the assembly.", LogLevel.Error);
                    translationFile = default;
                    return false;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string yaml = reader.ReadToEnd(); 
                    translationFile = Loader.Deserializer.Deserialize<T>(yaml);
                    
                    // Save the translation file
                    File.WriteAllText(path, yaml);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] The language '{language}' cannot load from the assembly.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
        
        translationFile = default;
        return false;
    }
}