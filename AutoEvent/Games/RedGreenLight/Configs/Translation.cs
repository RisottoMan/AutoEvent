using Exiled.API.Interfaces;

namespace AutoEvent.Games.Light
{
#if EXILED
    public class LightTranslation : ITranslation 
#else
    public class LightTranslation
#endif
    {
        public string LightCommandName { get; set; } = "light";
        public string LightName { get; set; } = "Red Light Green Light";
        public string LightDescription { get; set; } = "Reach the end of the finish line.";
        public string LightStart { get; set; } = "<color=#42aaff>Get ready to run</color>\nThe game start in: <color=red>{time}</color>";
        public string LightCycle { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#aad9ff>{state}</color>\n<color=red>{time} seconds remaining</color>";
        public string LightAllDied { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>No one was able to reach the finish line</color>";
        public string LightMoreWin { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>{count} players win</color>";
        public string LightPlayerWin { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=green>Winner: {winner}</color>";
        public string LightRedLight { get; set; } = "<color=red>Dont move</color>";
        public string LightGreenLight { get; set; } = "<color=green>Run</color>";
        public string LightRedLose { get; set; } = "<color=red>You run a red light</color>";
        public string LightNoTime { get; set; } = "<color=red>You didnt make it to the finish line!</color>";
        //public string LightHint { get; set; } = "<color=green>Press <color=yellow>[Alt]</color> to push the player</color>";
        //public string LightHintWait { get; set; } = "<color=red>Wait few seconds</color>";
        //public string LightHintReady { get; set; } = "<color=green>Ready</color>";
    }
}