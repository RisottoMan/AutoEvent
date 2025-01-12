using AutoEvent.Interfaces;
using CommandSystem;
using System;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using MEC;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;
internal class Language : ICommand, IUsageProvider
{
    public string Command => nameof(Language);
    public string Description => "Change language of the events, takes on 1 argument - language name";
    public string[] Aliases => ["lang", "language"];
    public string[] Usage => ["name / list"];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ev.lang"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "Only 1 argument is needed - the command name of the event!";
            return false;
        }

        if (arguments.ElementAt(0) == "list")
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Available translations: ");

            foreach (string lang in ConfigManager.ShowListOfLanguages())
            {
                stringBuilder.AppendLine(lang);
            }

            response = stringBuilder.ToString();
            return true;
        }

        if (!ConfigManager.ChangeLanguageByName(arguments.ElementAt(0)))
        {
            response = $"The {arguments.ElementAt(0)} language was not found in the assembly.";
            return false;
        }
        
        response = $"The language {arguments.ElementAt(0)} is loaded for mini-games.";
        return true;
    }
}