using CommandSystem;
using System;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MainCommand : ParentCommand
    {
        public override string Command => "ev";
        public override string Description => "main command for AutoEvent";
        public override string[] Aliases => Array.Empty<string>();
        // todo: add a command that allows admins to change event configs before / during games. Maybe create some preset configs like "crazy" or "relaxed", and use an attribute system.
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ListEvents());
            RegisterCommand(new RunEvent());
            RegisterCommand(new StopEvent());
            RegisterCommand(new Volume());
            RegisterCommand(new Reload());
            RegisterCommand(new Debug());
        }
        public MainCommand() => LoadGeneratedCommands();
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "please enter a valid subcommand: \nlist -> gets the list of the events\nrun -> run an event\nstop -> stop the current event\nvolume -> change the volume of the sounds\nreload -> reloads events";
            return false;
        }
    }
}
