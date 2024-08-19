using System;
using System.Collections.Generic;
using AutoEvent.API;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Puzzle;
public class GridSelector
{
    private byte GridSizeX { get; init; }
    private byte GridSizeY { get; init; }
    private string Salt { get; set; }
    public List<GridData> GridData { get; set; }

    public GridSelector(byte gridSizeX, byte gridSizeY, string salt)
    {
        GridSizeX = gridSizeX;
        GridSizeY = gridSizeY;
        GridData = new List<GridData>();
        Salt = salt;
    }

    public GridData SelectGridItem(byte platformSpread = 1, bool fillCenter = true,  Color? baseColor = null,float hueOffset = -1, float saturationOffset = -1, float vOffset = -1, int safePlatformCount = 1)
    {
        if (baseColor is null)
        {
            float r = UnityEngine.Random.Range((float)0, (float)1);
            float g = UnityEngine.Random.Range((float)0, (float)1);
            float b = UnityEngine.Random.Range((float)0, (float)1);
            baseColor = new Color(r, g, b);
        }
        Color.RGBToHSV(baseColor.Value, out float h, out float s, out float v);
        // DebugLogger.LogDebug($"Selecting GridInfo. Creating New Grid.");
        var data = new Puzzle.GridData(GridSizeX, GridSizeY);
        data.SafePointCount = safePlatformCount;
        data.Seed = (System.DateTime.Now.Ticks + Salt).GetHashCode().ToString();
        Texture2D bitMap = new Texture2D(GridSizeX, GridSizeY);
        for (byte XXX = 0; XXX < GridSizeX; XXX++)
        {
            for (byte YYY = 0; YYY < GridSizeY; YYY++)
            {
                bitMap.SetPixel(XXX, YYY , new Color((1/255f),(1/255f),(1/255f),(1/255f)));
            }
        }
        bitMap.anisoLevel = 0;
        bitMap.filterMode = FilterMode.Bilinear;

        int fillX = (int)Math.Ceiling((float)GridSizeX / 2f);
        int fillY = (int)Math.Ceiling((float)GridSizeY / 2f);
        // DebugLogger.LogDebug($"Filling Circle. {fillX}, {fillY}, {platformSpread}");
        //bitMap.DrawFilledCircle(fillX, fillY, platformSpread, new Color(0.5f,0.5f,0.5f,0.5f));
        if (fillCenter)
        {
            bitMap.DrawFilledCircle((int)Math.Ceiling(fillX * .75f), (int)Math.Ceiling(fillY * .75f), platformSpread, new Color(1,1,1,1));
        }
        else
            bitMap.DrawCircle((int)Math.Ceiling(fillX * .75f), (int)Math.Ceiling(fillY * .75f), platformSpread, new Color(1,1,1,1));
        ushort id = 0;
        
        
        // DebugLogger.LogDebug($"Starting BitProcessor.");
        
        // dont delete these, they are important for debugging.
        //string lineOutput = "";
        //string lineOutput2 = "";
        //string lineOutput3 = "";
        for (byte y = 0; y < GridSizeY; y++)
        {
            //string line = "\n";
            //string line2 = "\n";
            //string line3 = "\n";
            // DebugLogger.LogDebug($"Row {y} complete");
            for (byte x = 0; x < GridSizeX; x++)
            {
                float hue = (hueOffset == -1) ? Random.Range(0f, 1f) : h + Random.Range(-hueOffset, hueOffset);
                float saturation = (saturationOffset == -1) ? Random.Range(0f, 1f) : s + Random.Range(-saturationOffset, saturationOffset);
                float vv = (vOffset == -1) ? Random.Range(0f, 1f) : v + Random.Range(-vOffset, vOffset);
                Color newColor = Color.HSVToRGB(hue, saturation, vv);
                byte r = (byte)(newColor.r * 255);
                byte g = (byte)(newColor.g * 255);
                byte b = (byte)(newColor.b * 255);
                
                bitMap.GetPixel(x, y);
                // chance = System.BitConverter.GetBytes(bitMap.GetPixel(x, y).a)[0];
                byte chance = (byte) (bitMap.GetPixel(x, y).r * 255);
                byte intensity = 0;
                Random.InitState(GetIntFromSeededString(data.Seed, 3, id));
                if(chance > 0)
                    intensity = System.BitConverter.GetBytes(Random.Range(0, chance))[0];
                data.Points.Add(id, new GridPoint(intensity, chance){ R = r, G = g, B = b});
                //line3 += $" [{bitMap.GetPixel(x, y).r:F3}] ";
                //line2 += $" [{chance:000}] ";
                //line += $" [{intensity:000}] ";
                
                id++;
            }
            
            //lineOutput = line + lineOutput;
            //lineOutput2 = line2 + lineOutput2;
            //lineOutput3 = line3 + lineOutput3;
        }
        //DebugLogger.LogDebug($"BaseChances: \n{lineOutput3}");
        //DebugLogger.LogDebug($"Chances: \n{lineOutput2}");
        //DebugLogger.LogDebug($"Chances Intensities: \n{lineOutput}");
        // DebugLogger.LogDebug($"Processing Completed.")
        var points = data.SafePoints;
        foreach (var point in points)
        {
            var gridPoint = point.Value;
            gridPoint.R = (byte)(baseColor.Value.r * 255);
            gridPoint.R = (byte)(baseColor.Value.g * 255);
            gridPoint.B = (byte)(baseColor.Value.b * 255);
            data.Points[point.Key] = gridPoint;
        }

        // DebugLogger.LogDebug($"Adding Data.");
        this.GridData.Add(data);
        // DebugLogger.LogDebug($"Logging Output.");
        _logOutput(data);
        return data;
    }
    
    

    private void _logOutput(GridData data)
    {
        // DebugLogger.LogDebug($"Logging Debug Info. {this.GridSizeX}x{this.GridSizeY} ({data.SelectedPointId})");
        string output = "";
        ushort id = 0;
        try
        {
            for (byte y = 0; y < this.GridSizeY; y++)
            {
                string line = "\n";
                for (byte x = 0; x < this.GridSizeX; x++)
                {
                    if (data.SelectedPointId.Contains(id))
                        line += $" [X] ";
                    else
                        line += $" [=] ";
                    id++;
                }

                output = line + output;
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Caught an exception at output logging. \n{e}");
        }

        DebugLogger.LogDebug($"Output: \n{output}");
        /*foreach (var platform in PlatformData.OrderByDescending(x => x.Placement))
        {
            DebugLogger.LogDebug(
                (platform.LeftSideIsDangerous ? "[X] [=]" : "[=] [X]") + $"  Priority: {platform.Placement}",
                LogLevel.Debug, false);
        }*/

    }
    
    public static int GetIntFromSeededString(string seed, int count, int amount)
    {
        string seedGen = "";
        for (int s = 0; s < count; s++)
        {
            int indexer = (amount * count) + s;
            while (indexer >= seed.Length)
                indexer -= seed.Length - 1;
            seedGen += seed[indexer].ToString();
        }

        return int.Parse(seedGen);
    }
}