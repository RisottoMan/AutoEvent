// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DeathPartyConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/17/2023 10:50 PM
//    Created Date:     09/17/2023 10:50 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.Interfaces;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AutoEvent.Games.Infection;

[Description("Be aware that this plugin can cause lag if not carefully balanaced.")]
public class DeathPartyConfig : EventConfig
{
    [Description("How many grenades will spawn in the round from 1 - 300. [Default: 20 - 110]")]
    public GrenadeItem GrenadeCount { get; set; } = new GrenadeItem(20, 110);
    
    [Description("How fast grenades will appear from 0 - 5. [Default: 1 - 0.25]")]
    public GrenadeItem GrenadeSpeed { get; set; } = new GrenadeItem(1f, 0.1f);

    [Description("How high the grenades should spawn from 0 - 30. [Default: 20 - 5]")]
    public GrenadeItem GrenadeHeight { get; set; } = new GrenadeItem(20, 5);

    [Description("How long the grenade fuse is, from when grenades spawn. from 2 - 10. [Default: 10 - 4]")] 
    public GrenadeItem GrenadeFuseTime { get; set; } = new GrenadeItem(10, 4);
    
    [Description("How large should grenades become from 0.1 to 75. [Default: 4 - 1")]
    public GrenadeItem GrenadeScale { get; set; } = new GrenadeItem(4, 1);
    
    [Description("How far from center should grenades spawn from 1 to 30. [Default: 4 - 25")]
    public GrenadeItem GrenadeSpawnRadius { get; set; } = new GrenadeItem(4, 25);
    
    [Description("Should grenades spawn on top of randomly chosen players. This will not apply on the last round.")]
    public bool TargetPlayers { get; set; } = false;

    [Description("The amount of rounds that this gamemode lasts. The last round is always a super big grenade.")] 
    public int Rounds { get; set; } = 5;

}

public class GrenadeItem
{
    public GrenadeItem()
    {
        
    }
    public GrenadeItem(float start, float end)
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
}

public enum Filter
{
    Linear,
    //Quadratic,
    //Exponential,
}