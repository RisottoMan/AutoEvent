// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Serializer.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 1:45 PM
//    Created Date:     09/11/2023 1:45 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoEvent.Configs.Converters;
using AutoEvent.Configs.Tools;
using UnityEngine;
using UnityEngine.Pool;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace AutoEvent.Configs;

public static class Serialization
{
  /// <summary>
  /// Gets or sets the serializer for configs and translations.
  /// </summary>
  public static ISerializer Serializer { get; set; } = new SerializerBuilder()
    .WithTypeConverter(new VectorsConverter())
    .WithTypeConverter(new ColorConverter())
    .WithTypeConverter(new AttachmentIdentifiersConverter())
    .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
    .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
    .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .IgnoreFields()
    .DisableAliases()
    .Build();

  /// <summary>
  /// Gets or sets the deserializer for configs and translations.
  /// </summary>
  public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
    .WithTypeConverter(new VectorsConverter())
    .WithTypeConverter(new ColorConverter())
    .WithTypeConverter(new AttachmentIdentifiersConverter())
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
    .IgnoreFields()
    .IgnoreUnmatchedProperties()
    .Build();
  /// <summary>
  /// Gets or sets the quotes wrapper type.
  /// </summary>
  [Description("Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
  public static ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;
  /// <summary>
  /// Gets or sets the quotes wrapper type.
  /// </summary>
  [Description("Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
  public static ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;
  /// <summary>
  /// Converts a <see cref="string"/> to snake_case convention.
  /// </summary>
  /// <param name="str">The string to be converted.</param>
  /// <param name="shouldReplaceSpecialChars">Indicates whether special chars has to be replaced or not.</param>
  /// <returns>Returns the new snake_case string.</returns>
  public static string ToSnakeCase(this string str, bool shouldReplaceSpecialChars = true)
  {
    string snakeCaseString = string.Concat(str.Select((ch, i) => (i > 0) && char.IsUpper(ch) ? "_" + ch.ToString() : ch.ToString())).ToLower();

    return shouldReplaceSpecialChars ? Regex.Replace(snakeCaseString, @"[^0-9a-zA-Z_]+", string.Empty) : snakeCaseString;
  }
}
  