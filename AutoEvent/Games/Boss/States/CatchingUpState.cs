using PluginAPI.Core;
using System;

namespace AutoEvent.Games.Boss;

public class CatchingUpState : IBossState
{
    private Player? _target { get; set; }
    public string Name { get; } = "CatchingUp";
    public string Description { get; } = "The state in which Santa Claus runs after one player and makes him a minion-Santa";
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
