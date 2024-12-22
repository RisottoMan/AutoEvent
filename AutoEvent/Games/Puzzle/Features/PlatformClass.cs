using UnityEngine;

namespace AutoEvent.Games.Puzzle;

public class PlatformClass
{
    public PlatformClass(float sizeX, float sizeY, float positionX, float positionY)
    {
        X = sizeX;
        Y = sizeY;
        PositionX = positionX;
        PositionY = positionY;
    }

    public GameObject GameObject { get; set; }
    public ushort PlatformId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}