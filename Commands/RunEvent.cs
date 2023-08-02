using AutoEvent.Interfaces;
using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;

namespace AutoEvent.Commands
{
    internal class RunEvent : ICommand
    {
        public string Command => "run";
        public string Description => "Run the event, takes on 1 argument - the command name of the event.";
        public string[] Aliases => null;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("ev.run"))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }

            if (AutoEvent.ActiveEvent != null)
            {
                response = $"<color=red>The mini-game {AutoEvent.ActiveEvent.Name} is already running!</color>";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Only 1 argument is needed - the command name of the event!";
                return false;
            }

            Event ev = Event.GetEvent(arguments.At(0));
            if (ev == null)
            {
                response = $"<color=red>The mini-game {arguments.At(0)} is not found.</color>";
                return false;
            }

            if (ev.MapName != null)
            if (!Extensions.IsExistsMap(ev.MapName))
            {
                response = $"<color=red>You need a map {ev.MapName} to run a mini-game.</color>";
                return false;
            }

            Round.IsLocked = true;

            if (!Round.IsStarted)
            {
                Round.Start();

                Timing.CallDelayed(2f, () => {

                    foreach (Player player in Player.List)
                    {
                        player.ClearInventory();
                    }

                    ev.OnStart();
                    AutoEvent.ActiveEvent = ev;
                });
            }
            else
            {
                ev.OnStart();
                AutoEvent.ActiveEvent = ev;
            }

            AutoEvent.CountOfPlayedGames++;
            response = $"<color=green>The mini-game {ev.Name} has started!</color>";
            return false;
        }
    }
}
