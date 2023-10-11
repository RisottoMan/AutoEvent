// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ListModifier.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/16/2023 3:16 PM
//    Created Date:     09/16/2023 3:16 PM
// -----------------------------------------

using System;
using System.Collections;

namespace AutoEvent.API.OptionModificationEngine.OptionModifiers;

public class ListModifier : OptionModifier
{
    private IList List
    {
        get => (IList)base.WorkingValue;
        set => WorkingValue = value;
    }
    public Type KeyType { get; set; }
    internal ListModifier(Type type, object startValue) : base(type, startValue)
    {
        KeyType = type.GetGenericArguments()[0];

    }

    public virtual void RemoveEntry(object key)
    {
        List.Remove(key);
    }

    public virtual void ModifyEntry(object key, object newValue)
    {
        if (key is not int)
        {
            throw new ArgumentException("The key must be an integer for lists.");
        }

        List[(int)key] = newValue;
    }

    public virtual void AddEntry(object newValue)
    {
        List.Add(newValue);
    }

    public void SetCollectionValue(object newValue)
    {
        if (newValue is not IList)
        {
            throw new ArgumentException("The new value must be a list.");
        }

        List = (IList)newValue;
    }

    public void SetCollectionValue(string newValue) => SetCollectionValue(base.ComplexDeserialize(newValue));

}