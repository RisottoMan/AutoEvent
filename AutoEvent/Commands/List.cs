using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands
{
    internal class List : ICommand, IPermission
    {
        public List()
        {
            // Log.Debug("Skipping Registering List Command");
        }
        public string Command => nameof(List);
        public string Description => "Shows a list of all the events that can be started";
        public string[] Aliases => new string[] { };
        public string Permission { get; set; } = "ev.list";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }
            
            StringBuilder builder = new StringBuilder();
            if (!IsConsoleCommandSender)
            {
                builder.AppendLine("\"<color=yellow><b>List of events</b></color>:");
            }
            else
            {
                builder.AppendLine("\"List of events:");
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            Dictionary<string, List<Event>> events = new Dictionary<string, List<Event>>()
            {
                { "Internal Events", new List<Event>() },
                { "External Events", new List<Event>() },
                { "Exiled Events", new List<Event>() },
                //{ "cedmod", new List<Event>() },
                //{ "riptide", new List<Event>() },
            };
            events["Internal Events"] = Event.Events.Where(ev => ev is IInternalEvent).OrderBy(x => x.Name).ToList();
            events["External Events"] = Event.Events.Where(ev => ev is not IInternalEvent && ev is not IExiledEvent).OrderBy(x => x.Name).ToList();
            events["Exiled Events"] = Event.Events.Where(ev => ev is IExiledEvent).OrderBy(x => x.Name).ToList();
            //events["cedmod"] = Event.Events.Where(ev => ev is IInternalEvent).OrderBy(x => x.Name).ToList();
            // events["riptide"] = Event.Events.Where(ev => ev is IInternalEvent).OrderBy(x => x.Name).ToList();
            foreach (KeyValuePair<string, List<Event>> eventlist in events)
            {
                string color = "white";
                switch (eventlist.Key)
                {

                    case "Internal Events":
                        color = "red";
                        builder.AppendLine($"{(!IsConsoleCommandSender ? "<color=white>" : "")}[{(!IsConsoleCommandSender ? $"<color={color}>" : "")}==AutoEvent Events=={(!IsConsoleCommandSender ? "<color=white>" : "")}]");
                        
                        break;
                    case "External Events":
                        color = "blue";
                        builder.AppendLine($"{(!IsConsoleCommandSender ? "<color=white>" : "")}[{(!IsConsoleCommandSender ? $"<color={color}>" : "")}==External Events=={(!IsConsoleCommandSender ? "<color=white>" : "")}]");
                        
                        break;
                    default:
                        color = "orange";
                        
                        
                        builder.AppendLine($"{(!IsConsoleCommandSender ? "<color=white>" : "")}[{(!IsConsoleCommandSender ? $"<color={color}>" : "")}==Exiled Events=={(!IsConsoleCommandSender ? "<color=white>" : "")}]");
                        break;
                }
                
                foreach (Event ev in eventlist.Value)
                {
                    if (ev is IHidden) continue;
                    if (!IsConsoleCommandSender)
                        builder.AppendLine(
                            $"<color={color}>{ev.Name}</color> [<color=yellow>{ev.CommandName}</color>]: <color=white>{ev.Description}</color>");
                    else
                        builder.AppendLine($"{ev.Name} [{ev.CommandName}]: {ev.Description}");
                }
            }

            if (!IsConsoleCommandSender)
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
}
