# Commands

| Command                              | Description                                                    |
|--------------------------------------|----------------------------------------------------------------|
| `ev list`                            | Shows a list of available mini-games.                          |
| `ev run [argument]`                  | Starts the mini-game by an argument from the `ev_list` command |
| `ev stop`                            | Kills a stuck mini-game (Just kills all the players)           |
| `ev volume [0 - 200]`                | Change the global volume percent.                              |
| `ev language list`                   | List of all available translations.                            |
| `ev language load [language]`        | Load the translation. It will replace the current translation. |

----

# Description :frog:
``ev list`` => *To see all the mini-games, enter the command in the admin console:*
- This command will show all the mini-games.
- For example, ``[escape] <= Escape from the facility behind SCP-173 at supersonic speed!``
- The square brackets indicate the argument that can be used in the command ``ev run``

``ev run [name]`` => *To start the mini-game, enter the command:*
- This command starts the mini-game via the specified argument.
- For example, ``ev run escape``

``ev stop`` => *If the mini-games are stuck during the game, then enter the command:*
- This command will stop the mini-game by killing all the players on the server.
