namespace AutoEvent.Games.Boss;

public enum StateEnum
{
    /// <summary>
    /// The transitional state between all states
    /// </summary>
    Waiting,

    /// <summary>
    /// The state in which Santa Claus runs around the arena around the center
    /// </summary>
    Running,

    /// <summary>
    /// The state in which Santa Claus runs after one player and makes him a zombie
    /// </summary>
    CatchingUp,

    /// <summary>
    /// The state in which Santa Claus creates Lava Positions
    /// </summary>
    CreatingLava,

    /// <summary>
    /// The state in which Santa creates a rain of grenades
    /// </summary>
    GrenadeRain,

    /// <summary>
    /// The state in which Santa creates a Labyrinth
    /// </summary>
    Labyrinth,

    /// <summary>
    /// The state in which Santa creates a funny message that prevents shooting
    /// </summary>
    FunnyMessage,

    /// <summary>
    /// The state in which Santa summons evil mini-santa minions
    /// </summary>
    SummoningMinions
}
