using AutoEvent.Interfaces;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Events.Vote
{
    public class Plugin// : Event
    {
        public string Name { get; set; } = "Vote";
        public string Description { get; set; } = "Vote";
        public string MapName { get; set; }
        public string CommandName { get; set; } = "vote";

        public static List<Player> PlayerVotes;

        EventHandler _eventHandler;

        public void OnStart()
        {
            _eventHandler = new EventHandler();

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
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
