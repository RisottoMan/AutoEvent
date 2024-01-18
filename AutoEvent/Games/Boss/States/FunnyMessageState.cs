using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class FunnyMessageState : IBossState
{
    private Vector3 _newPos;
    public string Name { get; } = "FunnyMessage";
    public string Description { get; } = "The state in which Santa creates a funny message that prevents shooting";
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
