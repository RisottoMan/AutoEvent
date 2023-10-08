// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         YmlNavigator
//    Project:          YmlNavigator
//    FileName:         Option.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/15/2023 8:50 PM
//    Created Date:     09/15/2023 8:50 PM
// -----------------------------------------

namespace YmlNavigator
{
    public class Option : NavigatorAction
    {
        public Option(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
        internal override string Importance => Data;
    }
}