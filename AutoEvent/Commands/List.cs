using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
using System;
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

            foreach (Event ev in Event.Events)
            {
                string color = "white";
                switch (ev)
                {
                    case IInternalEvent:
                        color = "red";
                        break;
                    case IExiledEvent:
                        color = "blue";
                        break;
                    default:
                        color = "orange";
                        break;
                }
                if (ev is IHidden) continue;
                if(!IsConsoleCommandSender)
                    builder.AppendLine($"<color={color}>{ev.Name}</color> [<color=yellow>{ev.CommandName}</color>]: <color=white>{ev.Description}</color>");
                else
                    builder.AppendLine($"{ev.Name} [{ev.CommandName}]: {ev.Description}");
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
