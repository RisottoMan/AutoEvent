using AutoEvent.Events.EventArgs;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.Vote
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            if (!_plugin._voteList.ContainsKey(ev.Player))
            {
                _plugin._voteList.Add(ev.Player, false);
            }
        }

        [PluginEvent(ServerEventType.PlayerLeft)]
        public void OnPlayerLeft(PlayerLeftEvent ev)
        {
            if (_plugin._voteList.ContainsKey(ev.Player))
            {
                _plugin._voteList.Remove(ev.Player);
            }
        }

        public void OnPlayerNoclip(PlayerNoclipArgs ev)
        {
            if (_plugin._voteList.ContainsKey(ev.Player))
            {
                _plugin._voteList[ev.Player] = !_plugin._voteList[ev.Player];
            }
        }
    }
}
