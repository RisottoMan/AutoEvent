using AutoEvent.Interfaces;
using CommandSystem;
using System;
using System.Linq;
using AutoEvent.API;
using MEC;
using PluginAPI.Core;
using PlayerRoles;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands
{
    internal class Vote// : ICommand, IUsageProvider, IPermission
    {
        public string Command => nameof(Vote);
        public string Description => "Starts voting for mini-game, 1 argument - the command name of the event";
        public string[] Aliases => new string[] { };
        public string[] Usage => new string[] { "Event Name" };
        public string Permission { get; set; } = "ev.vote";

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
            if (ev == null || ev is IHidden)
            {
                response = $"The mini-game {arguments.At(0)} is not found.";
                return false;
            }

            Event vote = Event.GetEvent("Vote");
            if (vote == null)
            {
                response = $"The vote is not found.";
                return false;
            }

            IVote comp = vote as IVote;
            if (comp == null)
            {
                response = $"The IVote is not found.";
                return false;
            }

            comp.NewEvent = ev;
            Round.IsLocked = true;

            if (!Round.IsRoundStarted)
            {
                Round.Start();

                Timing.CallDelayed(2f, () => {

                    Extensions.TeleportEnd();
                    vote.StartEvent();
                    AutoEvent.ActiveEvent = vote;
                });
            }
            else
            {
                vote.StartEvent();
                AutoEvent.ActiveEvent = vote;
            }

            response = $"The vote {ev.Name} has started!";
            return true;
        }

    }
}
