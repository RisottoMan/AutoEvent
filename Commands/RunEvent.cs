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
                response = "You do not have permission to use this command";
                return false;
            }

            if (AutoEvent.ActiveEvent != null)
            {
                response = "The mini-game is already running!";
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
                response = "event not found.";
                return false;
            }

            Round.IsLocked = true;

            if (!Round.IsStarted)
            {
                Round.Start();
                Timing.CallDelayed(2f, () => {
                    ev.OnStart();
                    AutoEvent.ActiveEvent = ev;
                });
            }
            else
            {
                ev.OnStart();
                AutoEvent.ActiveEvent = ev;
            }

            response = "Event started !";
            return false;
        }
    }
}
