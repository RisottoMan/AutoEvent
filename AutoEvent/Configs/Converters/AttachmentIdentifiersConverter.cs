// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         AttachmentIdentifiersConverter.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 1:50 PM
//    Created Date:     09/11/2023 1:50 PM
// -----------------------------------------

using System;
using System.IO;
using InventorySystem.Items.Firearms.Attachments;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace AutoEvent.Configs.Converters;
/// <summary>
/// Converts a <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to Yaml configs and vice versa.
/// </summary>
public sealed class AttachmentIdentifiersConverter : IYamlTypeConverter
{
    /// <inheritdoc cref="IYamlTypeConverter" />
    public bool Accepts(Type type) => type == typeof(AttachmentName);

    /// <inheritdoc cref="IYamlTypeConverter" />
    public object ReadYaml(IParser parser, Type type)
    {
        if (!parser.TryConsume(out Scalar scalar) || !AttachmentIdentifier.TryParse(scalar.Value, out AttachmentName name))
            throw new InvalidDataException($"Invalid AttachmentNameTranslation value: {scalar?.Value}.");

        return Enum.Parse(type, name.ToString());
    }

    /// <inheritdoc cref="IYamlTypeConverter" />
    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        AttachmentName name = default;

        if (value is AttachmentName locAttachment)
            name = locAttachment;

        emitter.Emit(new Scalar(name.ToString()));
    }
}

public static class AttachmentIdentifier
{
    /// <summary>
    /// Converts the string representation of a <see cref="T:InventorySystem.Items.Firearms.Attachments.AttachmentName" /> to its <see cref="T:InventorySystem.Items.Firearms.Attachments.AttachmentName" /> equivalent.
    /// A return value indicates whether the conversion is succeeded or failed.
    /// </summary>
    /// <param name="s">The <see cref="T:System.String" /> to convert.</param>
    /// <param name="name">The converted <see cref="T:System.String" />.</param>
    /// <returns><see langword="true" /> if <see cref="T:System.String" /> was converted successfully; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string s, out AttachmentName name)
    {
        name = AttachmentName.None;
        foreach (AttachmentName attachmentName in Enum.GetValues(typeof (AttachmentName)))
        {
            if (attachmentName.ToString() == s)
            {
                name = attachmentName;
                return true;
            }
        }
        return false;
    }
}