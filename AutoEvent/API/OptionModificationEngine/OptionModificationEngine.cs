// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         OptionModificationEngine.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/16/2023 3:14 PM
//    Created Date:     09/16/2023 3:14 PM
// -----------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoEvent.API.OptionModificationEngine.OptionModifiers;
using AutoEvent.Commands.Config;
using AutoEvent.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using YmlNavigator;
using Debug = System.Diagnostics.Debug;
using Object = YmlNavigator.Object;
using ValueType = System.ValueType;

namespace AutoEvent.API.OptionModificationEngine;


public class OptionModificationEngine
{
    // Doesnt Contain first two args - not necessary.
    internal string ArgsCombined { get; private set; }
    internal string[] Args { get; private set; }
    internal string Response { get; private set; }
    private bool _success = false;
    private Event _event;
    private EventConfig _eventConfig;

    internal OptionModificationEngine(string[] args, Event ev, EventConfig conf)
    {
        _event = ev;
        _eventConfig = conf;
        string combined = "";
        List<string> newArgs = new List<string>();
        for (int i = 2; i < args.Length; i++)
        {
            combined += $"{args[i]} ";
            newArgs.Add(args[i]);
        }

        ArgsCombined = combined;
        Args = newArgs.ToArray();
        Response = "";
        _success = false;
    }

    internal bool Process(ref string response)
    {
        _internalProcess();
        response = Response;
        return _success;
    }
    
    /// <summary>
    /// Does the processing for results.
    /// </summary>
    private void _internalProcess()
    {
        // Event, and EventConfig should be non null by now.
        
        
        
        // Set the value of the whole config using the complex value deserializer
        if (Args[0].ToLower() == "set")
        {
            try
            {
                _setCurrentConfigToNewDeserializedConfig();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug("An error has occured while setting a config preset. _setConfigToNewConfig()",
                    LogLevel.Error, true);
                goto skipOut;
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                Response = "An error has occured.";
                _success = false;
                return;
            }

            return;
        }

        skipOut:
        // Change Specific Option
        // Option is null
        var propertyToChange = _eventConfig.GetType().GetProperty(Args[0]);
        if (propertyToChange is null)
        {
            Response =
                $"Could not find option \"{Args[0]}\" (preset {_eventConfig.PresetName}) (event {_event.Name})";
            _success = false;
            return;
        }

        // Create Option Modifier.
        // Ensure the value isn't null. For instance: A null list or dictionary would cause a lot of havoc.
        var value = propertyToChange.GetValue(this._eventConfig);
        if (value is null)
        {
            if (!_initializeNullValueFromDefaultConstructor(propertyToChange, ref value))
            {
                _success = false;
                return;
            }
        }

        // View value of option
        if (Args.Length < 2)
        {
            _respondWithCurrentValueOfConfigOption(value, propertyToChange);
            _success = true;
            return;
        }
        
        
        try
        {
            this._modifyConfigNew();
            return;
        }
        catch (Exception e)
        {
                //
        }

        // im (try)ing something new (except i) d(on)t know how itll go
        // ReSharper disable once EmptyGeneralCatchClause
        try
        {

        }
        catch (Exception e)
        {

        }

        Response = "Not Implemented Yet";
        _success = false;
        return;
    }

    private object _getDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    } 
    /// <summary>
    /// If a value is null, this will instantiate it with the default constructor.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns>False if it can not instantiate it with a default constructor.</returns>
    private bool _initializeNullValueFromDefaultConstructor(PropertyInfo property, ref object value)
    {
        DebugLogger.LogDebug($"Value is null", LogLevel.Debug);
        if (value is null)
        {
            try
            {
                var instance = _getDefaultValue(property.PropertyType);
                property.SetValue(this._eventConfig, instance);
                value = instance;
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug("Could not instantiate a null value.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                Response = "Could not instantiate a null value.";
                return false;
            }

            if (value is null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Completely replaces a config with a new deserialized config based on the arguments provided.
    /// </summary>
    private void _setCurrentConfigToNewDeserializedConfig()
    {
        string inputString = ArgsCombined.Substring(4, ArgsCombined.Length - 5);
        object newConf = null;
        var reg = new Regex(@"\{(.|\s)*\}");
        var matches = reg.Matches(inputString);
        if (matches.Count > 1)
        {
            foreach (Match match in matches)
            {
                try
                {
                    newConf = JsonConvert.DeserializeObject(match.Value, this._eventConfig.GetType());
                    break;
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"Could not validate this json. Skipping \n{e}", LogLevel.Debug);
                }
            }
        }

        if (newConf is null)
        {
            try
            {

                newConf = Configs.Serialization.Deserializer.Deserialize(inputString, this._eventConfig.GetType());
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug("An error has occured while deserializing yaml. At _setConfigToNewConfig()",
                    LogLevel.Error, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                Response = "An error has occured.";
                _success = false;
                return;
            }
        }

        if (newConf is not EventConfig)
        {
            Response = "An error has occured";
            DebugLogger.LogDebug($"New Config is not an Event Config.", LogLevel.Debug);
            _success = false;
            return;
        }

        _eventConfig = newConf as EventConfig;
        Response = $"Successfully set the value of preset \"{_eventConfig?.PresetName}\" (event {_event.Name})";
        _success = true;
    }

    /// <summary>
    /// Triggers a command response with the current value of a config option.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="propertyToChange"></param>
    private void _respondWithCurrentValueOfConfigOption(object value, PropertyInfo propertyToChange)
    {
        /* Option "RoleHealth" (dictionary[roletype, int]) current entries:
             *   - ClassD = 100
             *   - ChaosMarauder = 115
            */
        if (value is IDictionary dict)
        {
            Response =
                $"Option \"{propertyToChange.Name}\" (Dictionary[{propertyToChange.PropertyType.GenericTypeArguments[0]}, {propertyToChange.PropertyType.GenericTypeArguments[1]}]) Current Entries: \n";
            foreach (DictionaryEntry entry in dict)
            {
                Response += $"  - {entry.Key} = {entry.Value}";
            }

            if (dict.Count == 0)
            {
                Response += $"  [Empty]";
            }

            _success = true;
            return;
        }

        /* Option "AvailableRoles" (List[RoleType]) current entries:
         *   1. ClassD
         *   2. ChaosMarauder
        */
        if (value is IList list)
        {
            int i = 1;
            Response =
                $"Option \"{propertyToChange.Name}\" (List[{propertyToChange.PropertyType.GenericTypeArguments[0]}]) Current Entries: \n";
            foreach (object entry in list)
            {
                Response += $"  {i}. {entry}";
                i++;
            }

            if (list.Count == 0)
            {
                Response += $"  [Empty]";
            }

            _success = true;
            return;
        }

        Response =
            $"Option \"{propertyToChange.Name}\" is currently set to \"{value}\" (preset {this._eventConfig.PresetName}) (event {_event.Name}).";
        _success = true;
        return;
    }
    
    /*private void _modifyConfig()
    {
        

        /*ValueType valueType = _getValueType(propertyToChange.PropertyType);
        DebugLogger.LogDebug($"Value type: {valueType}", LogLevel.Debug);
        if (Args.Count < 5 && valueType is ValueType.List or ValueType.Dict)
        {
            Response += "Please Specify SubCommand. ";
            goto ModifyListSubCommands;
        }

        if (Args.Count < 5 && valueType is ValueType.Complex)
        {
            Response = $"Cannot set the value of this type currently.";
            _success = false;
            return;
        }

        // modify non list/dict options.
        if (Args.Count < 5 && valueType is ValueType.Enum or ValueType.Convertible)
        {
            try
            {

                object newValue;
                if (valueType is ValueType.Enum)
                {
                    newValue = Enum.Parse(propertyToChange.PropertyType, Args[3], true);
                }
                else
                {
                    newValue = Convert.ChangeType(Args[3], propertyToChange.PropertyType);
                }

                propertyToChange.SetValue(conf, newValue);
                response =
                    $"Option \"{propertyToChange.Name}\"'s value has been set to \"{value}\" (preset {conf.PresetName}) (event {ev.Name}).";
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception at Command Config.Modify() -> Set Simple Value.",
                    LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                response = "An error has occured while attempting to set the value.";
                return false;
            }
        }

        // Set complex value
        if (valueType == ValueType.Complex)
        {
            response = "Cannot set complex value type. Not Implemented Yet.";
            return false;
        }

        if (valueType is ValueType.List or ValueType.Dict)
        {
            try
            {
                if (value is null)
                {
                    if (!_initializeNullValueFromDefaultConstructor(propertyToChange, ref value, conf,
                            ref response))
                    {
                        return false;
                    }
                }

                switch (arguments.At(4).ToLower())
                {
                    case "add":
                        if (arguments.Count < 7)
                        {
                            response += "Missing Arguments. ";
                            goto ModifyListSubCommands;
                        }

                        if (value is IDictionary dict)
                        {
                            if (arguments.Count < 7)
                            {
                                response =
                                    "For dictionaries, you must supply a key and value when adding an entry.\n";
                                goto ModifyListSubCommands;
                            }

                            if (dict.Contains(arguments.At(5)))
                            {
                                response =
                                    "An entry was already found for that value. Use modify to modify the value of existing entries \n";
                                goto ModifyListSubCommands;
                            }

                            try
                            {
                                _getValueOfObjectFromString(arguments.At(0),);
                                dict.Add();
                            }
                            catch (Exception e)
                            {
                                DebugLogger.LogDebug(
                                    $"Could not add value to a dictionary. Ensure the value is correct.",
                                    LogLevel.Error, true);
                                DebugLogger.LogDebug($"{e}", LogLevel.Debug, false);

                            }
                        }

                        if (value is IList list)
                        {

                        }

                        break;
                    case "remove":
                        if (arguments.Count < 6)
                        {
                            Response += "Missing Arguments. ";
                            goto ModifyListSubCommands;
                        }

                        break;
                    case "modify":
                        if (arguments.Count < 7)
                        {
                            Response += "Missing Arguments. ";
                            goto ModifyListSubCommands;
                        }

                        break;
                    default:
                        goto ModifyListSubCommands;
                }

                Response = "Not Implemented Yet.";
                return false;
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug(
                    $"Caught an error at command Config.Modify() -> Set / Modify / Remove List/Dict Value.",
                    LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                Response = "An error has occured while executing the command.";
                return false;
            }

        }

        ModifyListSubCommands:
        _modifyListSubCommand();
        Response = Response;
        return _success;

    }*/

    private void _modifyConfigNew()
    {
        try
        {
            List<NavigatorAction> list = ConfigNavigator.Navigate(ArgsCombined);
            if (list.Count < 1)
            {
                DebugLogger.LogDebug($"No actions found.", LogLevel.Debug);
                Response = "No actions specified.";
                _success = false;
                return;
            }

            if (list[0] is MoveAction)
            {
                list.RemoveRange(0, 1);
            }

            if (list[0] is not Option option)
            {
                Response = "Please Specify a config option.";
                _success = false;
                return;
            }

            
            ComplexObjectModifier confModifier = (ComplexObjectModifier)OptionModifier.GetOrCreateOptionModifer<ComplexObjectModifier>(this._eventConfig.GetType(), this._eventConfig);
            try
            {
                OptionModifier mod = confModifier.GetObjectModifier(option.Data);
                option.Modifier = mod;
                if (option.To is null)
                {
                    Response = $"The current value of {option.Data} is \"{mod.WorkingValue}\"";
                    _success = true;
                    return;
                }
                _processNavAction(option.To);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Could not find option", LogLevel.Debug);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                Response = $"Could not find option.";
                _success = false;
                return;
            }
            object priorObject;
            
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"An error has occured while processing navigation actions.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            Response = $"An error has occured.";
            _success = false;
            return;
        }

    }

    private int _recursiveCount = 0;
    private void _processNavAction(NavigatorAction action)
    {
        if (_recursiveCount >= 10)
        {
            return;
        }

        _recursiveCount++;
        if (action.From is null)
        {
            DebugLogger.LogDebug($"An error has occured while processing nav actions.", LogLevel.Error, true);
            DebugLogger.LogDebug($"Action from is null. Should not be possible.", LogLevel.Debug);
            Response = "An error has occured while processing actions.";
            _success = false;
            return;
        }

        switch (action)
        {
            case MoveAction move:
                if (move.To is null)
                {
                    DebugLogger.LogDebug($"No property to move to has been specified.", LogLevel.Debug);
                    Response = "No option to move to has been specified.";
                    _success = false;
                    return;
                }

                if (move.To is not Option nextOptionMove)
                {
                    DebugLogger.LogDebug($"Tried to move to a non sub-option.", LogLevel.Debug);
                    Response = "You can only move to sub-options.";
                    _success = false;
                    return;
                }
                
                if (move.From.Modifier is not ComplexObjectModifier prevModifierMove)
                {
                    DebugLogger.LogDebug($"Cannot modify a non complex action", LogLevel.Debug);
                    Response = "You cannot select a sub-option of the option, as the property does not have any sub-options.";
                    _success = false;
                    return;
                }

                nextOptionMove.Modifier = prevModifierMove.GetObjectModifier(nextOptionMove.Data);
                _processNavAction(nextOptionMove);
                break;
            case AddAction addAction:
                if (addAction.From.Modifier is null || addAction.From is not Option)
                {
                    DebugLogger.LogDebug($"Entries can only be added for objects that are config options.", LogLevel.Debug);
                    Response = "You can only add entries to valid options that are dictionaries and lists.";
                    _success = false;
                    return;
                }

                if (addAction.From.Modifier is not ListModifier)
                {
                    DebugLogger.LogDebug($"Object has to be a list or dictionary.", LogLevel.Debug);
                    Response = "You can only add entries to dictionaries and lists.";
                    _success = false;
                    return;
                }
                if (!(addAction.To is Object || addAction.To is Option || addAction.To is KeyValuePair))
                {
                    DebugLogger.LogDebug($"Cannot add a non-option / object to a dictionary. {addAction.To.GetType().Name}", LogLevel.Debug);
                    Response = "You can only add entries to valid options that are dictionaries and lists. Valid entries can include key value pairs (dictionaries), values (lists), or objects (json).";
                    _success = false;
                    return;
                }

                if (addAction.From.Modifier is DictionaryModifier && addAction.To is Option)
                {
                    DebugLogger.LogDebug($"You can only add KeyValuePairs (<a,b>) or objects to dictionaries.", LogLevel.Debug);
                    Response = $"You can only add KeyValuePairs (<a,b>) or objects to dictionaries.";
                    _success = false;
                    return;
                }

                if (addAction.From.Modifier is DictionaryModifier dict)
                {
                    if ((OptionModifier.GetConfigOptionValueType(dict.KeyType) is OptionModifiers.ValueType.Complex &&
                         addAction.To is not Object or KeyValuePair { Key: not Object }) ||
                        (OptionModifier.GetConfigOptionValueType(dict.ValueType) is OptionModifiers.ValueType.Complex &&
                         addAction.To is not Object or KeyValuePair { Value: not Object })) 
                    {
                        DebugLogger.LogDebug($"Key or Value is requires a json object to add, which is not present.", LogLevel.Debug);
                        Response = $"Key or Value is requires a json object to add, which is not present.";
                        _success = false;
                        return;
                    }

                    if (addAction.To is KeyValuePair kvp)
                    {
                        object key = null;
                        if (kvp.Key is Object keyObj)
                        {
                            key = JsonConvert.DeserializeObject(keyObj.Data);
                        }
                        else
                        {
                            key = (kvp.Key as Option)?.Data;
                        }

                        if (kvp.Value is Object valueObj)
                        {
                            dict.AddEntry(key, JsonConvert.DeserializeObject(valueObj.Data));
                        }
                        else
                        {
                            dict.AddEntry(key, ((Option)kvp.Value)?.Data);
                        }
                    }
                    if (addAction.To is Object obj)
                    {
                        dict.AddEntry(JsonConvert.DeserializeObject(obj.Data));
                    }
                }
                else
                {
                    ListModifier list = addAction.From.Modifier as ListModifier;
                    if (OptionModifier.GetConfigOptionValueType(list.KeyType) is OptionModifiers.ValueType.Complex &&
                        addAction.To is not Object)
                    {
                        DebugLogger.LogDebug($"Key requires a json object to add, which is not present.", LogLevel.Debug);
                        Response = $"Key requires a json object to add, which is not present.";
                        _success = false;
                        return;
                    }

                    if (addAction.To is Object obj)
                    {
                        list.AddEntry(JsonConvert.DeserializeObject(obj.Data, list.KeyType));
                    }

                    if (addAction.To is Option opt)
                    {
                        list.AddEntry(opt.Data);
                    }
                }

                _success = true;
                Response = "Successfully added entry.";
                return ;
            case ModifyAction modify:
                if (modify.Modifier is null)
                {
                    DebugLogger.LogDebug("An error has occured, while setting a value.", LogLevel.Error, true);
                    Response = "Could not set value due to an error.";
                    _success = false;
                    return;
                }
                
                if (modify.To is null)
                {
                    DebugLogger.LogDebug($"No property to move to has been specified.", LogLevel.Debug);
                    Response = "No option to move to has been specified.";
                    _success = false;
                    return;
                }

                if (modify.From is not Option prevOptionModify)
                {
                    DebugLogger.LogDebug($"You must select an option to modify.", LogLevel.Debug);
                    Response = "You must select an option to modify.";
                    _success = false;
                    return;
                }

                if (prevOptionModify.Modifier is DictionaryModifier or ListModifier)
                {
                    if(modify.To is not Object objMove)
                    {
                        DebugLogger.LogDebug($"Cannot set value of dict or list with non object.", LogLevel.Debug);
                        Response = "Dictionaries and Lists can only be set with json values";
                        _success = false;
                        return;
                    }

                    if (prevOptionModify.Modifier is DictionaryModifier dictModifier)
                    {
                        dictModifier.SetCollectionValue(JsonConvert.DeserializeObject(objMove.Data, dictModifier.WorkingValue.GetType()));
                    }
                    else if(prevOptionModify.Modifier is ListModifier listModifier)
                    {
                        listModifier.SetCollectionValue(JsonConvert.DeserializeObject(objMove.Data, listModifier.WorkingValue.GetType()));
                    }

                    _success = true;
                    Response = "Successfully set value of the option.";
                    goto RecursiveUpdate;
                }
                if (modify.To is not Option nextOption)
                {
                    DebugLogger.LogDebug($"Tried to move to a non sub-option.", LogLevel.Debug);
                    Response = "You can only move to sub-options.";
                    _success = false;
                    return;
                }

                modify.Modifier!.WorkingValue = modify.Modifier!.GetObjectFromString(nextOption.Data);
                RecursiveUpdate:
                var prevModifier = modify.Modifier;
                NavigatorAction prevAction = modify.From;
                while (true)
                {
                    
                    if (modify.Modifier is ComplexObjectModifier complexObjectModifier)
                    {
                        complexObjectModifier.ApplyOptionToProperty(prevModifier);
                        prevModifier = complexObjectModifier;
                    }

                    if (prevAction.From is null)
                    {
                        break;
                    }
                    prevAction = prevAction.From;
                }
                return;
        }
    }

    private void _modifyListSubCommand()
    {
        Response +=
            $"To modify a list or dictionary please use one of the following subcommands: \n" +
            "  <color=yellow>Add [key*] [value]</color>       -> Add a new entry to the. *Note: Key is not necessary for lists.*\n" +
            "  <color=yellow>Remove [key]</color>             -> Remove an existing entry.\n" +
            "  <color=yellow>Modify [key] [new value]</color> -> Modify an existing entry.";
        _success = false;
    }
}