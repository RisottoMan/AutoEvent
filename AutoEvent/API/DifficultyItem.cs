using System.ComponentModel;
using UnityEngine;

namespace AutoEvent.API;
public class DifficultyItem
{
    public DifficultyItem()
    {
        
    }
    
    public DifficultyItem(float start, float end)
    {
        StartingValue = start;
        EndingValue = end;
    }

    [Description("The value at the start of the game.")]
    public float StartingValue { get; set; }

    [Description("The value at the end of the game.")]
    public float EndingValue { get; set; }

    public float GetValue(int round, int maxRound, float minClamp, float maxClamp)
    {
        round -= 1;
        var zero = new Vector2(0, Mathf.Clamp(StartingValue, minClamp, maxClamp));
        var end = new Vector2(maxRound - 1, Mathf.Clamp(EndingValue, minClamp, maxClamp));
        return _getLinearValue(zero, end, round);
    }

    private float _getLinearValue(Vector2 point1, Vector2 point2, int x)
    {
        var dx = point2.x - point1.x;  //This part has problem in your code
        if (dx == 0)
            return float.NaN;
        var m = (point2.y - point1.y) / dx;
        var b = point1.y - (m * point1.x);

        return m*x + b;
    }
}