using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class JumpingState : IBossState
{
    private GameObject _santaObject;
    public string Name { get; } = "JumpingState";
    public string Description { get; } = "Santa Claus jumps creating a wave that need to jump over";
    public int Stage { get; } = 1;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; } = new TimeSpan(0, 0, 10);

    public void Init(Plugin plugin)
    {
        _santaObject = plugin.santaObject;
    }

    public void Update()
    {

    }

    public void Deinit()
    {

    }
}
