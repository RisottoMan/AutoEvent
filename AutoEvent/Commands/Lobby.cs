using AutoEvent.Interfaces;
using CommandSystem;
using System;
using System.Linq;
using AutoEvent.API;
using MEC;
using PluginAPI.Core;
using Utils.NonAllocLINQ;
using PlayerRoles;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands
{
    internal class Lobby// : ICommand, IPermission
    {
        public string Command => nameof(Lobby);
        public string Description => "Starting a lobby in which the winner chooses a mini-game";
        public string[] Aliases => new string[] { };
        public string Permission { get; set; } = "ev.lobby";

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

            Event lobby = Event.GetEvent("Lobby");
            if (lobby == null)
            {
                response = $"The lobby is not found.";
                return false;
            }

            Round.IsLocked = true;

            if (!Round.IsRoundStarted)
            {
                Round.Start();

                Timing.CallDelayed(2f, () => {

                    lobby.StartEvent();
                    AutoEvent.ActiveEvent = lobby;
                });
            }
            else
            {
                lobby.StartEvent();
                AutoEvent.ActiveEvent = lobby;
            }

            response = $"The lobby event has started!";
            return true;
        }

    }
}
