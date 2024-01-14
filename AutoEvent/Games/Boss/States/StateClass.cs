using MER.Lite.Objects;
using System;

namespace AutoEvent.Games.Boss;

public class StateClass<T>
{
    public SchematicObject SantaObject { get; set; }
    public StateEnum CurrentState { get; set; }
    public T CurrentValue { get; set; }
    public TimeSpan Timer { get; set; }
}
