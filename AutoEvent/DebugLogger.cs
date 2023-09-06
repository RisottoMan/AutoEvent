// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DebugLogger.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/05/2023 7:30 PM
//    Created Date:     09/05/2023 7:30 PM
// -----------------------------------------

using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent;

public class DebugLogger
{
    public static DebugLogger Singleton;
    public DebugLogger()
    {
        Singleton = this;
        _debugLogs = new List<string>();
    }

    public static bool Debug = false;
    public static bool AntiEnd = false;
    private List<string> _debugLogs;
    public static void LogDebug(string input, LogLevel level, bool outputIfNotDebug = false)
    {
        Singleton._debugLogs.Add($"[{level.ToString()}] {(!outputIfNotDebug ? "[Hidden]": " ")}" + input);
        if (outputIfNotDebug || AutoEvent.Debug)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Log.Debug(input);
                    break;
                case LogLevel.Error:
                    Log.Error(input);
                    break;
                case LogLevel.Info:
                    Log.Info(input);
                    break;
                case LogLevel.Warn:
                    Log.Warn(input);
                    break;
            }
        }
    }

    public static void WriteOutput(string fileName)
    {
        File.WriteAllLines(fileName, Singleton._debugLogs);
    }
}

public enum LogLevel
{
    Info,
    Debug,
    Warn,
    Error,
}