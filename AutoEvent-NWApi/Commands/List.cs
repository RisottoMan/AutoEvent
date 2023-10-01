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
        public string Description => "Shows a list of all the events that can be started.";
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
                        color = "yellow";
                        break;
                    case IExiledEvent:
                        color = "blue";
                        break;
                    default:
                        color = "orange";
                        break;
                }
                if(!IsConsoleCommandSender)
                    builder.AppendLine($"<color={color}>{ev.Name} [<color=white>{ev.CommandName}<color={color}>]: {ev.Description}");
                else
                    builder.AppendLine($"{ev.Name} [{ev.CommandName}]: {ev.Description}");
            }

            if (!IsConsoleCommandSender)
            {
                builder.AppendLine("\n<color=yellow>To run an event, use the [<i><color=white>Command Name<color=yellow></i>] like following:</color>");
                builder.AppendLine("<color=yellow>Ev Run <i>CommandName</i></color>");
            }
            else
            {
                builder.AppendLine("\nTo run an event, use the [Command Name] like following:");
                builder.AppendLine("Ev Run CommandName");
                
            }

            response = builder.ToString();
            return true;
        }

    }
}
