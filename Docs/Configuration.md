# Changing configs
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

#### What does list mean? 
This means that you can add multiple audio files and schematics to your config at the same time.

```
available_sounds:
- This is the first item in the list
- This is the second item in the list
```
The structures of the elements will be presented below. You can copy and paste them as I indicated here. A minus sign indicates a new object.

## Detailed configuration
### Available Sounds - A list of sounds that can be used for this event

| Option    | Description                               |
|-----------|-------------------------------------------|
| Chance    | The chance of getting this sound          |
| SoundName | The name of the sound                     |
| Volume    | The volume that the sound should play at. |
| Loop      | Should the sound loop or not.             |

```
available_sounds:   - A list of sounds that can be used for this event.
- # I am the first item in the list, but you can copy and paste me as the next item. Don't forget to copy all the parameters in me.
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
6) If you find an error when starting the mini-games, repeat all the steps again (1 - 5).

### Available Maps

| Option     | Description                                                                        |
|------------|------------------------------------------------------------------------------------|
| Chance     | The chance that this map will be selected. If it is the only map, this is ignored. |
| Position   | The position to spawn the map.                                                     |
| Rotation   | The rotation of the map.                                                           |
| Scale      | The scale of the map.                                                              |
| IsStatic   | If there are objects in the map that use animations, then they will not be static. |
| SeasonFlag | You can specify on which holiday to allow the map to be launched.                  |

```
available_maps:   - A list of schematics that can be used for this event.
- # I am the first item in the list, but you can copy and paste me as the next item. Don't forget to copy all the parameters in me.
  chance: 50   - If you have several music files, they will be selected by chance in percent. For example, 10%.
  map:  - This is an object that contains detailed information about your map.
    sound_name: 'DeathParty'   - Pay special attention that you do not specify json, but the name of the folder of your schematics.
    position:   - The Vector3 type stores the x/y/z coordinates in which your map will appear.
      x: 10
      y: 1012
      z: -40
    rotation:   - The Vector3 type saves the x/y/z rotation of your map.
      x: 0
      y: 0
      z: 0
    scale:   - The Vector3 type saves the x/y/z size of your map.
      x: 1
      y: 1
      z: 1
    is_static: true   - Optimizes the tps for your server if you specify true, but does not work with animations.
  season_flag: None   - Specify None if you want your map to always work, not on holidays.
```

### FriendlyFireSettings
Be aware: Some events may override this setting if it is deemed necessary for event functionality. By default most plugins will just use whatever the default server value is. ***This feature works with CedMod.***

| Value   | Description                                                  |
|---------|--------------------------------------------------------------|
| Enable  | Enable Friendly Fire / Friendly Fire Autoban for the round.  |
| Disable | Disable Friendly Fire / Friendly Fire Autoban for the round. |
| Default | Use whatever the default server setting is.                  |
