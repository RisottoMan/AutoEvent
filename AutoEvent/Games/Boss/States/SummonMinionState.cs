using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class SummonMinionState : IBossState
{
    private bool IsMinionSpawned;
    public string Name { get; } = "Summonning Minions";
    public string Description { get; } = "The state in which Santa summons evil mini-santa minions";
    public int Stage { get; } = 1;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; } = new TimeSpan(0, 0, 20);

    public void Init(Plugin plugin)
    {
        IsMinionSpawned = false;
        Animation = plugin.santaObject.GetComponent<Animator>();
        Animation.Play("SummonSpawn");
    }

    public void Update()
    {
        if (!Animation.isActiveAndEnabled && IsMinionSpawned != true)
        {
            // Спавн миньонов
        }
    }

    public void Deinit()
    {

    }
}
