using System.ComponentModel;
using Exiled.API.Features;

namespace AutoEvent.API;
public class ArtificialHealth
{
    public ArtificialHealth(){}

    public ArtificialHealth(float initialHealth, float maxHealth = 100f, float regenerationAmount = 1f, float absorptionPercent = 100, bool permanent = true, float duration = 10f, bool clearOtherInstances = true)
    { 
        InitialAmount = initialHealth;
        MaxAmount = maxHealth;
        RegenerationAmount = regenerationAmount;
        AbsorptionPercent = absorptionPercent;
        Permanent = permanent;
        Duration = duration;
    }

    public void ApplyToPlayer(Player player)
    {
        if (MaxAmount <= 0)
            return;
        
        player.AddAhp(InitialAmount, MaxAmount, -1 * RegenerationAmount, AbsorptionPercent / 100f, Duration, Permanent);
    }
    [Description("How much AHP the player will get at first.")]
    public float InitialAmount { get; set; } = 0f;

    [Description("The max amount the player will be able to get.")]
    public float MaxAmount { get; set; } = 100f;

    [Description("The amount of artificial hp the player will get each cycle. 1 is slow, 5 is fast, and 10+ is really fast")]
    public float RegenerationAmount { get; set; } = 1f;

    [Description("The percent of damage that the artificial health will take before effecting the player. (70% = when the player takes 10 damage, 7 of it will be absorbed by ahp, 3 will hurt them directly.)")]
    public float AbsorptionPercent { get; set; } = 100f;
    
    [Description("If set to false, the player will lose this after they take enough damage, or it wears out.")]
    public bool Permanent { get; set; } = true;

    [Description("How long to wait before regenerating.")]
    public float Duration { get; set; } = 30f;
}