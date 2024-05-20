using System.Collections.Generic;
using AutoEvent.API;
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