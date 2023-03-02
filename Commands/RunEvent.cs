using AutoEvent.Interfaces;
using CommandSystem;
using System;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class RunEvent : ICommand
    {
        public string Command => "ev_run";
        public string Description => "Run the event, takes on 1 argument - the command name of the event.";
        public string[] Aliases => null;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("ev.run"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            Player admin = Player.Get((sender as CommandSender).SenderId);
            if (!Round.IsStarted)
            {
                response = "The round has not started!";
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
            var arr = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "AutoEvent.Events");
            foreach (var type in arr)
            {
                if (type.GetProperty("CommandName") != null)
                {
                    var ev = Activator.CreateInstance(type);
                    try
                    {
                        if ((string)type.GetProperty("CommandName").GetValue(ev) == arguments.ElementAt(0))
                        {
                            var eng = type.GetMethod("OnStart");
                            if (eng != null)
                            {
                                sender.Respond("Trying to run an event, OnStart is not null...");
                                eng.Invoke(Activator.CreateInstance(type), null);
                                Round.IsLocked = true;
                                AutoEvent.ActiveEvent = (IEvent)ev;
                                response = "The event is found, run it.";
                                return true;
                            }
                            response = "Somehow, the class that was selected does not have OnStart() in it";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        response = $"An error occurred when running the event. Error: {ex.Message}";
                    }
                }
            }
            response = "The event was not found, nothing happened.";
            return false;
        }

        static private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }
    }
}
