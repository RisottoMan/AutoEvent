// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ColorExtensions.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 9:13 PM
//    Created Date:     10/16/2023 9:13 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace AutoEvent.API;

public class ColorMatcher
{
    // closed match for hues only:
    public int closestColor1(List<Color> colors, Color target)
    {
        Color.RGBToHSV(target, out float h, out _, out _);
        var diffs = colors.Select(n =>
        {
            Color.RGBToHSV(n, out float h2, out _, out _);
            return getHueDistance(h2, h);
        });
        var diffMin = diffs.Min(n => n);
        return diffs.ToList().FindIndex(n => n == diffMin);
    }

// closed match in RGB space
    public int closestColor2(List<Color> colors, Color target)
    {
        var colorDiffs = colors.Select(n => ColorDiff(n, target)).Min(n => n);
        return colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
    }

// weighed distance using hue, saturation and brightness
    public int closestColor3(List<Color> colors, Color target)
    {
        Color.RGBToHSV(target, out float h, out _, out _);
        var num1 = ColorNum(target);
        var diffs = colors.Select(n =>
        {
            
            Color.RGBToHSV(n, out float h2, out _, out _);
            return Math.Abs(ColorNum(n) - num1) +
                   getHueDistance(h2, h);
        });
        var diffMin = diffs.Min(x => x);
        return diffs.ToList().FindIndex(n => n == diffMin);
    }

    // color brightness as perceived:
    public float getBrightness(Color c)
    {
        return (c.r * 0.299f + c.g * 0.587f + c.b * 0.114f) / 256f;
    }

// distance between two hues:
    public float getHueDistance(float hue1, float hue2)
    {
        var d = Math.Abs(hue1 - hue2);
        return d > 180 ? 360 - d : d;
    }

//  weighed only by saturation and brightness (from my trackbars)
    public float ColorNum(Color c)
    {
        Color.RGBToHSV(c, out float _, out float s, out _);

        return s * 1 +
               getBrightness(c) * 1;
    }

// distance in RGB space
    public int ColorDiff(Color c1, Color c2)
    {
        return (int)Math.Sqrt((c1.r - c2.r) * (c1.r - c2.r)
                              + (c1.g - c2.g) * (c1.g - c2.g)
                              + (c1.b - c2.b) * (c1.b - c2.b));
    }
}