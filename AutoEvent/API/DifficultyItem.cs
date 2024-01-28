// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DifficultyItem.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 4:01 PM
//    Created Date:     09/18/2023 4:01 PM
// -----------------------------------------

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

    [Description("The method that the value will be calculated.")]
    public Filter CalculationMethod { get; set; } = Filter.Linear;
    public float GetValue(int round, int maxRound, float minClamp, float maxClamp)
    {

        round -= 1;
        var zero = new Vector2(0, Mathf.Clamp(StartingValue, minClamp, maxClamp));
        var end = new Vector2(maxRound - 1, Mathf.Clamp(EndingValue, minClamp, maxClamp));
        return _getLinearValue(zero, end, round);
        switch (CalculationMethod)
        {
            case Filter.Linear:
                return _getLinearValue(zero, end, round);
            /*case Filter.Quadratic:
                return _getQuadraticValue(zero, end, round);
            case Filter.Exponential:
                return _getExponentialValue(zero, end, round);*/
        }
        return 1;
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

    private float _getQuadraticValue(Vector2 point1, Vector2 point2, int x)
    {
        return 0;
        /*Vector3 p0 = point1.y;
        Vector3 p1 = middle.transform.position;
        Vector3 p2 = point2.y;
        float t;
        Vector3 position;
        for(int i = 0; i < numberOfPoints; i++)
        {
            t = i / (numberOfPoints - 1.0f);
            position = (1.0f - t) * (1.0f - t) * p0 
                       + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
            lineRenderer.SetPosition(i, position);
        }*/
    }
    private float _getExponentialValue(Vector2 point1, Vector2 point2, int x)
    {
        return 0;
        /**/
    }

    public enum Filter
    {
        Linear,
        //Quadratic,
        //Exponential,
    }
}