// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ValidatingNodeDeserializer.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 2:13 PM
//    Created Date:     09/11/2023 2:13 PM
// -----------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using PluginAPI.Core;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AutoEvent.Configs.Tools;

/// <summary>
/// Basic configs validation.
/// </summary>
public sealed class ValidatingNodeDeserializer : INodeDeserializer
{
    private readonly INodeDeserializer nodeDeserializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingNodeDeserializer"/> class.
    /// </summary>
    /// <param name="nodeDeserializer">The node deserializer instance.</param>
    public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
    {
        this.nodeDeserializer = nodeDeserializer;
    }

    /// <inheritdoc cref="INodeDeserializer"/>
    public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer,
        out object value)
    {
        if (nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
        {
            if (value is null)
                Log.Error("Null value");
            Validator.ValidateObject(value, new ValidationContext(value, null, null), true);

            return true;
        }

        return false;

        // Log.Error($"{e}");
        // value = null;
        // return false;
    }
}