// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         YmlNavigator
//    Project:          YmlNavigator
//    FileName:         Yaml.ConfigNavigator.KeyValuePair.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/15/2023 8:48 PM
//    Created Date:     09/15/2023 8:48 PM
// -----------------------------------------

namespace YmlNavigator
{
    public class KeyValuePair : NavigatorAction
    {
        public NavigatorAction Key { get; set; }
        public NavigatorAction Value { get; set; }
        internal override string Importance => $"";

    }
}