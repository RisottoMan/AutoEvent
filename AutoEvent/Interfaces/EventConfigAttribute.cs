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
using YamlDotNet.Core;

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
        object conf = null;
        try
        {
            if (File.Exists(configPath))
            {
                conf = Configs.Serialization.Deserializer.Deserialize(File.ReadAllText(configPath), type);
            }

            if (!(conf is null or not EventConfig))
            {
                //_isLoaded = true;
                //return conf;
            }
            DebugLogger.LogDebug("Config was not serialized into an event config. It will be deleted and remade.");
            
        }
        catch (YamlException e)
        {
            DebugLogger.LogDebug("Caught a bad config.");
            // probably an updated config. We will just replace it.
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Caught an exception loading a config.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }

        if (File.Exists(configPath))
        {
            try
            {
                File.Delete(configPath);
            }
            catch (Exception e)
            {
                //
            }
        }

        // Check version plugin and config

        CreateNewConfig(ref conf, type, configPath);
        _isLoaded = true;
        return conf;
    }

    private void CreateNewConfig(ref object conf, Type type, string configPath)
    {
        conf = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
        if (conf is null)
        {
            DebugLogger.LogDebug("Conefig is null.", LogLevel.Debug);
        }
        File.WriteAllText(configPath, Configs.Serialization.Serializer.Serialize(conf));
        _isLoaded = true;
    }

    public bool IsLoaded => _isLoaded;

    protected bool _isLoaded = false;

}