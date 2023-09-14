using CommandSystem;
using System;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MainCommand : ParentCommand, IUsageProvider
    {
        public override string Command => "ev";
        public override string Description => "main command for AutoEvent";
        public override string[] Aliases => Array.Empty<string>();
        public string[] Usage => Array.Empty<string>();

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new List());
            RegisterCommand(new Run());
            RegisterCommand(new Stop());
            RegisterCommand(new Volume());
            RegisterCommand(new Reload.Reload());
            RegisterCommand(new Debug.Debug());
        }
        public MainCommand() => LoadGeneratedCommands();
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please enter a valid subcommand: \n" +
                       "<color=yellow>  list <color=white>-> gets the list of the events\n" +
                       "<color=yellow>  run <color=white>-> run an event\n" +
                       "<color=yellow>  stop <color=white>-> stop the current event\n" +
                       "<color=yellow>  volume <color=white>-> change the volume of the sounds\n" +
                       "<color=yellow>  reload <color=white>-> reloads events, configs, and translations";
            return false;
        }
    }
}
