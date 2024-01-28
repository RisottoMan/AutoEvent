using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class GrenadeRainState : IBossState
{
    public string Name { get; } = "Grenade Rain";
    public string Description { get; } = "The state in which Santa creates a rain of grenades";
    public int Stage { get; } = 1;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; }

    public void Init(Plugin plugin)
    {

    }

    public void Update()
    {

    }

    public void Deinit()
    {

    }
}
