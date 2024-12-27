using System.Collections.Generic;
using AutoEvent.Interfaces;

namespace AutoEvent;

public class TranslationFile
{
    public Dictionary<string, EventTranslation> Translations { get; set; }
    public string Language { get; set; } = "english";
}