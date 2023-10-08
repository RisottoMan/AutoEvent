// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EnumModifier.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/16/2023 3:16 PM
//    Created Date:     09/16/2023 3:16 PM
// -----------------------------------------

using System;

namespace AutoEvent.API.OptionModificationEngine.OptionModifiers;

public class EnumModifier : OptionModifier
{
    internal EnumModifier(Type type, object startValue) : base(type, startValue)
    {
    }

    public void SetValue(string data)
    {
        base.WorkingValue = base.EnumDeserialize(data);
    }
}