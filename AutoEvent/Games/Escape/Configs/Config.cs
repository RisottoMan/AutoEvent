using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Escape;
public class Config : EventConfig
{
    [Description("How long players have to escape in seconds. [Default: 70]")]
    public int EscapeDurationTime { get; set; } = 70;

    [Description("The time of the start and resume of the warhead in seconds. [Default: 100]")]
    public int EscapeResumeTime { get; set; } = 100;
}