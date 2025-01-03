using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoEvent.Interfaces;

namespace AutoEvent;
public static class ConfigManager
{
    private static string _configPath { get; set; }
    private static string _translationPath { get; set; }
    public static void LoadConfigsAndTranslations()
    {
        _configPath = Path.Combine(AutoEvent.BaseConfigPath, "config.yml");
        _translationPath = Path.Combine(AutoEvent.BaseConfigPath, "translation.yml");

        // Load Configs
        try
        {
            if (!File.Exists(_configPath))
            {
                var configs = new Dictionary<string, EventConfig>();
                
                foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
                {
                    configs.Add(ev.Name, ev.InternalConfig);
                }
                
                Save(_configPath, configs);
            }
            else
            {
                var eventConfigs = Load<Dictionary<string, EventConfig>>(_configPath);
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    if (eventConfigs.TryGetValue(ev.Name, out EventConfig config))
                    {
                        ev.InternalConfig = config;
                    }
                }
            }
            
            DebugLogger.LogDebug($"[ConfigManager] The configs of the mini-games are loaded.");
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] cannot read from the config.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }

        // Load Translations
        try
        {
            var translations = new Dictionary<string, EventTranslation>();
            
            // If the translation file is not found, then create a new one.
            if (!File.Exists(_translationPath))
            {
                DebugLogger.LogDebug($"[ConfigManager] The translation.yml file was not found. Creating a new translation...");
                string systemLanguage = CultureInfo.CurrentCulture.DisplayName.Split(' ')[0].ToLower();
                systemLanguage = "english"; // todo
                DebugLogger.LogDebug($"[ConfigManager] The system language is {systemLanguage}");
                translations = LoadTranslationFromAssembly(systemLanguage);
            }
            // Otherwise, check language of the translation with the language of the config.
            else
            {
                translations = Load<Dictionary<string, EventTranslation>>(_translationPath);
            }
            
            // Move translations to each mini-games
            foreach (var ev in AutoEvent.EventManager.Events)
            {
                if (translations.TryGetValue(ev.Name, out EventTranslation translation))
                {
                    ev.InternalTranslation = translation;
                    
                    ev.Name = translation.Name;
                    ev.Description = translation.Description;
                    ev.CommandName = translation.CommandName;
                }
            }
            
            DebugLogger.LogDebug($"[ConfigManager] The translations of the mini-games are loaded.");
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] Cannot read from the translation.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
    }

    private static Dictionary<string, EventTranslation> LoadTranslationFromAssembly(string language)
    {
        Dictionary<string, EventTranslation> translations;
        
        // Try to get a translation from an assembly
        if (!TryGetTranslationFromAssembly(language, _translationPath, out translations))
        {
            // Otherwise, create default translations from all mini-games.
            translations = new Dictionary<string, EventTranslation>();
            
            foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
            {
                ev.InternalTranslation.Name = ev.Name;
                ev.InternalTranslation.Description = ev.Description;
                ev.InternalTranslation.CommandName = ev.CommandName;
                
                translations.Add(ev.Name, ev.InternalTranslation);
            }

            // Save the translation file
            Save(_translationPath, translations);
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
                    translationFile = Configs.Serialization.Deserializer.Deserialize<T>(yaml);
                    
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
    
    private static T Load<T>(string path)
    {
        string yaml = File.ReadAllText(path);
        return Configs.Serialization.Deserializer.Deserialize<T>(yaml);
    }
    
    private static void Save<T>(string path, T data)
    {
        string yaml = Configs.Serialization.Serializer.Serialize(data);
        File.WriteAllText(path, yaml);
    }
}