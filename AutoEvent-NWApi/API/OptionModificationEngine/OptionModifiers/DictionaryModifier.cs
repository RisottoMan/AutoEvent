// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DictionaryModifier.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/16/2023 3:17 PM
//    Created Date:     09/16/2023 3:17 PM
// -----------------------------------------

using System;
using System.Collections;

namespace AutoEvent.API.OptionModificationEngine.OptionModifiers;

public class DictionaryModifier : ListModifier
{
    public Type ValueType { get; set; }

    private IDictionary Dictionary
    {
        get => (IDictionary)base.WorkingValue;
        set => WorkingValue = value;
    }

    internal DictionaryModifier(Type type, object startValue) : base(type, startValue)
    {
        if (base.WorkingValue is not IDictionary)
        {
            throw new ArgumentException("Object must be a dictionary to use the dictionary modifier.");
        }
        ValueType = type.GetGenericArguments()[1];
    }

    public override void RemoveEntry(object key)
    {
        Dictionary.Remove(key);
    }

    public override void ModifyEntry(object key, object newValue)
    {
        Dictionary[key] = newValue;
    }

    public override void AddEntry(object newValue)
    {
        if (newValue is DictionaryEntry dictionaryEntry)
        {
            Dictionary.Add(dictionaryEntry.Key, dictionaryEntry.Value);
            return;
        }

        throw new ArgumentException("Value must be a Dictionary Entry for dictionary entries.");
    }

    public void AddEntry(object newKey, object newValue)
    {
        Dictionary.Add(newKey, newValue);
    }
}