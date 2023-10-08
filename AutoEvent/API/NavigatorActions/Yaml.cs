// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         YmlNavigator
//    Project:          YmlNavigator
//    FileName:         Yaml.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/15/2023 8:46 PM
//    Created Date:     09/15/2023 8:46 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AutoEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace YmlNavigator
{
    public partial class ConfigNavigator
    {
        private static object _getJsonObject(string item)
        {
            foreach (Match match in new Regex(@"\{(.|\s)*\}").Matches(item))
            {
                try
                {

                    try
                    {
                        object _ = JsonConvert.DeserializeObject(match.Value);
                        return new Object() { Data = item };
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine($"Skipping. ");
                        continue;
                    }

                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Json");
                    //
                }
            }

            return null;
        }

        public static List<NavigatorAction> Navigate(string input)
        {
            DateTime now = DateTime.Now;
            Dictionary<int, int> IgnoreIndex = new Dictionary<int, int>();
            List<int> Index2 = new List<int>();
            Dictionary<int, NavigatorAction> actions = new Dictionary<int, NavigatorAction>();

            // Example -> Preset1 -> PlayerClasses + <33, { "RoleTypeId": "ClassD", "Health": 125, "Items": [ "Medkit", "PainKillers", "GunAK" ] }>
            // Check for KVP
            foreach (Match match in new Regex(
                             "(?:(?:[<]?)([\\da-zA-Z!\\s{}\\[\\]\"'.,:]+)(?:(?![^{]*}),)([\\da-zA-Z!\\s{}\\[\\]\"'.,:]+)[>]?)")
                         .Matches(input))
            {
                try
                {

                    if (IgnoreIndex.Any(x => x.Key == match.Index)) //&& match.Index <= x.Key + x.Value))
                    {
                        continue;
                    }

                    if (match.Groups.Count < 3)
                    {
                        continue;
                    }

                    var kvp = new KeyValuePair();
                    var key = _getJsonObject(match.Groups[1].Value);
                    var value = _getJsonObject(match.Groups[2].Value);
                    kvp.Key = key is null
                        ? new Option(match.Groups[1].Value)
                        : (NavigatorAction)new Object() { Data = match.Groups[1].Value };
                    kvp.Value = value is null
                        ? new Option(match.Groups[2].Value)
                        : (NavigatorAction)new Object() { Data = match.Groups[2].Value };


                    IgnoreIndex.Add(match.Index, match.Length);
                    Index2.Add(match.Index);
                    Index2.Add(match.Index + match.Length);
                    actions.Add(match.Index, kvp);
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Key Value Pairs");
                    //
                }
            }

            // Check for Objects
            foreach (Match match in new Regex(@"[\{\[](.|\s)*[\}\]]").Matches(input))
            {
                try
                {
                    if (IgnoreIndex.Any(x => x.Key <= match.Index && match.Index <= x.Key + x.Value))
                    {
                        continue;
                    }

                    try
                    {
                        object _ = JsonConvert.DeserializeObject(match.Value);
                    }
                    catch (Exception e)
                    {
                        DebugLogger.LogDebug($"Navigator Json Ignore: \n{e}", LogLevel.Debug);
                        continue;
                    }

                    int len = match.Value.Length;
                    IgnoreIndex.Add(match.Index, len);
                    Index2.Add(match.Index);
                    Index2.Add(match.Index + len);
                    actions.Add(match.Index, new Object() { Data = match.Value });
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"Caught an Exception while Processing Json at navigator", LogLevel.Debug);
                }
            }


            // Check for moves
            foreach (Match match in new Regex("->").Matches(input))
            {
                try
                {
                    if (IgnoreIndex.Any(x => x.Key <= match.Index && match.Index <= x.Key + x.Value))
                    {
                        continue;
                    }

                    IgnoreIndex.Add(match.Index, match.Length);
                    Index2.Add(match.Index);
                    Index2.Add(match.Index + match.Length);
                    actions.Add(match.Index, new MoveAction());
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Navigations");
                    // 
                }
            }

            // Check for value setters.
            foreach (Match match in new Regex("=").Matches(input))
            {
                try
                {
                    if (IgnoreIndex.Any(x => x.Key <= match.Index && match.Index <= x.Key + x.Value))
                    {
                        continue;
                    }

                    IgnoreIndex.Add(match.Index, match.Length);
                    Index2.Add(match.Index);
                    Index2.Add(match.Index + match.Length);
                    actions.Add(match.Index, new ModifyAction());
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Modifications");
                    // 
                }
            }

            // Check for entry adds.
            foreach (Match match in new Regex("[+]").Matches(input))
            {
                try
                {

                    if (IgnoreIndex.Any(x => x.Key <= match.Index && match.Index <= x.Key + x.Value))
                    {
                        continue;
                    }

                    Index2.Add(match.Index);
                    Index2.Add(match.Index + match.Length);
                    IgnoreIndex.Add(match.Index, 2);
                    actions.Add(match.Index, new AddAction());
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Entry Additions");
                    //
                }
            }

            // Check for entry removals 
            foreach (Match match in new Regex("[-]").Matches(input))
            {
                try
                {

                    if (IgnoreIndex.Any(x => x.Key <= match.Index && match.Index <= x.Key + x.Value))
                    {
                        continue;
                    }

                    IgnoreIndex.Add(match.Index, match.Length);
                    Index2.Add(match.Index);
                    Index2.Add(match.Index + match.Length);
                    actions.Add(match.Index, new RemoveAction());
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Entry Removals");
                    //
                }
            }
            // Check for anything else

            for (int i = 0; i < Index2.Count; i += 2)
            {
                try
                {

                    bool isUpperBoundary = i % 2 != 0;
                    if (isUpperBoundary)
                    {
                        i++;
                        continue;
                    }

                    var orderedList = Index2.OrderBy(x => x).ToList();
                    int curNumber = orderedList[i];
                    int prevNumber = i - 1 >= 0 ? orderedList.ToList()[i - 1] : -1;
                    int nextNumber = i + 1 < Index2.Count ? orderedList.ToList()[i + 1] : -1;
                    string val = "";
                    if (prevNumber == -1)
                    {
                        val = input.Substring(0, curNumber);
                    }
                    else
                    {
                        val = input.Substring(prevNumber, curNumber - prevNumber);
                    }

                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }

                    actions.Add(prevNumber == -1 ? 0 : prevNumber + 0, new Option(val.Trim()));
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Caught an Exception while Processing Objects");
                    //
                }
            }

            try
            {
                // Example -> Preset1 -> PlayerClasses + <33,{ "RoleTypeId": "ClassD", "Health": 125, "Items": [ "Medkit", "PainKillers", "GunAK" ]}>
                int lastIndex = Index2.OrderBy(x => x).Last();
                if (!(lastIndex >= input.Length))
                {
                    string val = input.Substring(lastIndex, input.Length - lastIndex);
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        actions.Add(lastIndex + 2, new Option(val));
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Caught Exception while Processing the Last Object");
                //Console.WriteLine($"{e}");
            }

            // Order the actions
            List<KeyValuePair<int, NavigatorAction>> order = new List<KeyValuePair<int, NavigatorAction>>();
            foreach (var x in actions.OrderBy(x => x.Key))
            {
                order.Add(x);
            }

            List<NavigatorAction> output = new List<NavigatorAction>();
            for (int i = 0; i < order.Count; i++)
            {
                try
                {

                    var action = order[i];
                    KeyValuePair<int, NavigatorAction>? nextObject = null;
                    if (i + 1 < actions.Count)
                    {
                        nextObject = order[i + 1];
                    }

                    KeyValuePair<int, NavigatorAction>? prevObject = null;
                    if (i - 1 >= 0)
                    {
                        prevObject = order[i - 1];
                    }

                    //string obj = $"[{action.Value.GetType().Name}";
                    //Console.WriteLine(obj.PadLeft(15) + $"] {action.Value.ToString()}");

                    action.Value.To = nextObject?.Value;
                    action.Value.From = prevObject?.Value;
                    switch (action.Value)
                    {
                        case MoveAction move:
                            break;
                        case AddAction add:
                            break;
                        case RemoveAction subtract:
                            break;
                        case ModifyAction modify:
                            break;
                        case KeyValuePair kvp:
                            // Console.WriteLine($"  [{kvp.Key.GetType().Name}]".PadLeft(19) + $" {kvp.Key.Importance}");
                            // Console.WriteLine($"  [{kvp.Value.GetType().Name}]".PadLeft(19) + $" {kvp.Value.Importance}");
                            break;
                        case Object obj:
                            break;
                        case Option option:
                            break;
                    }

                    output.Add(action.Value);
                }
                catch (Exception e)
                {
                    //
                }
            }

            TimeSpan duration = DateTime.Now.Subtract(now);
            // Console.WriteLine($"Query took {duration.TotalSeconds:00}.{duration.TotalMilliseconds:000} seconds");
            return output;
        }
    }
}
// Example -> Preset1 -> PlayerClasses + <33,{ "RoleTypeId": "ClassD", "Health": 125, "Items": [ "Medkit", "PainKillers", "GunAK" ]}>
