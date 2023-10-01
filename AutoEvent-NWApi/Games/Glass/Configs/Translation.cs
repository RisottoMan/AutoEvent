using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class GlassTranslate  : ITranslation 
#else
    public class GlassTranslate
#endif
    {
        public string GlassCommandName { get; set; } = "glass";
        public string GlassName { get; set; } = "Dead Jump";
        public string GlassDescription { get; set; } = "Jump on fragile platforms";
        public string GlassStart { get; set; } = "<size=50>Dead Jump\nJump on fragile platforms</size>\n<size=20>Alive players: {plyAlive} \nTime left: {time}</size>";
        public string GlassDied { get; set; } = "You fallen into lava";
        public string GlassWinSurvived { get; set; } = "<color=yellow>Human wins! Survived {plyAlive} players</color>";
        public string GlassWinner { get; set; } = "<color=red>Dead Jump</color>\n<color=yellow>Winner: {winner}</color>";
        public string GlassFail { get; set; } = "<color=red>Dead Jump</color>\n<color=yellow>All players died</color>";
    }
}