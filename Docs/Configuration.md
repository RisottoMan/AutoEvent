# Events and Configs
A List of Events and a list of config options they support.

## General
All configs have shared options that are present in any config. If use improperly or contains parameters outside the scope of the event, it may break the event.

### Available Shared Options:

| Option Name                 | Description                                                                                                                                                    | See Also:             |
|-----------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------|
| Available Maps              | Allows you to specify a map or list of maps to choose from for the event to use. **Some events require custom components in the event to work, such as jail.** | MapChance             |
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


### SoundChance
> ### SoundInfo

### FriendlyFireSettings
Be aware: Some events may override this setting if it is deemed necessary for event functionality. By default most plugins will just use whatever the default server value is. ***This feature works with CedMod.***

| Value   | Description                                                  |
|---------|--------------------------------------------------------------|
| Enable  | Enable Friendly Fire / Friendly Fire Autoban for the round.  |
| Disable | Disable Friendly Fire / Friendly Fire Autoban for the round. |
| Default | Use whatever the default server setting is.                  |

### Vector3


### Loadout




## Battle


## Boss
## Deathmatch
## DeathParty
## Escape
## FallDown
## FinishWay
## Football
## Glass
## GunGame
## HideAndSeek
## Infection
## Jail
## Knives
## Lava
## Line
## Puzzle
## Survival
## Versus
## ZombieEscape
