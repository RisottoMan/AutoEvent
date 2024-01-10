using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Games.Fnaf.Features;

public class AnimatronicClass
{
    public int Level { get; set; }
    public FreddyState State { get; set; }
    public GameObject GameObject { get; set; }
    public List<Vector3> Positions { get; set; }
    public int IndexPosition { get; set; }
    public float Timer { get; set; }
    public float Counter { get; set; }
    public bool IsDoorClosed { get; set; }
}
