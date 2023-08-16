using Exiled.Events.EventArgs.Player;
using PluginAPI.Events;

namespace AutoEvent.Events.Vote
{
    internal class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnToggleNoclip(TogglingNoClipEventArgs ev)
        {
            if (!_plugin.PlayerVotes.Contains(ev.Player))
            {
                _plugin.PlayerVotes.Add(ev.Player);
            }
        }

        public void OnVoiceChat(VoiceChattingEventArgs ev)
        {
            if (!_plugin.PlayerVotes.Contains(ev.Player))
            {
                _plugin.PlayerVotes.Remove(ev.Player);
            }
        }

        public void OnPlayerLeft(LeftEventArgs ev)
        {
            if (!_plugin.PlayerVotes.Contains(ev.Player))
            {
                _plugin.PlayerVotes.Remove(ev.Player);
            }
        } 
    }
}
