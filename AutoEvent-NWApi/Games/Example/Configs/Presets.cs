// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SingleClassPreset.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 2:16 PM
//    Created Date:     09/13/2023 2:16 PM
// -----------------------------------------

using System.Collections.Generic;
using AutoEvent.Games.Example;
using AutoEvent.Interfaces;
using UnityEngine;

namespace AutoEvent.Games.Example;

public class Presets
{
    public static ExampleConfig SingleLoadout() => new ExampleConfig()
    {
        Loadouts = new List<Loadout>()
        {
            new Loadout()
            {
                Chance = 1,
                Health = 100,
                Items = new List<ItemType>()
                {
                    ItemType.GunAK, ItemType.Painkillers, ItemType.Medkit
                }
            }
        }
    };
    
    public static ExampleConfig BigPeople()
    {
        ExampleConfig conf = new ExampleConfig();
        foreach (var loadout in conf.Loadouts)
        {
            loadout.Size = new Vector3(2, 2, 2);
        }

        return conf;
    }
    
}