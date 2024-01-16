using PluginAPI.Core;
using System;

namespace AutoEvent.Games.Boss;

public class CatchingUpState : IBossState
{
    public Player CatchingPlayer { get; set; }
    public TimeSpan Timer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Init()
    {
        
    }

    public void Init(TimeSpan eventTime)
    {
        throw new NotImplementedException();
    }

    public void Update()
    {
        
    }
}
