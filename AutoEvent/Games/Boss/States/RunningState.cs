using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class RunningState : IBossState
{
    public Vector3 NextPosition { get; set; }
    public TimeSpan Timer { get; set; }

    public void Init(TimeSpan eventTime)
    {
        Timer = new TimeSpan(0, 0, 5);
    }

    public void Update()
    {
        DebugLogger.LogDebug(Timer.TotalSeconds.ToString());
    }
}
