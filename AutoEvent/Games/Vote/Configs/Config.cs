using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Vote;
public class Config : EventConfig
{
    [Description("How many seconds to hold a vote?")]
    public short VoteTimeInSeconds { get; set; } = 30;
    [Description("How long to wait in seconds after the voting is over?")]
    public short PostRoundDelayInSeconds { get; set; } = 5;
    [Description("The loadouts that players can get.")]
    public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Items = new List<ItemType>() { ItemType.Coin }
        }
    };
}

