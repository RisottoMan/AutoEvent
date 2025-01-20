using System;
using AutoEvent.API.Season.Enum;

namespace AutoEvent.API.Season;
public class SeasonMethod
{
    /// <summary>
    /// Get today's festive style
    /// </summary>
    /// <returns></returns>
    public static SeasonStyle GetSeasonStyle()
    {
        if (_curStyle is not null)
            return _curStyle;

        SeasonStyle style = null;
        DateTime curDate = new DateTime(2024, DateTime.Now.Month, DateTime.Now.Day);
        
        foreach (SeasonStyle item in _styles)
        {
            if (item.FirstDate <= curDate && curDate <= item.LastDate)
            {
                style = item;
            }
        }
        
        style ??= new SeasonStyle()
        {
            PrimaryColor = "#FFFF00",
            SeasonFlag = 0
        };

        _curStyle = style;
        return style;
    }

    /// <summary>
    /// Current style
    /// </summary>
    private static SeasonStyle _curStyle { get; set; }

    /// <summary>
    /// All styles are located here
    /// </summary>
    private static SeasonStyle[] _styles { get; } =
    {
        new SeasonStyle()
        {
            Text = "<size=80><color=#42aaff><b>🎄 MERRY CHRISTMAS 🎄</b></color></size>",
            PrimaryColor = "#42aaff",
            SeasonFlag = SeasonFlags.Christmas,
            FirstDate = new DateTime(2024, 1, 1),
            LastDate = new DateTime(2024, 1, 10)
        },
        new SeasonStyle()
        {
            Text = "<size=80><color=#42aaff><b>🎄 HAPPY NEW YEAR 🎄</b></color></size>",
            PrimaryColor = "#77dde7",
            SeasonFlag = SeasonFlags.Christmas,
            FirstDate = new DateTime(2024, 1, 11),
            LastDate = new DateTime(2024, 2, 13)
        },
        new SeasonStyle()
        {
            Text = "<size=75><color=#FF96DE><b>😍 Happy Valentine’s Day 😍</b></color></size>",
            PrimaryColor = "#FF96DE",
            SeasonFlag = SeasonFlags.ValentineDay,
            FirstDate = new DateTime(2024, 2, 14),
            LastDate = new DateTime(2024, 2, 21)
        },
        new SeasonStyle()
        {
            Text = "<size=70><color=#27A327><b>😂 Funny April Fool's Day 😂</b></color></size>",
            PrimaryColor = "#27A327",
            SeasonFlag = SeasonFlags.AprilFoolDay,
            FirstDate = new DateTime(2024, 4, 1),
            LastDate = new DateTime(2024, 4, 7)
        },
        new SeasonStyle()
        {
            Text = "<size=75><color=#F5F5DC><b>🐰 Easter Holidays 🐣</b></color></size>",
            PrimaryColor = "#F5F5DC",
            SeasonFlag = SeasonFlags.EasterHolidays,
            FirstDate = new DateTime(2024, 4, 17),
            LastDate = new DateTime(2024, 4, 24)
        },
        new SeasonStyle()
        {
            Text = "<size=85><color=#F1FF52><b>☀️ Summer Holidays ☀️</b></color></size>",
            PrimaryColor = "#F1FF52",
            SeasonFlag = SeasonFlags.SummerHolidays,
            FirstDate = new DateTime(2024, 6, 1),
            LastDate = new DateTime(2024, 6, 14)
        },
        new SeasonStyle()
        {
            Text = "<size=80><color=#FFA500><b>🔔📖 Autumn 📖🔔</b></color></size>",
            PrimaryColor = "#FFA500",
            SeasonFlag = SeasonFlags.Autumn,
            FirstDate = new DateTime(2024, 9, 1),
            LastDate = new DateTime(2024, 9, 14)
        },
        new SeasonStyle()
        {
            Text = "<size=80><color=#8B00FF><b>👻 Halloween 🎃</b></color></size>",
            PrimaryColor = "#8B00FF",
            SeasonFlag = SeasonFlags.Halloween,
            FirstDate = new DateTime(2024, 10, 21),
            LastDate = new DateTime(2024, 10, 30)
        },
        new SeasonStyle()
        {
            Text = "<size=80><color=#FF0000><b>🔥 Black Friday 🔥</b></color></size>",
            PrimaryColor = "#FF0000",
            SeasonFlag = SeasonFlags.BlackFriday,
            FirstDate = new DateTime(2024, 11, 25),
            LastDate = new DateTime(2024, 11, 30)
        },
        new SeasonStyle()
        {
            Text = "<size=70><color=#42aaff><b>New year is coming...</b></color></size>",
            PrimaryColor = "#77dde7",
            SeasonFlag = SeasonFlags.Christmas,
            FirstDate = new DateTime(2024, 12, 25),
            LastDate = new DateTime(2024, 12, 31)
        }
    };
}