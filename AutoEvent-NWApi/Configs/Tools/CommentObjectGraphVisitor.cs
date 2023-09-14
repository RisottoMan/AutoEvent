// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         CommentObjectGraphVisitor.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/11/2023 2:12 PM
//    Created Date:     09/11/2023 2:12 PM
// -----------------------------------------

using Serialization;

namespace AutoEvent.Configs.Tools;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

/// <summary>
/// Source: https://dotnetfiddle.net/8M6iIE.
/// </summary>
public sealed class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsObjectGraphVisitor"/> class.
    /// </summary>
    /// <param name="nextVisitor">The next visitor instance.</param>
    public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
        : base(nextVisitor)
    {
    }

    /// <inheritdoc/>
    public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
    {
        if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment is not null)
        {
            context.Emit(new Comment(commentsDescriptor.Comment, false));
        }

        return base.EnterMapping(key, value, context);
    }
}