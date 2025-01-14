using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;

public class Translations : ICommand, IUsageProvider
{
    public string Command => nameof(Translations);
    public string[] Aliases { get; } = [];
    public string Description => "Translations managemenent";

    public string[] Usage { get; } = [];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("ev.list"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        string arg0 = arguments.At(0).ToLower();
        if (arg0 == "list")
        {
            if (arguments.Count != 1)
            {
                response = "Usage: ev_translations list";
                return false;
            }

            response = "List of translations:\n";
            try
            {
                List<string> displayedValues = new List<string>();
                foreach (KeyValuePair<string, string> pair in ConfigManager.LanguageByCountryCodeDictionary)
                {
                    if (displayedValues.Contains(pair.Value))
                        continue;
                    displayedValues.Add(pair.Value);

                    response += $"{pair.Value} ({pair.Key})\n";
                }
            }
            catch (Exception e)
            {
                response = $"Failed to list translations: {e.Message}";
                return false;
            }

            response += "Use ev_translations load [countryCode] to load a translation.";
            return true;
        }
        if (arg0 == "load")
        {
            if (arguments.Count != 2)
            {
                response = "Usage: ev_translations load [countryCode]";
                return false;
            }

            try
            {
                string countryCode = arguments.At(1).ToUpper();
                if (!ConfigManager.LanguageByCountryCodeDictionary.ContainsKey(countryCode))
                {
                    response = "Invalid country code!";
                    return false;
                }

                // All we need is to create a new translation file, and nuke the old one. This is a hacky way, but this is reusing existing code.
                _ = ConfigManager.LoadTranslationFromAssembly(countryCode);

                // Force translation reload
                ConfigManager.LoadTranslations();
                response = "Translation loaded!";
                return true;
            }
            catch (Exception e)
            {
                response = $"Failed to load translation: {e.Message}";
                return false;
            }
        }

        response = "Translations management:\n" +
                   "ev_translations list - list all translations\n" +
                   "ev_translations load [countryCode] - load translation\n";
        return true;
    }

    // Remember, when these stars align.
    // We will find our sign.
    // Through this orb,
    // Sights of dreams,
    // Shades of Space,
    // Eternal Grace,
    // Sense of Might,
    // Blazing light,
    // These new Worlds.
}