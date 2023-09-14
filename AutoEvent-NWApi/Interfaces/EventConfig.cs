// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 3:32 PM
//    Created Date:     09/13/2023 3:32 PM
// -----------------------------------------

using System.ComponentModel;
using YamlDotNet.Serialization;

namespace AutoEvent.Interfaces;

public class EventConfig
{
    public EventConfig()
    {
        
    }
    [YamlIgnore]
    public string PresetName { get; set; }
    [Description("Should this plugin output debug logs.")]
    public bool Debug { get; set; }
}