using AutoEvent.Interfaces;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Events.Vote
{
    public class Plugin// : Event
    {
        public string Name { get; set; } = "Vote";
        public string Description { get; set; } = "Vote";
        public string Author { get; set; } = "KoT0XleB";
        public string MapName { get; set; }
        public string CommandName { get; set; } = "vote";

        public List<Player> PlayerVotes;
        EventHandler _eventHandler;
        public string eventName { get; set; }

        public void OnStart()
        {
            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.TogglingNoClip += _eventHandler.OnToggleNoclip;
            Exiled.Events.Handlers.Player.VoiceChatting += _eventHandler.OnVoiceChat;
            Exiled.Events.Handlers.Player.Left += _eventHandler.OnPlayerLeft;

            Timing.RunCoroutine(OnEventRunning(), "battle_time");
        }

        public void OnStop()
        {
            Exiled.Events.Handlers.Player.TogglingNoClip -= _eventHandler.OnToggleNoclip;
            Exiled.Events.Handlers.Player.VoiceChatting -= _eventHandler.OnVoiceChat;
            Exiled.Events.Handlers.Player.Left -= _eventHandler.OnPlayerLeft;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public IEnumerator<float> OnEventRunning()
        {
            PlayerVotes = new();

            foreach(Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.Tutorial);
            }

            /*
            Event ev = Event.GetEvent(arguments.At(0));
            if (ev == null)
            {
                response = "This mini-game has not been found.";
                return false;
            }
            */

            for (int time = 30; time > 0; time--)
            {
                var text = $"Опрос: {PlayerVotes.Count()} из {Player.List.Count()} за ивент {Name}\n" +
                    $"Нажмите [ALT] За и [Q] Против\n" +
                    $"Осталось {time} секунд";
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            //Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
