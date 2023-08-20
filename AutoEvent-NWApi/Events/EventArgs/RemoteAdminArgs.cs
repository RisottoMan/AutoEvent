
using PluginAPI.Core;
using System.Reflection;

namespace AutoEvent.Events.EventArgs
{
    public class RemoteAdminArgs
    {
        public RemoteAdminArgs(CommandSender sender, Player player, string command, string name, string[] args, bool isSuccess = true, bool isAllowed = true)
        {
            CommandSender = sender;
            Player = player;
            Command = command;
            Name = name;
            Args = args;
            IsSuccess = isSuccess;
            IsAllowed = isAllowed;
        }
        public string Reply
        {
            get
            {
                return returnMessage;
            }
            set
            {
                if (pref == "")
                {
                    pref = Assembly.GetCallingAssembly().GetName().Name;
                }

                returnMessage = pref + "#" + value;
            }
        }

        public string Prefix
        {
            get
            {
                return pref;
            }
            set
            {
                pref = value;
            }
        }

        public CommandSender CommandSender { get; }

        public Player Player { get; }

        public string Command { get; }

        public string Name { get; }

        public string[] Args { get; }

        public bool IsSuccess { get; set; }

        public bool IsAllowed { get; set; }

        private string returnMessage;

        public string pref;
    }
}
