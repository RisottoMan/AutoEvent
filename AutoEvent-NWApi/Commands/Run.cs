using AutoEvent.Interfaces;
using CommandSystem;
using System;
using MEC;
using PluginAPI.Core;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands
{
    internal class Run : ICommand, IUsageProvider, IPermission
    {
        public string Command => nameof(Run);
        public string Description => "Run the event, takes on 1 argument - the command name of the event.";
        public string[] Aliases => new []{ "start", "play", "begin" };
        public string[] Usage => new string[] { "[Event Name]" };
        public string Permission { get; set; } = "ev.run";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }
            if (AutoEvent.ActiveEvent != null)
            {
                response = $"The mini-game {AutoEvent.ActiveEvent.Name} is already running!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Only 1 argument is needed - the command name of the event!";
                return false;
            }

            Event ev = Event.GetEvent(arguments.At(0));
            if (ev == null)
            {
                response = $"The mini-game {arguments.At(0)} is not found.";
                return false;
            }

            if (!(ev is IEventMap map && !string.IsNullOrEmpty(map.MapInfo.MapName)))
            {
                DebugLogger.LogDebug("No map has been specified for this event!", LogLevel.Warn, true);
            }
            else if (!Extensions.IsExistsMap(map.MapInfo.MapName))
            {
                response = $"You need a map {map.MapInfo.MapName} to run a mini-game.";
                return false;
            }

            Round.IsLocked = true;

            if (!Round.IsRoundStarted)
            {
                Round.Start();

                Timing.CallDelayed(2f, () => {

                    foreach (Player player in Player.GetPlayers())
                    {
                        player.ClearInventory();
                    }

                    ev.StartEvent();
                    AutoEvent.ActiveEvent = ev;
                });
            }
            else
            {
                ev.StartEvent();
                AutoEvent.ActiveEvent = ev;
            }

            response = $"The mini-game {
                ev.Name} has started!";
            return true;
        }

    }
}
