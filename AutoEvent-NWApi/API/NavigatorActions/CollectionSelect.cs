// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         YmlNavigator
//    Project:          YmlNavigator
//    FileName:         CollectionSelect.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/15/2023 8:49 PM
//    Created Date:     09/15/2023 8:49 PM
// -----------------------------------------

namespace YmlNavigator
{

    public class CollectionSelect : NavigatorAction
    {
        public CollectionSelect(string input)
        {
            _input = input;
        }

        public string CollectionName { get; set; }
        public bool IsDictionary { get; set; }
        public string Key { get; set; }
        private string _input;
    }
}