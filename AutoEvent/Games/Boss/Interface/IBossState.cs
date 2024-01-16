using System;

namespace AutoEvent.Games.Boss;

public interface IBossState
{
    TimeSpan Timer { get; set; }
    void Init(TimeSpan eventTime);
    void Update();
}