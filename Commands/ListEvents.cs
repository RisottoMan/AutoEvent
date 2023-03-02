using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class ListEvents : ICommand
    {
        public string Command => "ev_list";
        public string Description => "Shows a list of all the events that can be started.";
        public string[] Aliases => null;
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("ev.list"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            Player admin = Player.Get((sender as CommandSender).SenderId);

            string resp = String.Empty;
            resp += "<color=yellow><b>List of events (when running an event, you are responsible for it)</color></b>:\n";
            var arr = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "AutoEvent.Events");
            foreach (var type in arr)
            {
                if (type.GetProperty("CommandName") != null)
                {
                    var command = Activator.CreateInstance(type);
                    var description = Activator.CreateInstance(type);
                    try
                    {
                        resp += $"<b><color=yellow>[{type.GetProperty("CommandName").GetValue(command)}]</color></b> <= {type.GetProperty("Description").GetValue(description)}\n";
                    }
                    catch (Exception ex)
                    {
                        response = $"An error occurred while reading the events: {ex.Message}";
                        return false;
                    }
                }
            }
            response = resp;
            return true;
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
