using CommandSystem;
using System;
using PluginAPI.Core;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class MainCommand : ParentCommand
    {
        public override string Command => "ev";
        public override string Description => "Main command for AutoEvent";
        public override string[] Aliases => new string [] { };

        public override void LoadGeneratedCommands()
        {
            try
            {
                // Log.Debug("Skipping Main Command");
                RegisterCommand(new List());
                RegisterCommand(new Run());
                RegisterCommand(new Stop());
                RegisterCommand(new BuildInfo());
                //RegisterCommand(new Lobby());
                //RegisterCommand(new Vote());
                RegisterCommand(new Volume());
                RegisterCommand(new NoRestart()); 
                RegisterCommand(new Reload.Reload());
                RegisterCommand(new Debug.Debug());
                RegisterCommand(new Config.Config());
            }
            catch (Exception e)
            {
                Log.Warning($"Caught an exception while registering commands.");
                Log.Debug($"{e}");
            }
        }
        public MainCommand() => this.LoadGeneratedCommands();
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please enter a valid subcommand: \n";
            foreach (var x in this.Commands)
            {
                string args = "";
                if (x.Value is IUsageProvider usage)
                {
                    foreach (string arg in usage.Usage)
                    {
                        args += $"[{arg}] ";
                    }
                }

                if (sender is not ServerConsoleSender)
                    response += $"<color=yellow> {x.Key} {args}<color=white>-> {x.Value.Description}. \n";
                else
                    response += $" {x.Key} {args} -> {x.Value.Description}. \n";
            }
        
                       // "<color=yellow>  list <color=white>-> gets the list of the events\n" +
                       // "<color=yellow>  run <color=white>-> run an event\n" +
                       // "<color=yellow>  stop <color=white>-> stop the current event\n" +
                       // "<color=yellow>  volume <color=white>-> change the volume of the sounds\n" +
                       // "<color=yellow>  reload <color=white>-> reloads events, configs, and translations";
            return false;
        }
    }

}
