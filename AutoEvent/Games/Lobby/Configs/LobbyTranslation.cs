using Exiled.API.Interfaces;

namespace AutoEvent.Games.Lobby
{
#if EXILED
    public class LobbyTranslation : ITranslation 
#else
    public class LobbyTranslation
#endif
    {
        public string LobbyCommandName { get; set; } = "lobby";
        public string LobbyName { get; set; } = "Lobby";
        public string LobbyDescription { get; set; } = "A lobby in which one quick player chooses a mini-game.";
        public string LobbyCycle { get; set; } = "<color=#1378f2>Vote: Press [Alt] Pros or [Alt]x2 Cons</color>\n<color=yellow>{trueCount}</color> <color=#1378f2>of</color> <color=yellow>{allCount}</color> <color=#1378f2>players for {newName}</color>\n<color=yellow>{time}</color> <color=#1378f2>seconds left!</color>";
        public string LobbyGlobalMessage { get; set; } = "<color=#f03405>Lobby</color>\n<color=#1378f2>{message}</color>\n<color=yellow>{count}</color> <color=#f03405>players in the lobby</color>";
        public string LobbyGetReady { get; set; } = "<b><color=yellow>Get ready to run to the center and choose a mini game</color></b>";
        public string LobbyRun { get; set; } = "<color=red>RUN</color>";
        public string LobbyChoosing { get; set; } = "<b><color=#1378f2>Waiting for the {nickName} to choose mini-game</color></b>";
        public string LobbyFinishMessage { get; set; } = "<color=#1378f2>The lobby is finished.</color>\n<color=#1378f2>The player {nickName} chose the {newName} mini-game.</color>\n<color=#f03405>Total</color> <color=yellow>{count}</color> <color=#f03405>players in the lobby</color>";
    }
}