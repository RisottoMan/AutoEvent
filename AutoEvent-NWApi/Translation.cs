using AutoEvent.Games.Infection;
using System.ComponentModel;
#if EXILED
using Exiled.API.Interfaces;
#endif 
namespace AutoEvent
{
    // Yes, it looks terrible. I just have very little time to create a large plugin.
    // Later I will create a system of configs and translations.
    // Sorry :(
#if EXILED
    public class Translation : ITranslation
#else
    public class Translation
#endif
    {
        [Description("Zombie Infection Game Mode")]
        public InfectTranslate InfectTranslate { get; set; } = new InfectTranslate();

        [Description("Atomic Escape Game Mode")]
        public EscapeTranslate EscapeTranslate { get; set; } = new EscapeTranslate();

        [Description("Simon's Prison Game Mode")]
        public JailTranslate JailTranslate { get; set; } = new JailTranslate();

        [Description("Cock Fights Game Mode")]
        public VersusTranslate VersusTranslate { get; set; } = new VersusTranslate();

        [Description("Knives of Death Game Mode")]
        public KnivesTranslate KnivesTranslate { get; set; } = new KnivesTranslate();

        [Description("Deathmatch Game Mode")]
        public DeathmatchTranslate DeathmatchTranslate { get; set; } = new DeathmatchTranslate();

        [Description("GunGame Game Mode")]
        public GunGameTranslate GunGameTranslate { get; set; } = new GunGameTranslate();

        [Description("Battle Game Mode")]
        public BattleTranslate BattleTranslate { get; set; } = new BattleTranslate();

        [Description("Football Game Mode")]
        public FootballTranslate FootballTranslate { get; set; } = new FootballTranslate();

        [Description("Dead Jump Game Mode (Glass)")]
        public GlassTranslate GlassTranslate { get; set; } = new GlassTranslate();

        [Description("Puzzle Game Mode")]
        public PuzzleTranslate PuzzleTranslate { get; set; } = new PuzzleTranslate();

        [Description("Zombie Survival Game Mode (Zombie 2)")]
        public SurvivalTranslate SurvivalTranslate { get; set; } = new SurvivalTranslate();

        [Description("Fall Down Game Mode")]
        public FallTranslate FallTranslate { get; set; } = new FallTranslate();

        [Description("Death Line Game Mode")]
        public LineTranslate LineTranslate { get; set; } = new LineTranslate();

        [Description("Hide And Seek Game Mode")]
        public HideTranslate HideTranslate { get; set; } = new HideTranslate();

        [Description("Death Party Game Mode")]
        public DeathTranslate DeathTranslate { get; set; } = new DeathTranslate();

        [Description("FinishWay Game Mode")]
        public FinishWayTranslate FinishWayTranslate { get; set; } = new FinishWayTranslate();

        [Description("Zombie Escape Game Mode")]
        public ZombieEscapeTranslate ZombieEscapeTranslate { get; set; } = new ZombieEscapeTranslate();

        [Description("Lava Game Mode")]
        public LavaTranslate LavaTranslate { get; set; } = new LavaTranslate();

        [Description("Boss Battle Game Mode")]
        public BossTranslate BossTranslate { get; set; } = new BossTranslate();
    }
}
