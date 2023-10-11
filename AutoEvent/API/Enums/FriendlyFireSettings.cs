// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         FriendlyFireSettings.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/27/2023 12:18 AM
//    Created Date:     09/27/2023 12:18 AM
// -----------------------------------------

using System.ComponentModel;

namespace AutoEvent.API.Enums;

public enum FriendlyFireSettings
{
    [Description("Enables Friendly Fire / Autoban")]
    Enable,
    [Description("Disables Friendly Fire / Autoban")]
    Disable,
    [Description("Uses the server default setting for Friendly Fire / Autoban")]
    Default
}