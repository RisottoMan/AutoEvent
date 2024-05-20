namespace AutoEvent.Games.Jail;

public static class Preset
{
    public static Config AdminEvent => new Config()
    {
        LockdownSettings = new LockdownSettings()
        {
            AutoReleaseDelayInSeconds = -1,
            LockdownDurationInSeconds = -1,
            LockdownCooldownDurationInSeconds = -1,
            LockdownLocksGatesAsWell = false,
        },
        PrisonerLives = 1,
    };

    public static Config PublicServerEvent => new Config()
    {
        LockdownSettings = new LockdownSettings(20f, 15f, 15f, true),
        PrisonerLives = 3,
    };
}