// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         YmlNavigator
//    Project:          YmlNavigator
//    FileName:         Yaml.ConfigNavigator.NavigatorAction.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/15/2023 8:50 PM
//    Created Date:     09/15/2023 8:50 PM
// -----------------------------------------

using AutoEvent.API.OptionModificationEngine.OptionModifiers;
using JetBrains.Annotations;

namespace YmlNavigator
{
    public class NavigatorAction
    {
        internal virtual string Importance { get; } = "";

        public override string ToString()
        {
            return Importance;
        }

        public NavigatorAction From { get; set; }
        public NavigatorAction To { get; set; }
        [CanBeNull] public OptionModifier Modifier { get; set; }
        public object Property { get; set; }
    }
}