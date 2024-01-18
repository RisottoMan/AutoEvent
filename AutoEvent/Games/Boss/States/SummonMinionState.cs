using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class SummonMinionState : IBossState
{
    public string Name { get; } = "Summonning Minions";
    public string Description { get; } = "The state in which Santa summons evil mini-santa minions";
    public TimeSpan Timer { get; set; }

    public void Init()
    {

    }

    public void Update()
    {

    }

    public void Deinit()
    {

    }
}
