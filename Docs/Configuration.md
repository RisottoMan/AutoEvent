# Configuration of configs
A List of Events and a list of config options they support.

## General
All configs have shared options that are present in any config. If use improperly or contains parameters outside the scope of the event, it may break the event.

### Available Shared Options:

| Option Name                 | Description                                                                                                                                                    | See Also:             |
|-----------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------|
| Available Maps              | Allows you to specify a map or list of maps to choose from for the event to use. **Some events require custom components in the event to work, such as versus.** | MapChance             |
| Available Sounds            | Allows you to specify an audio or list of audios to choose from for the event to use.                                                                          | SoundChance           |
| Event Friendly Fire Autoban | Allows you to manually specify whether or not the friendly fire autoban should be enabled / disabled.                                                          | FriendlyFireSettings  |
| Event Friendly Fire         | Allows you to manually specify whether friendly fire should be allowed for an event.                                                                           | FriendlyFireSettings  |
| Debug                       | Should debug mode be enabled for this plugin. For any built in plugins, this option is ignored, and the base config option is used instead.                    |                       |


### General Setting Types
Many of the settings used option types that are can be used by any plugin. Here are the general option types to be aware of. 

### MapChance

| Option | Description                                                                        |
|--------|------------------------------------------------------------------------------------|
| Chance | The chance that this map will be selected. If it is the only map, this is ignored. |
| Map    | The MapInfo (See Below) for the map.                                               |

> ### MapInfo
| Option   |     |     | 
|----------|-----|-----|
| MapName  |     |     |
| Position |     |     |
| Rotation |     |     |

### Available Sounds - A list of sounds that can be used for this event

| Option    | Description                               |
|-----------|-------------------------------------------|
| Chance    | The chance of getting this sound          |
| SoundName | The name of the sound                     |
| Volume    | The volume that the sound should play at. |
| Loop      | Should the sound loop or not.             |

```
available_sounds:   - A list of sounds that can be used for this event.
  chance: 10   - If you have several music files, they will be selected by chance in percent. For example, 10%.
  sound:   - This is an object that contains detailed information about your music.
    sound_name: 'YourMusicName.ogg'   - Specify the name of your music with the ogg type.
    volume: 25  - Specify the volume of the music. It should not be too loud [VSR 8.3.1] or quiet.
    loop: true  - Loops the music, so if it reaches the end, it will start playing again.
    audio_player_base:  - Do not specify anything. This is my mistake)
```

#### How do convert audio files from mp3 to ogg format?
1) Go to website https://convertio.co/mp3-ogg/
2) Click on the "Advanced" button to adjust the settings
3) Settings:
- Codec: Ogg (Vorbis)
- Quality: Lowest  - The quality does not affect the sound quality in the game
- Audio Channels: Mono (1.0)
- Frequency: 48000 Hz
- Volume: No change
4) Insert your audio file to website and click the "Convert" button.
5) Download and transfer the file to the Music folder.

### FriendlyFireSettings
Be aware: Some events may override this setting if it is deemed necessary for event functionality. By default most plugins will just use whatever the default server value is. ***This feature works with CedMod.***

| Value   | Description                                                  |
|---------|--------------------------------------------------------------|
| Enable  | Enable Friendly Fire / Friendly Fire Autoban for the round.  |
| Disable | Disable Friendly Fire / Friendly Fire Autoban for the round. |
| Default | Use whatever the default server setting is.                  |
