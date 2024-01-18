using System;

namespace AutoEvent.Games.Boss;

public class LabyrinthState : IBossState
{
    public string Name { get; } = "Labyrinth";
    public string Description { get; } = "The state in which Santa creates a Labyrinth";
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
