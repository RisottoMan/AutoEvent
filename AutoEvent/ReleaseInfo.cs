// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ReleaseInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/22/2023 9:49 PM
//    Created Date:     10/22/2023 9:49 PM
// -----------------------------------------

using System;

namespace AutoEvent;

public struct ReleaseInfo
{
    public ReleaseInfo(string version, Version semanticVersion, string name, string changelog, DateTime releaseDate)
    {
        Version = version;
        SemanticVersion = semanticVersion;
        Name = name;
        ChangeLog = changelog;
        ReleaseDate = releaseDate;
    }
    public string Name { get; }
    public string Version { get; }
    public Version SemanticVersion { get; }
    public string ChangeLog { get; }
    public DateTime ReleaseDate { get; set; }
}