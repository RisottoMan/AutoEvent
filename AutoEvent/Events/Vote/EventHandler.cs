using Exiled.Events.EventArgs.Player;
using PluginAPI.Events;

namespace AutoEvent.Events.Vote
{
    internal class EventHandler
    {
        public void OnToggleNoclip(TogglingNoClipEventArgs ev)
        {
            if (!Plugin.PlayerVotes.Contains(ev.Player))
            {
                Plugin.PlayerVotes.Add(ev.Player);
            }
        }

        public void OnVoiceChat(VoiceChattingEventArgs ev)
        {
            if (!Plugin.PlayerVotes.Contains(ev.Player))
            {
                Plugin.PlayerVotes.Remove(ev.Player);
            }
        }

        public void OnPlayerLeft(LeftEventArgs ev)
        {
            if (!Plugin.PlayerVotes.Contains(ev.Player))
            {
                Plugin.PlayerVotes.Remove(ev.Player);
            }
        } 
    }
}
