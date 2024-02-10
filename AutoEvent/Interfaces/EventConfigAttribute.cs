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
using Version = System.Version;

namespace AutoEvent.Interfaces;

[AttributeUsage(AttributeTargets.Property)]
public class EventConfigAttribute : Attribute
{
    public EventConfigAttribute()
    {
        
    }
    
    public virtual object Load(string folderPath, string configName, Type type, Version version)
    {
        string configPath = Path.Combine(folderPath, configName + ".yml");
        object conf = null;
        try
        {
            if (File.Exists(configPath))
            {
                conf = Configs.Serialization.Deserializer.Deserialize(File.ReadAllText(configPath), type);
            }

            if (conf is not null and EventConfig config)
            {
                if (config.ConfigVersion == version.ToString())
                {
                    _isLoaded = true;
                    _isCorrected = true;
                    return conf;
                }
                else DebugLogger.LogDebug($"The config version and the plugin version are not equal. It will be deleted and remade.");
            }
            else DebugLogger.LogDebug("Config was not serialized into an event config. It will be deleted and remade.");
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
            catch (Exception e) { }
        }

        CreateNewConfig(ref conf, type, configPath, version);
        _isLoaded = true;
        return conf;
    }

    private void CreateNewConfig(ref object conf, Type type, string configPath, Version version)
    {
        conf = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
        if (conf is null)
        {
            DebugLogger.LogDebug("Config is null.", LogLevel.Debug);
        }
        
        if (conf is EventConfig evConf)
        {
            evConf.ConfigVersion = version.ToString();
        }

        File.WriteAllText(configPath, Configs.Serialization.Serializer.Serialize(conf));
        _isLoaded = true;
    }

    // Is the config version correct?
    public bool IsCorrected => _isCorrected;
    protected bool _isCorrected = false;

    // Is the config loaded?
    public bool IsLoaded => _isLoaded;
    protected bool _isLoaded = false;
}