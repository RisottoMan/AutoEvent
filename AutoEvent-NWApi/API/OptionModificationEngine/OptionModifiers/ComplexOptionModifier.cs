// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ComplexOptionModifier.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/16/2023 3:17 PM
//    Created Date:     09/16/2023 3:17 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoEvent.API.OptionModificationEngine.OptionModifiers;

public class ComplexObjectModifier : OptionModifier
{
    internal ComplexObjectModifier(Type type, object startValue) : base(type, startValue)
    {
        AvailableOptions = new List<PropertyInfo>();
        foreach (PropertyInfo prop in type.GetProperties())
        {
            if (!prop.CanWrite || !prop.CanRead)
            {
                continue;
            }

            AvailableOptions.Add(prop);
        }
    }

    public List<PropertyInfo> AvailableOptions { get; private set; }

    public void SetOptionValue(string propertyName, object newValue) =>
        SetOptionValue(AvailableOptions.First(x => propertyName == x.Name), newValue);

    public void SetOptionValue(PropertyInfo propertyInfo, object newValue)
    {
        propertyInfo.SetValue(base.ReferenceObject, newValue);
    }

    public OptionModifier GetObjectModifier(string propertyName) =>
        GetObjectModifier(AvailableOptions.First(x => x.Name == propertyName));

    public OptionModifier GetObjectModifier(PropertyInfo propertyInfo)
    {
        var t = GetObjectModifierType(propertyInfo);
        var m = typeof(OptionModifier).GetMethod(nameof(OptionModifier.GetOrCreateOptionModifer))
            ?.MakeGenericMethod(t);
        var res = m.Invoke(null, new[] { propertyInfo.PropertyType, propertyInfo.GetValue(this.WorkingValue) });
        ((OptionModifier)res).PropertyName = propertyInfo.Name;
        return res as OptionModifier;
    }

    public void ApplyOptionToProperty(OptionModifier modifier)
    {
        if (modifier.PropertyName == "")
        {
            throw new ArgumentException("Modifier is not a property modifier.");
        }

        var prop = this.AvailableOptions.First(x => x.Name == modifier.PropertyName);
        prop.SetValue(this.WorkingValue, modifier.WorkingValue);
    }

    public void SetFullValue(string data)
    {
        WorkingValue = ComplexDeserialize(data);
    }
}
