// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GridData.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 2:58 PM
//    Created Date:     10/16/2023 2:58 PM
// -----------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Games.Puzzle;

public struct GridData
{
    public GridData(byte gridSizeX, byte gridSizeY)
    {
        GridSizeX = gridSizeX;
        GridSizeY = gridSizeY;
    }
    private byte GridSizeX { get; init; }
    private byte GridSizeY { get; init; }
    public string Seed { get; internal set; } = "";
    public Dictionary<ushort, GridPoint> Points { get; set; } = new Dictionary<ushort, GridPoint>();
    public List<KeyValuePair<ushort, GridPoint>> SafePoints => Points.OrderByDescending(pnt => pnt.Value.Intensity).Take(SafePointCount).ToList();
    public int SafePointCount { get; set; } = 1;
    public List<ushort> SelectedPointId => SafePoints.Select(x => x.Key).ToList();
    /* Grid Size X = 5
     * Grid Size Y = 5
     * 
     * 5 | 20 21 22 23 24 
     * 4 | 15 16 17 18 19 
     * 3 | 10 11 12 13 14 
     * 2 | 05 06 07 08 09
     * 1 | 00 01 02 03 04 
     *---|---------------
     * X | 1  2  3  4  5 
     
     * selected 18
     * (4, 3)
     */

    /*
    public static void Testing()
    {
        int gridSizeX = 5;
        int gridSizeY = 5;
        
        int selected =  9;
        
        int selectedY = (int)((float)selected / gridSizeX)+1;
        int selectedX = selected % gridSizeY;
        string selectedXY = "";
        string output = "\n---|---------------\n X | 1  2  3  4  5 ";
        for(int y = 0; y < gridSizeY; y++){
            string tempOut = "";
            tempOut += $"\n {y+1} |";
            if(selected == (gridSizeX * y) )
              tempOut += "[";
            else
              tempOut += " ";
            for (int x = 0; x < gridSizeX; x++){
                int number = y * gridSizeX + x;
                if(number == selected - 1 && (x+1) % gridSizeX != 0){
                    tempOut += $"{number:00}[";
                }
                else if(number == selected){
                    tempOut +=$"{number:00}]";
                    selectedXY = $"({x},{y+1})";
                }
                else
                    tempOut += $"{number:00} ";
            }
            output = $"{tempOut}" + output;
            // output += "\n";
        }
        Console.WriteLine ($"Selected: {selected} ({selectedX},{selectedY})\n{output}");

        Console.WriteLine ($"Selected: {selected} {selectedXY}");
        Console.WriteLine ($"Selected: {selected} ({selectedX},{selectedY})");
    }
     */
}