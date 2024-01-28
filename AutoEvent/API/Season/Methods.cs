using System;

namespace AutoEvent.API.Season;

public class Methods
{
    private static SeasonStyle _curStyle { get; set; }
    public static SeasonStyle GetSeasonStyle()
    {
        if (_curStyle is not null)
            return _curStyle;

        SeasonStyle style = null;
        DateTime curDate = DateTime.Now.Date;

        switch(curDate.Month)
        {
            case 1:
                {
                    if (0 < curDate.Day && curDate.Day <= 10)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=80><color=#42aaff><b>🎄 MERRY CHRISTMAS 🎄</b></color></size>",
                            PrimaryColor = "#42aaff",
                        };
                    }

                    if (10 < curDate.Day && curDate.Day <= 30)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=80><color=#42aaff><b>🎄 HAPPY NEW YEAR 🎄</b></color></size>",
                            PrimaryColor = "#77dde7",
                        };
                    }

                    break;
                }
            case 2:
                {
                    if (13 < curDate.Day && curDate.Day <= 21)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=75><color=#FF96DE><b>😍 Happy Valentine’s Day 😍</b></color></size>",
                            PrimaryColor = "#FF96DE",
                        };
                    }

                    break;
                }
            case 4:
                {
                    if (0 < curDate.Day && curDate.Day <= 7)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=70><color=#27A327><b>😂 Funny April Fool's Day 😂</b></color></size>",
                            PrimaryColor = "#27A327",
                        };
                    }

                    if (16 < curDate.Day && curDate.Day <= 24)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=75><color=#F5F5DC><b>🐰 Easter Holidays 🐣</b></color></size>",
                            PrimaryColor = "#F5F5DC",
                        };
                    }

                    break;
                }
            case 6:
                {
                    if (0 < curDate.Day && curDate.Day <= 14)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=85><color=#F1FF52><b>☀️ Summer Holidays ☀️</b></color></size>",
                            PrimaryColor = "#F1FF52",
                        };
                    }

                    break;
                }
            case 9:
                {
                    if (0 < curDate.Day && curDate.Day <= 14)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=80><color=#FFA500><b>🔔📖 Autumn 📖🔔</b></color></size>",
                            PrimaryColor = "#FFA500",
                        };
                    }

                    break;
                }
            case 10:
                {
                    if (20 < curDate.Day && curDate.Day <= 30)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=80><color=#8B00FF><b>👻 Halloween 🎃</b></color></size>",
                            PrimaryColor = "#8B00FF",
                        };
                    }

                    break;
                }
            case 11:
                {
                    if (27 < curDate.Day && curDate.Day <= 30)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=80><color=#FF0000><b>🔥 Black Friday 🔥</b></color></size>",
                            PrimaryColor = "#FF0000",
                        };
                    }

                    break;
                }
            case 12:
                {
                    if (24 < curDate.Day && curDate.Day <= 31)
                    {
                        style = new SeasonStyle()
                        {
                            Text = "<size=70><color=#42aaff><b>New year is coming...</b></color></size>",
                            PrimaryColor = "#77dde7",
                        };
                    }

                    break;
                }
        }

        style ??= new SeasonStyle()
        {
            PrimaryColor = "#FFFF00"
        };

        _curStyle = style;
        return style;
    }
}