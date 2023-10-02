// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         UnderscoredNamingConvention.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 2:09 PM
//    Created Date:     09/11/2023 2:09 PM
// -----------------------------------------

using System.Collections.Generic;
using AutoEvent.API;
using YamlDotNet.Serialization;

namespace AutoEvent.Configs.Tools;

/// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention"/>
public class UnderscoredNamingConvention : INamingConvention
{
    /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance"/>
    public static UnderscoredNamingConvention Instance { get; } = new();

    /// <summary>
    /// Gets the list.
    /// </summary>
    public List<object> Properties { get; } = new();

    /// <inheritdoc/>
    public string Apply(string value)
    {
        string newValue = value.ToSnakeCase();
        Properties.Add(newValue);
        return newValue;
    }
}