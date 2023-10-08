// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventConfigPresetAttribute.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 1:10 PM
//    Created Date:     09/13/2023 1:10 PM
// -----------------------------------------

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace AutoEvent.Interfaces;

public class EventConfigPresetAttribute : EventConfigAttribute
{
    public void Load(string folderPath, PropertyInfo property, object value)
    {
        if (_isLoaded)
        {
            return;
        }
        // Configs/Event/Presets/PresetName.yml
        string configPath = Path.Combine(folderPath, property.Name + ".yml");
        if (!File.Exists(configPath))
        {
            if (value is null)
            {
                DebugLogger.LogDebug("Preset is null.", LogLevel.Debug);
            }
            File.WriteAllText(configPath, Configs.Serialization.Serializer.Serialize(value));
            
            base._isLoaded = true;
        }
        
    }
}