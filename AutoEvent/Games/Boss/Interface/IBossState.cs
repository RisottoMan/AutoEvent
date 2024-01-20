using MER.Lite.Objects;
using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public interface IBossState
{
    string Name { get; }
    string Description { get; }
    int Stage { get; }
    Animator Animation { get; set; }
    TimeSpan Timer { get; set; }
    void Init(Plugin plugin);
    void Update();
    void Deinit();
}