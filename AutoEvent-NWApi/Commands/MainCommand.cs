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
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ListEvents());
            RegisterCommand(new RunEvent());
            RegisterCommand(new StopEvent());
            RegisterCommand(new Volume());
        }
        public MainCommand() => LoadGeneratedCommands();
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "please enter a valid subcommand: \nlist -> gets the list of the events\nrun -> run an event\nstop -> stop the current event";
            return false;
        }
    }
}
