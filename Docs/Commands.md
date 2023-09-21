# Commands

| Command                              | Description                                                    |
|--------------------------------------|----------------------------------------------------------------|
| `ev list`                            | Shows a list of available mini-games.                          |
| `ev run [argument]`                  | Starts the mini-game by an argument from the `ev_list` command |
| `ev stop`                            | Kills a stuck mini-game (Just kills all the players)           |
| `ev volume [0 - 200]`                | Change the global volume percent.                              |
| `ev config list [event]`             | List config presets that are available for events.             |
| `ev config select [event] [preset]`  | Select a config preset to use while running an event.          |
| `ev reload [configs / events ]`      | Reload all events or the configs for all events.               |
| `ev debug [option]`                  | Options for developers to test events.                         |

----

# Description :frog:
## *To see all the mini-games, enter the command in the admin console:*
``ev list``
#### This command will show all the mini-games.
#### For example, ``[escape] <= Escape from the facility behind SCP-173 at supersonic speed!``
#### The square brackets indicate the argument that can be used in the command ``ev run``

## *To start the mini-game, enter the command:*
``ev run [name]``
#### This command starts the mini-game via the specified argument.
#### For example, ``ev run escape``

## *To select a config for a mini-game, enter the command:*
``ev config select [event name] [preset name]``
#### If you or the plugin author has made config presets, you can select them before or during an event.
#### Be aware that not all config options can be applied to an event while it is running.
#### For example ``ev config select Infection PeanutInfection``

## *If the mini-games are stuck during the game, then enter the command:*
``ev stop``
#### This command will stop the mini-game by killing all the players on the server.
