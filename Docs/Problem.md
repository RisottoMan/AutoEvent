# Problems :trollface:
### *You may encounter a problem because of which the mini-games will not run.*
### *So I left hints on how to fix the problems:*
----
 >  You need a map to run a mini-game.1
 - You need to download *``Schematics.tar.gz``* from the [latest release](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) and ``unzip it to the folder`` where the schematics should be located.
 - If you are using ``Exiled``, then unzip the archive in ``EXILED/Configs/AutoEvent/Schematics``
 - If you are using ``NWApi``, then unzip the archive in ``PluginAPI/plugins/global/AutoEvent/Schematics``
   - Ensure that the maps are added to the proper directory defined in the `schematics_directory_path` config.
   - Ensure that the directory defined has proper read/write permissions, and is actually a valid directory. 
     - In pterodactyl the config must be in the `/home/container/` directory. 
----
 >  You do not have permission to use this command
 - You didn't grant the rights for the role. [(Look at the point "2. Permission")](https://github.com/KoT0XleB/AutoEvent-Exiled/blob/main/Docs/Installation.md)
----
 >  Only 1 argument is needed - the command name of the event!
 - You have run the command incorrectly. [(Follow this link to understand)](https://github.com/KoT0XleB/AutoEvent-Exiled/blob/main/Docs/Commands.md)
----
 >  The event was not found, nothing happened.
 - This is the most common cause, but it's easy to fix. There may be several reasons:
    - You have installed the plugins incorrectly. [(Try again.)](https://github.com/KoT0XleB/AutoEvent-Exiled/blob/main/Docs/Installation.md)
    - Or the problem is that you have not installed [Maps](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Schematics) and [Music](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music).
    - Try checking the debug-output.log.
---- 
## Enabling Debug Mode (debug-output.log): 
Autoevent has 2 methods of logging debug outputs. By default neither modes of logging are on. They can be enabled with their respective config options.
- Method 1 - Console logging (`debug`):
   - Console logging logs all errors to the console directly.
- Method 2 - Debug File Logging (`auto_log_debug`):
   - Debug file logging stores all errors to a debug file in the base autoevent directory. 
     - For NWApi: `~/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/debug-output.log`
     - For Exiled: `~/.config/EXILED/Configs/AutoEvent/debug-output.log`
