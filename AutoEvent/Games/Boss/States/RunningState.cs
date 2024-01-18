using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class RunningState : IBossState
{
    private Vector3 _newPos;
    public string Name { get; } = "Running";
    public string Description { get; } = "The state in which Santa Claus runs around the arena around the center";
    public TimeSpan Timer { get; set; }

    public void Init()
    {
        Timer = new TimeSpan(0, 0, 5);
    }

    public void Update()
    {
        DebugLogger.LogDebug(Timer.TotalSeconds.ToString());
    }

    public void Deinit()
    {

    }
}
