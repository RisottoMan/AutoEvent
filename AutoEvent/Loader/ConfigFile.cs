using System.Collections.Generic;
using AutoEvent.Interfaces;

namespace AutoEvent;

public class ConfigFile
{
    public Dictionary<string, EventConfig> Configs { get; set; }
    public string Language { get; set; } = "english";
}