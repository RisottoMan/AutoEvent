using System.ComponentModel;
using YamlDotNet.Serialization;


namespace AutoEvent.Interfaces;
public class SoundInfo
{
    public SoundInfo() { }
    public SoundInfo(string name, byte volume = 10, bool loop = true, AudioPlayer audioPlayerApi = null)
    {
        SoundName = name;
        Volume = volume;
        Loop = loop;
        AudioPlayer = audioPlayerApi;
    }
    [Description("The name of the sound.")]
    public string SoundName { get; set; }
    
    [Description("The volume that the sound should play at.")]
    public byte Volume { get; set; } = 10;
    
    [Description("Should the sound loop or not.")]
    public bool Loop { get; set; } = true;

    [Description("The object that plays music.")]
    public AudioPlayer AudioPlayer { get; set; }

    [YamlIgnore]
    public bool StartAutomatically { get; set; } = true;
}