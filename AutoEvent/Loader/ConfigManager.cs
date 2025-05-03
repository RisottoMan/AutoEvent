using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoEvent.Interfaces;
using Exiled.API.Extensions;
using Exiled.Loader;

namespace AutoEvent;
public static class ConfigManager
{
    internal static string ConfigPath { get; } = Path.Combine(AutoEvent.BaseConfigPath, "configs.yml");

    internal static string TranslationPath { get; } = Path.Combine(AutoEvent.BaseConfigPath, "translation.yml");

    public static void LoadConfigsAndTranslations()
    {
        LoadConfigs();
        LoadTranslations();
    }

    internal static void LoadConfigs()
    {
        try
        {
            var configs = new Dictionary<string, object>();

            if (!File.Exists(ConfigPath))
            {
                foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
                {
                    configs.Add(ev.Name, ev.InternalConfig);
                }

                // Save the config file
                File.WriteAllText(ConfigPath, Loader.Serializer.Serialize(configs));
            }
            else
            {
                configs = Loader.Deserializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(ConfigPath));
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

    internal static void LoadTranslations()
    {
        try
        {
            var translations = new Dictionary<string, object>();

            // If the translation file is not found, then create a new one.
            if (!File.Exists(TranslationPath))
            {
                string countryCode = "EN";
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string url = $"http://ipinfo.io/{Exiled.API.Features.Server.IpAddress}/country";
                        countryCode = client.DownloadString(url).Trim();
                    }
                }
                catch (WebException ex)
                {
                    DebugLogger.LogDebug(input:"Couldn't verify the server country. Providing default translation.");
                }
                DebugLogger.LogDebug($"[ConfigManager] The translation.yml file was not found. Creating a new translation for {countryCode} language...");
                translations = LoadTranslationFromAssembly(countryCode);
            }
            // Otherwise, check language of the translation with the language of the config.
            else
            {
                translations = Loader.Deserializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(TranslationPath));
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

                object? obj = Loader.Deserializer.Deserialize(Loader.Serializer.Serialize(rawDeserializedTranslation),
                    ev.InternalTranslation.GetType());
                if (obj is not EventTranslation translation)
                {
                    DebugLogger.LogDebug($"[ConfigManager] {ev.Name} malformed translation.");
                    continue;
                }

                ev.InternalTranslation.CopyProperties(translation);

                ev.Name = translation.Name;
                ev.Description = translation.Description;
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

    internal static Dictionary<string, object> LoadTranslationFromAssembly(string countryCode)
    {
        Dictionary<string, object> translations;

        // Try to get a translation from an assembly
        if (!TryGetTranslationFromAssembly(countryCode, TranslationPath, out translations))
        {
            translations = GenerateDefaultTranslations();
        }

        return translations;
    }

    internal static Dictionary<string, object> GenerateDefaultTranslations()
    {
        // Otherwise, create default translations from all mini-games.
        Dictionary<string, object> translations = new Dictionary<string, object>();

        foreach (var ev in AutoEvent.EventManager.Events.OrderBy(r => r.Name))
        {
            ev.InternalTranslation.Name = ev.Name;
            ev.InternalTranslation.Description = ev.Description;
            ev.InternalTranslation.CommandName = ev.CommandName;

            translations.Add(ev.Name, ev.InternalTranslation);
        }

        // Save the translation file
        File.WriteAllText(TranslationPath, Loader.Serializer.Serialize(translations));
        return translations;
    }

    private static bool TryGetTranslationFromAssembly<T>(string countryCode, string path, out T translationFile)
    {
        if (!LanguageByCountryCodeDictionary.TryGetValue(countryCode, out string language))
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

    internal static Dictionary<string, string> LanguageByCountryCodeDictionary { get; } = new()
    {
        ["EN"] = "english",
        ["CN"] = "chinese",
        ["FR"] = "french",
        ["DE"] = "german",
        ["NL"] = "german", //sorry :)
        ["IT"] = "italian",
        ["PL"] = "polish",
        ["BR"] = "portuguese",
        ["PT"] = "portuguese",
        ["RU"] = "russian",
        ["KZ"] = "russian",
        ["BY"] = "russian",
        ["UA"] = "russian", //sorry :)
        ["ES"] = "spanish",
        ["TH"] = "thai",
        ["TR"] = "turkish",
    };
}