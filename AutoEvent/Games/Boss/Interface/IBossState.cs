using System;

namespace AutoEvent.Games.Boss;

public interface IBossState
{
    string Name { get; }
    string Description { get; }
    TimeSpan Timer { get; set; }
    void Init();
    void Update();
    void Deinit();
}