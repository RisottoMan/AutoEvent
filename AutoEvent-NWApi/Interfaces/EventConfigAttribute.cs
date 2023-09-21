// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 12:36 PM
//    Created Date:     09/13/2023 12:36 PM
// -----------------------------------------

using System;
using System.ComponentModel;
using System.IO;

namespace AutoEvent.Interfaces;

[AttributeUsage(AttributeTargets.Property)]
public class EventConfigAttribute : Attribute
{
    public EventConfigAttribute()
    {
        
    }
    
    public virtual object Load(string folderPath, string configName, Type type)
    {
        string configPath = Path.Combine(folderPath, configName + ".yml");
        object conf;
        if (!File.Exists(configPath))
        {
            conf = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
            if (conf is null)
            {
                DebugLogger.LogDebug("Config is null.", LogLevel.Debug);
            }
            File.WriteAllText(configPath, Configs.Serialization.Serializer.Serialize(conf));
            _isLoaded = true;
            return conf;
        }
        conf = Configs.Serialization.Deserializer.Deserialize(File.ReadAllText(configPath), type);
        _isLoaded = true;
        return conf;
    }

    public bool IsLoaded => _isLoaded;

    protected bool _isLoaded = false;

}