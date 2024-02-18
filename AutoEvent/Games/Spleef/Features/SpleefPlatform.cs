using UnityEngine;

namespace AutoEvent.Games.Spleef;
public class SpleefPlatform
{
    public SpleefPlatform(float sizeX, float sizeY, float sizeZ, float positionX, float positionY, float positionZ, Color color)
    {
        X = sizeX;
        Y = sizeY;
        Z = sizeZ;
        PositionX = positionX;
        PositionY = positionY;
        PositionZ = positionZ;
        Color = color;
    }

    public GameObject GameObject { get; set; }
    public ushort PlatformId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public Color Color { get; set; }
}