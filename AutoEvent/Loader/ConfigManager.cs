using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using AutoEvent.Interfaces;

namespace AutoEvent;
public static class ConfigManager
{
    public static void LoadConfigsAndTranslations()
    {
        string configPath = Path.Combine(AutoEvent.BaseConfigPath, "config.yml");
        string translationPath = Path.Combine(AutoEvent.BaseConfigPath, "translation.yml");

        // Load Configs
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
            
            DebugLogger.LogDebug($"[ConfigManager] The configs of the mini-games are loaded.");
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] cannot read from config.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
        
        // Load Translations
        try
        {
            TranslationFile translationFile = new();
            
            // If the translation file is not found, then create a new one.
            if (!File.Exists(translationPath))
            {
                DebugLogger.LogDebug($"[ConfigManager] The translation.yml file was not found. Creating a new translation...");
                string systemLanguage = CultureInfo.CurrentCulture.DisplayName.ToLower();
                translationFile = LoadTranslationFromAssembly(systemLanguage);
            }
            // Otherwise, check language of the translation with the language of the config.
            else
            {
                string configLanguage = AutoEvent.Singleton.Config.Language;
                translationFile = Load<TranslationFile>(translationPath);
                
                // If the user has changed the language in the config, then create a new one.
                if (translationFile.Language != configLanguage)
                {
                    DebugLogger.LogDebug($"[ConfigManager] {translationFile.Language} language was not found in the assembly.");
                    File.Delete(translationPath);
                    translationFile = LoadTranslationFromAssembly(configLanguage);
                }
            }
            
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
            DebugLogger.LogDebug($"[EventLoader] cannot read from translation.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
    }

    private static TranslationFile LoadTranslationFromAssembly(string language)
    {
        string translationPath = Path.Combine(AutoEvent.BaseConfigPath, "translation.yml");
        var file = new TranslationFile();
        
        // Try to get a translation from an assembly
        if (!TryGetTranslationFromAssembly(language, out file))
        {
            // Otherwise, create default translations from all mini-games.
            file = new TranslationFile { Language = "english" };
            var translations = new Dictionary<string, EventTranslation>();
            
            foreach (var ev in AutoEvent.EventManager.Events)
            {
                translations.Add(ev.Name, ev.InternalTranslation);
            }

            file.Translations = translations;
        }
        
        // Save the translation file
        Save(translationPath, file);
        return file;
    }

    private static bool TryGetTranslationFromAssembly<T>(string language, out T translationFile)
    {
        string resourceName = $"Translations.{language}.yml";
        
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    DebugLogger.LogDebug($"[ConfigManager] {language} language was not found in the assembly.", LogLevel.Error);
                    translationFile = default;
                    return false;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    translationFile = Exiled.Loader.Loader.Deserializer.Deserialize<T>(reader.ReadToEnd());
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            DebugLogger.LogDebug($"[ConfigManager] {language} language cannot load from the assembly.", LogLevel.Error, true);
            DebugLogger.LogDebug($"{ex}", LogLevel.Debug);
        }
        
        translationFile = default;
        return false;
    }
    
    private static T Load<T>(string path)
    {
        string yaml = File.ReadAllText(path);
        return Exiled.Loader.Loader.Deserializer.Deserialize<T>(yaml);
    }
    
    private static void Save<T>(string path, T data)
    {
        string yaml = Exiled.Loader.Loader.Serializer.Serialize(data);
        File.WriteAllText(path, yaml);
    }
}