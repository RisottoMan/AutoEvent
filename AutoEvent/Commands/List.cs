using AutoEvent.API.Season;
using AutoEvent.Interfaces;
using CommandSystem;
using System;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using UnityEngine;

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
        string color = style.PrimaryColor;
        
        if (style.Text != null)
            builder.AppendLine(style.Text);

        var eventList = AutoEvent.EventManager.Events.Where(ev => ev is not IHiddenCommand).OrderBy(x => x.Name).ToList();
        foreach (IEvent ev in eventList)
        {
            if (!isConsole)
                builder.AppendLine($"<color={color}>{ev.Name}</color> [<color=yellow>{ev.CommandName}</color>]: <color=white>{ev.Description}</color>");
            else
                builder.AppendLine($"{ev.Name} [{ev.CommandName}]: {ev.Description}");
        }

        if (!AutoEvent.EventManager.IsMerLoaded)
        {
            builder.AppendLine("\n<i><color=red>MapEditorReborn is not loaded. There are only those mini-games that don't run maps.</color></i>");
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