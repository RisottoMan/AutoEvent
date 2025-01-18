using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;

public class Translations : ICommand, IUsageProvider
{
    public string Command => "Language";
    public string[] Aliases { get; } = [];
    public string Description => "Change plugin's language";

    public string[] Usage { get; } = [];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("ev.language"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count == 0)
            goto syntax;

        string arg0 = arguments.At(0).ToLower();
        if (arg0 == "list")
        {
            if (arguments.Count != 1)
            {
                response = "Usage: ev language list";
                return false;
            }

            response = "List of translations:\n";
            try
            {
                foreach (string language in ConfigManager.LanguageByCountryCodeDictionary.Values.Distinct()
                             .ToList())
                {
                    response += $"{language}\n";
                }
            }
            catch (Exception e)
            {
                response = $"Failed to list translations: {e.Message}";
                return false;
            }

            response += "Use ev language load [language] to load a translation.";
            return true;
        }

        if (arg0 == "load")
        {
            if (arguments.Count != 2)
            {
                response = "Usage: ev language load [language]";
                return false;
            }

            try
            {
                string language = arguments.At(1).ToLower();
                /*
                if (language == "english" || language == "default")
                {
                    ConfigManager.GenerateDefaultTranslations();
                    response = "Default translation loaded!\n" +
                               "Server restart is required to apply changes.";
                    return true;
                }*/

                if (!ConfigManager.LanguageByCountryCodeDictionary.ContainsValue(language))
                {
                    response = "Language not found!";
                    return false;
                }

                string countryCode = ConfigManager.LanguageByCountryCodeDictionary
                    .FirstOrDefault(x => x.Value == language).Key;

                _ = ConfigManager.LoadTranslationFromAssembly(countryCode);
                ConfigManager.LoadTranslations();
                response = "Translation loaded!\n" +
                           "Server restart is required to apply changes.";
                return true;
            }
            catch (Exception e)
            {
                response = $"Failed to load translation: {e.Message}";
                return false;
            }
        }

        syntax:
        response = "Translations management:\n" +
                   "ev language list - list all available plugin localisations\n" +
                   "ev language load [language] - set language (restart will be required)\n";
        return true;
    }
}