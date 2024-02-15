# Installation
### Select the installation on the framework that you are using.
# Exiled
## 1. Download and Setup :moyai:
### *You need to download the latest release:*

- Plugin [``AutoEvent.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) move to => ``EXILED/Plugins``
  - Dependencies are no longer necessary. They are bundled into the plugin now.

- [Music](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music) files move to => ``EXILED/Configs/AutoEvent/Music``

- [Maps](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Schematics) folders with a files move to => ``EXILED/Configs/AutoEvent/Schematics``

- Ensure that the Config has the following directories, and that they can be accessed to the server.
```yml
# Where the configs directory is located. By default it is located in the AutoEvent folder.
event_configs_directory_path: /home/container/.config/EXILED/Configs/AutoEvent/Configs
# Where the external events directory is located. By default it is located in the AutoEvent folder.
external_events_directory_path: /home/container/.config/EXILED/Configs/AutoEvent/Events
# Where the schematics directory is located. By default it is located in the AutoEvent folder.
schematics_directory_path: /home/container/.config/EXILED/Configs/AutoEvent/Schematics
# Where the music directory is located. By default it is located in the AutoEvent folder.
music_directory_path: /home/container/.config/EXILED/Configs/AutoEvent/Music
```
- ***Sometimes these settings fail to auto-generate in the config properly, so please double check they are valid before reaching out to us.***


## 2. Permission :gem:
### *Give permission to your role in ``EXILED/Configs/permissions.yml``:*

```
owner:
  inheritance: [ ]
  permissions:
    - ev.*
```
Available Permission Tress:
```
ev.*  /   - Main Permission for all AutoEvent commands.
  - ev.list   - View the available events.
  - ev.run    - Run an event.
  - ev.stop   - Stop an event.
  - ev.volume - Change the volume of all events.
  
  - ev.config.* /  - Main permission for all config commands.
      - ev.config.list    - List all available config presets for an event.
      - ev.config.select  - Select a different config preset for an event.
  
  - ev.reload.* /  - Main permission for all reload commands.
      - ev.reload.configs       - Reload configs / config presets for all events.
      - ev.reload.events        - Reload all events.
      - ev.reload.translations  - Reload all translations.
      
  - ev.debug - Main permission for all debug commands.
```

# NWApi
## 1. Download and Setup :moyai:
### *You need to download the latest release:*
- Plugin [``AutoEvent.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) move to => ``PluginAPI/plugins/global``
  - Dependencies are no longer necessary. They are bundled into the plugin now.
  
- [Music](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music) files move to => ``PluginAPI/plugins/global/AutoEvent/Music``

- [Maps](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Schematics) folders with a files move to => ``PluginAPI/plugins/global/AutoEvent/Schematics``

- Ensure that the Config has the following directories, and that they can be accessed to the server. 
```yml
# Where the configs directory is located. By default it is located in the AutoEvent folder.
event_configs_directory_path: /home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Configs
# Where the external events directory is located. By default it is located in the AutoEvent folder.
external_events_directory_path: /home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Events
# Where the schematics directory is located. By default it is located in the AutoEvent folder.
schematics_directory_path: /home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Schematics
# Where the music directory is located. By default it is located in the AutoEvent folder.
music_directory_path: /home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Music
```
- ***Sometimes these settings fail to auto-generate in the config properly, so please double check they are valid before reaching out to us.*** 

## 2. Permission :gem:
### *Give permission to your role in ``PluginAPI/plugins/global/AutoEvent-NWApi/configs/autoevent.yml``:*

```
# A list of admins who can run mini-games. Specify the GroupName from the config_remoteadmin
permission_list:
- owner
- admin
- moderator
```

### If you use CedMod or NWApiPermissionSystem you can use the following permissions instead:
```
ev.*  /   - Main Permission for all AutoEvent commands.
  - ev.list   - View the available events.
  - ev.run    - Run an event.
  - ev.stop   - Stop an event.
  - ev.volume - Change the volume of all events.
  
  - ev.config.* /  - Main permission for all config commands.
      - ev.config.list    - List all available config presets for an event.
      - ev.config.select  - Select a different config preset for an event.
  
  - ev.reload.* /  - Main permission for all reload commands.
      - ev.reload.configs       - Reload configs / config presets for all events.
      - ev.reload.events        - Reload all events.
      - ev.reload.translations  - Reload all translations.
      
  - ev.debug - Main permission for all debug commands.
```
