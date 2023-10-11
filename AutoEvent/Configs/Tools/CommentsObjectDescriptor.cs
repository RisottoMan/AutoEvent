// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         CommentsObjectDescriptor.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/21/2023 10:21 AM
//    Created Date:     09/21/2023 10:21 AM
// -----------------------------------------
using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AutoEvent.Configs.Tools;


/// <summary>
/// Source: https://dotnetfiddle.net/8M6iIE.
/// </summary>
public sealed class CommentsObjectDescriptor : IObjectDescriptor
{
    private readonly IObjectDescriptor innerDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsObjectDescriptor"/> class.
    /// </summary>
    /// <param name="innerDescriptor">The inner descriptor instance.</param>
    /// <param name="comment">The comment to be written.</param>
    public CommentsObjectDescriptor(IObjectDescriptor innerDescriptor, string comment)
    {
        this.innerDescriptor = innerDescriptor;
        Comment = comment;
    }

    /// <summary>
    /// Gets the comment to be written.
    /// </summary>
    public string Comment { get; private set; }

    /// <inheritdoc cref="IObjectDescriptor" />
    public object Value => innerDescriptor.Value;

    /// <inheritdoc cref="IObjectDescriptor" />
    public Type Type => innerDescriptor.Type;

    /// <inheritdoc cref="IObjectDescriptor" />
    public Type StaticType => innerDescriptor.StaticType;

    /// <inheritdoc cref="IObjectDescriptor" />
    public ScalarStyle ScalarStyle => innerDescriptor.ScalarStyle;
}