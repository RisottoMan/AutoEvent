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
    private static string _configLanguage { get; set; } = "english";
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
                ConfigFile file = new ConfigFile
                {
                    Configs = new Dictionary<string, EventConfig>(),
                    Language = "english"
                };
                
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    file.Configs.Add(ev.Name, ev.InternalConfig);
                }

                _configLanguage = file.Language;
                Save(_configPath, file);
            }
            else
            {
                var eventConfigs = Load<ConfigFile>(_configPath);
                foreach (var ev in AutoEvent.EventManager.Events)
                {
                    if (eventConfigs.Configs.TryGetValue(ev.Name, out EventConfig config))
                    {
                        ev.InternalConfig = config;
                    }
                }
                
                _configLanguage = eventConfigs.Language;
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
            TranslationFile translationFile = new();
            
            // If the translation file is not found, then create a new one.
            if (!File.Exists(_translationPath))
            {
                DebugLogger.LogDebug($"[ConfigManager] The translation.yml file was not found. Creating a new translation...");
                string systemLanguage = CultureInfo.CurrentCulture.DisplayName.Split(' ')[0].ToLower();
                DebugLogger.LogDebug($"[ConfigManager] The system language is {systemLanguage}");
                translationFile = LoadTranslationFromAssembly(systemLanguage);
            }
            // Otherwise, check language of the translation with the language of the config.
            else
            {
                translationFile = Load<TranslationFile>(_translationPath);
                
                // If the user has changed the language in the config, then create a new one.
                if (translationFile.Language != _configLanguage)
                {
                    DebugLogger.LogDebug($"[ConfigManager] {translationFile.Language} language was not found in the assembly.");
                    File.Delete(_translationPath);
                    translationFile = LoadTranslationFromAssembly(_configLanguage);
                }
            }
            
            // Change language in config
            ConfigFile file = Load<ConfigFile>(_configPath);
            file.Language = translationFile.Language;
            Save(_configPath, file);
            
            // Move translations to each mini-games
            foreach (var ev in AutoEvent.EventManager.Events)
            {
                if (translationFile.Translations.TryGetValue(ev.Name, out EventTranslation translation))
                {
                    ev.InternalTranslation = translation;
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

    private static TranslationFile LoadTranslationFromAssembly(string language)
    {
        TranslationFile file;
        
        // Try to get a translation from an assembly
        if (!TryGetTranslationFromAssembly(language, _translationPath, out file))
        {
            // Otherwise, create default translations from all mini-games.
            file = new TranslationFile { Language = "english" };
            var translations = new Dictionary<string, EventTranslation>();
            
            foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
            {
                translations.Add(ev.Name, ev.InternalTranslation);
            }

            file.Translations = translations;
            
            // Save the translation file
            Save(_translationPath, file);
        }
        
        return file;
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