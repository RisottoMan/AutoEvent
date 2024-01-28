using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class CreatingLavaState : IBossState
{
    public string Name { get; } = "CreatingLava";
    public string Description { get; } = "The state in which Santa Claus creates Lava Positions";
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
