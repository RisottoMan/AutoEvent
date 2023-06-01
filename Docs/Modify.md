# 😈 Modify
## You can easily learn the structure of mini games and can create your own mini games
## Or you can view the source code in the Example folder [here](https://github.com/KoT0XleB/AutoEvent-Exiled/blob/main/Events/Example/Plugin.cs)
```
namespace AutoEvent.Events.Example
{
    // This is an example that will help you figure out how to start creating mini-games.It's easy!
    public class Plugin // : IEvent // <- Uncomment the line so that the mini-game becomes visible in the ev_list command
    {
        public string Name => "Example Event Name";
        public string Description => "Example Event Description";
        public string Color => "FFFF00";
        public string CommandName => "command_name";
        public TimeSpan EventTime { get; set; } // This is a time counter that can be used to show the duration of the mini-game

        EventHandler _eventHandler;

        // the OnStart() code that runs when using ev_run [command_name]
        public void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            OnEventStarted();
        }
        // the OnStop() code that stops the mini-game from working
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;

            Timing.CallDelayed(5f, () => EventEnd()); // The delay that takes five seconds to do the cleanup
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }

        // It starts after OnStart() and performs actions before the start of the mini-game
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Player.List.ToList().ForEach(player =>
            {
                player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.All);
                //player.EnableEffect(EffectType.Ensnared, 10);
            });
            // Uncomment the line to turn on the music
            //Extensions.PlayAudio("Music_Name.ogg", volume, loop, name_of_bot);

            Timing.RunCoroutine(OnEventRunning(), "escape_run");
        }
        // In mini games , there are three stages of the game:
        // 1) Before the start;
        // 2) Looping until the end condition is met, for example, until there are no players left;
        // 3) The end of the mini-game;
        // Proper development and implementation of all these stages is essential.
        public IEnumerator<float> OnEventRunning()
        {
            // 1) Countdown before the start of the game
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast("Broadcast before the start of the mini-game", 1);
                yield return Timing.WaitForSeconds(1f); // note that yield return produces a delay every 1 second or whatever value you want.
                EventTime += TimeSpan.FromSeconds(1f); // And this is a time counter
            }

            // 2) Looping until the necessary condition for the end of the mini-game occurs.
            // Also note that you have to check the number of live players, otherwise the round will loop. => Player.List.Count(r => r.IsAlive) > 0
            while (Player.List.Count(r => r.Role.Type == RoleTypeId.ClassD) > 5 && Player.List.Count(r => r.IsAlive) > 0) 
            {
                Extensions.Broadcast("Looping mini-games, you can display the time of the game or the number of remaining players", 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            // 3) The end of the mini game
            foreach (Player player in Player.List)
            {
                player.Kill("End game");
            }
            Extensions.Broadcast("Broadcast of the end of the mini-game", 10);

            OnStop();
            yield break;
        }
        // Cleaning is also no less necessary so that you can play several mini-games without problems.
        public void EventEnd()
        {
            Extensions.CleanUpAll(); // We clean all ragdolls and items
            Extensions.TeleportEnd(); // We'll teleport everyone to the tower
            Extensions.StopAudio(); // We turn off the music
        }
    }
}
```
