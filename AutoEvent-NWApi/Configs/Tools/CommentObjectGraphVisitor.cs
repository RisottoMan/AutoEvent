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

using System;
using System.Collections;
using System.Reflection;
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
        try
        {
            var ignoreAttribute = key.GetCustomAttribute<YamlIgnoreAttribute>();
            var propertyIgnoreAttribute = value.Type.GetCustomAttribute<YamlIgnoreAttribute>();

            if (ignoreAttribute is not null)
                return false;
            if (propertyIgnoreAttribute is not null)
                return false;
            var memberAttribute = key.GetCustomAttribute<YamlMemberAttribute>();
            var propertyMemberAttribute = key.GetCustomAttribute<YamlMemberAttribute>();
            DefaultValuesHandling handling = (DefaultValuesHandling)0;
            if (memberAttribute is not null && memberAttribute.IsDefaultValuesHandlingSpecified)
            {
                handling |= memberAttribute.DefaultValuesHandling;
            }

            if (propertyMemberAttribute is not null && propertyMemberAttribute.IsDefaultValuesHandlingSpecified)
            {
                handling |= propertyMemberAttribute.DefaultValuesHandling;
            }

            if (handling == 0)
                goto SkipDefaultsCheck;

            if (handling.HasFlag(DefaultValuesHandling.OmitDefaults))
            {
                var defaultValue = value.Type.IsValueType ? Activator.CreateInstance(value.Type) : null;
                if (Equals(value.Value, defaultValue))
                    return false;
            }

            if (handling.HasFlag(DefaultValuesHandling.OmitNull))
            {
                if (Equals(value.Value, null))
                    return false;
            }

            if (handling.HasFlag(DefaultValuesHandling.OmitEmptyCollections))
            {
                if (value.Value is ICollection { Count: 0 })
                    return false;
            }

        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Yaml error caught. E: \n {e}");
        }
        SkipDefaultsCheck:

        if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment is not null)
        {
            context.Emit(new Comment(commentsDescriptor.Comment, false));
        }

        return base.EnterMapping(key, value, context);
    }
}
