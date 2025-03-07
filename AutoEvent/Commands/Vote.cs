using AutoEvent.Interfaces;
using CommandSystem;
using System;
using MEC;
using PluginAPI.Core;
using Exiled.Permissions.Extensions;
using Callvote;
using Callvote.VoteHandlers;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Commands;
internal class Vote : ICommand, IUsageProvider
{
    public string Command => nameof(Vote);
    public string Description => "Starts voting for mini-game, 1 argument - the command name of the event";
    public string[] Aliases => [];
    public string[] Usage => ["Event Name"];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.ToLower().Contains("callvote")))
        {
            response = "Callvote was not detected. Please install it if you want to use it's functionality.";

        }
        if (!sender.CheckPermission("ev.vote"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }
        if (AutoEvent.EventManager.CurrentEvent != null)
        {
            response = $"The mini-game {AutoEvent.EventManager.CurrentEvent.Name} is already running!";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "Only 1 argument is needed - the command name of the event!";
            return false;
        }

        Event ev = AutoEvent.EventManager.GetEvent(arguments.At(0));
        if (VotingAPI.CurrentVoting is not null)
        {
            response = Callvote.Callvote.Instance.Translation.VotingInProgress;
            return false;
        }

        Round.IsLocked = true;

        Dictionary<string, string> options = new Dictionary<string, string>();
        options.Add(Callvote.Callvote.Instance.Translation.CommandYes, Callvote.Callvote.Instance.Translation.OptionYes);
        options.Add(Callvote.Callvote.Instance.Translation.CommandNo, Callvote.Callvote.Instance.Translation.OptionNo);

        VotingAPI.CurrentVoting = new Voting("%Player% <color=#EEDC8A>asks</color>: Start event %Event%?".Replace("%Player%", Player.Get(sender).Nickname).Replace("%Event%", ev.Name), options, Player.Get(sender), delegate (Voting vote)
        {
            if (vote.Counter[Callvote.Callvote.Instance.Translation.CommandYes] < vote.Counter[Callvote.Callvote.Instance.Translation.CommandNo])
            {
                Map.Broadcast(5, "Not enough votes to start a event! Aborting...");
                Round.IsLocked = false;
            }
            Map.Broadcast(5, "Voting passed! Starting event...");
            if (!Round.IsRoundStarted)
            {
                Round.Start();

                Timing.CallDelayed(2f, () => {

                    Extensions.TeleportEnd();
                    ev.StartEvent();
                    AutoEvent.EventManager.CurrentEvent = ev;
                });
            }
            else
            {
                ev.StartEvent();
                AutoEvent.EventManager.CurrentEvent = ev;
            }
        });

        response = $"The voting for {arguments.At(0)} has started!"; //$"The vote {ev.Name} has started!"
        return true;
    }
}