// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         Log.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 4:01 PM
//    Created Date:     10/27/2023 4:01 PM
// -----------------------------------------

namespace InventoryMenu.API;

public class Log
{
    /// <summary>
    /// Invoked on logging. Hook this to receive logs from the api.
    /// </summary>
    public static event Action<string, LogLevel> OnLog;

    /// <summary>
    /// Used to log information.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="logLevel">The <see cref="LogLevel"/> of the log.</param>
    internal static void Message(string message, LogLevel logLevel)
    {
        OnLog?.Invoke(message, logLevel);
        return;
        switch (logLevel)
        {
            case LogLevel.Debug:
                PluginAPI.Core.Log.Debug(message, "Inventory Menus");
                break;
            case LogLevel.Info:
                PluginAPI.Core.Log.Info(message, "Inventory Menus"); 
                break;
            case LogLevel.Warn:
                PluginAPI.Core.Log.Warning(message, "Inventory Menus"); 
                break;
            case LogLevel.Error:
                PluginAPI.Core.Log.Error(message, "Inventory Menus");
                break;
        }
    }

    internal static void Debug(string message) => Message(message, LogLevel.Debug);
    internal static void Warn (string message) => Message(message, LogLevel.Warn);
    internal static void Error(string message) => Message(message, LogLevel.Error);
    internal static void Info (string message) => Message(message, LogLevel.Info);
    
    public enum LogLevel
    {
        /// <summary>
        /// Debugging information. Users typically don't need to see this.
        /// </summary>
        Debug = 0,
        
        /// <summary>
        /// Non-Critical Errors.
        /// </summary>
        Warn = 1,
        
        /// <summary>
        /// Critical Errors.
        /// </summary>
        Error = 2,
        
        /// <summary>
        /// General API Information.
        /// </summary>
        Info = 3
    }
}