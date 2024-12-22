using AutoEvent.API.Season;
using AutoEvent.Interfaces;
using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;
internal class List : ICommand
{
    public string Command => nameof(List);
    public string Description => "Shows a list of all the events that can be started";
    public string[] Aliases => [];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ev.list"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        StringBuilder builder = new StringBuilder();
        
        bool isConsole = sender is ServerConsoleSender;
        if (!isConsole)
        {
            builder.AppendLine("\"<color=yellow><b>List of events</b></color>:");
        }
        else
        {
            builder.AppendLine("\"List of events:");
        }

        SeasonStyle style = SeasonMethod.GetSeasonStyle();
        if (style.Text != null)
            builder.AppendLine(style.Text);

        List<Event> eventList = Event.Events.Where(ev => ev is not IHiddenCommand).OrderBy(x => x.Name).ToList();
        foreach (var eventItem in eventList)
        {
            string color = style.PrimaryColor;
            builder.AppendLine($"{(!isConsole ? "<color=white>" : "")}[{(!isConsole ? $"<color={color}>" : "")}==AutoEvent Events=={(!isConsole ? "<color=white>" : "")}]");

            foreach (Event ev in eventItem)
            {
                if (!isConsole)
                    builder.AppendLine($"<color={color}>{ev.Name}</color> [<color=yellow>{ev.CommandName}</color>]: <color=white>{ev.Description}</color>");
                else
                    builder.AppendLine($"{ev.Name} [{ev.CommandName}]: {ev.Description}");
            }
        }

        if (!isConsole)
        {
            builder.AppendLine("\nTo run an event, use the [<i><color=yellow>CommandName</color></i>] like following:");
            builder.AppendLine("Ev Run <i><color=yellow>CommandName</color></i>");
        }
        else
        {
            builder.AppendLine("\nTo run an event, use the [CommandName] like following:");
            builder.AppendLine("Ev Run CommandName");
            
        }

        response = builder.ToString();
        return true;
    }
}