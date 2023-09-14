// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         CommentGatheringTypeInspector.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 2:11 PM
//    Created Date:     09/11/2023 2:11 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace AutoEvent.Configs.Tools;

/// <summary>
/// Spurce: https://dotnetfiddle.net/8M6iIE.
/// </summary>
public sealed class CommentGatheringTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector innerTypeDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentGatheringTypeInspector"/> class.
    /// </summary>
    /// <param name="innerTypeDescriptor">The inner type description instance.</param>
    public CommentGatheringTypeInspector(ITypeInspector innerTypeDescriptor)
    {
        this.innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException("innerTypeDescriptor");
    }

    /// <inheritdoc/>
    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
    {
        return innerTypeDescriptor
            .GetProperties(type, container)
            .Select(descriptor => new CommentsPropertyDescriptor(descriptor));
    }
}