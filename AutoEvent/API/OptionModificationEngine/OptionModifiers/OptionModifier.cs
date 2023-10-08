// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         OptionModifier.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/16/2023 3:15 PM
//    Created Date:     09/16/2023 3:15 PM
// -----------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Discord;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using YmlNavigator;

namespace AutoEvent.API.OptionModificationEngine.OptionModifiers;

public class OptionModifier
{
    public static OptionModifier GetOrCreateOptionModifer<T>(Type type, object startValue) where T : OptionModifier
    {
        var modifier = OptionModifier.Instances.FirstOrDefault(x => x._workingValue == startValue);

        if (modifier is not null)
        {
            return modifier;
        }

        //return (OptionModifier)default(T);
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;// | BindingFlags.IgnoreReturn;// | BindingFlags.Public;
        CultureInfo culture = null;
        object result = null;
        try
        {

            result = Activator.CreateInstance(type: typeof(T), bindingAttr: bindingFlags, culture: culture,
                binder: null, args: new object[] { type, startValue });
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"{e}", LogLevel.Debug, true);
            goto Null;
        }

        if (result is null)
            goto Null;
        
        return result as OptionModifier;
        Null:
            DebugLogger.LogDebug($"Could not find default constructor.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"Could not find default constructor. T: {typeof(T).Name}", LogLevel.Debug);
            return null;
    }

    protected OptionModifier(Type type, object startValue)
    {
        Instances.Add(this);
        ObjectType = type;
        _workingValue = startValue;
    }

    internal string PropertyName { get; set; }

    public Type GetObjectModifierType(PropertyInfo propertyInfo) =>
        GetObjectModifierType(propertyInfo.PropertyType);

    public Type GetObjectModifierType(object obj) => GetObjectModifierType(obj.GetType());

    public object GetObjectFromString(string input)
    {
        switch (GetConfigOptionValueType(this.ObjectType))
        {
            case ValueType.Complex or ValueType.Dict or ValueType.List:
                return ComplexDeserialize(input);
            case ValueType.Convertible:
                return Convert.ChangeType(input, this.ObjectType);
            case ValueType.Enum:
                return EnumDeserialize(input);
        }

        return null;
    }
    public Type GetObjectModifierType(Type type)
    {
        switch (GetConfigOptionValueType(type))
        {
            case ValueType.Complex:
                return typeof(ComplexObjectModifier);
            case ValueType.Convertible:
                return typeof(BasicModifier);
            case ValueType.Dict:
                return typeof(DictionaryModifier);
            case ValueType.List:
                return typeof(ListModifier);
            case ValueType.Enum:
                return typeof(EnumModifier);
        }

        throw new NotImplementedException();
    }

    protected static List<OptionModifier> Instances { get; set; } = new List<OptionModifier>();
    protected Type ObjectType { get; set; }

    /// <summary>
    /// A reference to the object this option is located in.
    /// </summary>
    protected object ReferenceObject { get; set; }

    /// <summary>
    /// A local instance of the value.
    /// </summary>
    internal object WorkingValue
    {
        get => _workingValue;
        set => _workingValue = value;
    }

    private object _workingValue;

    protected object EnumDeserialize(string data, Type enumTypeOverride = null)
    {
        return Enum.Parse(enumTypeOverride ?? ObjectType, data, true);
    }

    protected object SimpleDeserialize(string data, Type objectTypeOverride = null)
    {
        return Convert.ChangeType(data, objectTypeOverride ?? ObjectType);
    }

    protected object ComplexDeserialize(string data, Type objectTypeOverride = null)
    {
        throw new NotImplementedException("Complex Deserialize has yet to be implemented.");
    }

    public static ValueType GetConfigOptionValueType(Type type)
    {
        if (type.GetInterface(nameof(IConvertible)) is not null)
            return ValueType.Convertible;
        if (type.GetInterface(nameof(IDictionary)) is not null)
            return ValueType.Dict;
        if (type.GetInterface(nameof(IList)) is not null)
            return ValueType.List;
        if (type.IsEnum)
            return ValueType.Enum;
        return ValueType.Complex;
    }
}
        