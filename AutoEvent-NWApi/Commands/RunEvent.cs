using AutoEvent.Interfaces;
using CommandSystem;
using System;
using MEC;
using PluginAPI.Core;

namespace AutoEvent.Commands
{
    internal class RunEvent : ICommand
    {
        public string Command => "run";
        public string Description => "Run the event, takes on 1 argument - the command name of the event.";
        public string[] Aliases => null;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var config = AutoEvent.Singleton.Config;
            var player = Player.Get(sender);

            if (sender is ServerConsoleSender || sender is CommandSender cmdSender && cmdSender.FullPermissions)
            {
                goto skipPermissionCheck;
            }
            if (!config.PermissionList.Contains(ServerStatic.PermissionsHandler._members[player.UserId]))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }
            skipPermissionCheck:
            
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

            if (!(ev is IEventMap map && !string.IsNullOrEmpty(map.MapInfo.MapName)))
            {
                DebugLogger.LogDebug("No map has been specified for this event!", LogLevel.Warn, true);
            }
            else if (!Extensions.IsExistsMap(map.MapInfo.MapName))
            {
                response = $"<color=red>You need a map {map.MapInfo.MapName} to run a mini-game.</color>";
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

            response = $"<color=green>The mini-game {ev.Name} has started!</color>";
            return false;
        }
    }
}
