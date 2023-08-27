# Installation
# Exiled
## 1. Download and Setup :moyai:
### *You need to download the latest release:*

- Plugin [``AutoEvent.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) move to => ``EXILED/Plugins``

- Plugin [``MapEditorReborn.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) (or here [discord](https://discord.gg/sQcSSPjf8p)) move to => ``EXILED/Plugins``

- Plugin [``SCPSLAudioApi.dll``](https://github.com/CedModV2/SCPSLAudioApi/releases/latest)  move to => ``EXILED/Plugins/dependencies``

- [Music](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music) files move to => ``EXILED/Configs/Music``

- [Maps](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Schematics) folders with a files move to => ``EXILED/Configs/MapEditorReborn/Schematics``

## 2. Permission :gem:
### *Give permission to your role in ``EXILED/Configs/permissions.yml``:*

```
owner:
  inheritance: [ ]
  permissions:
    - ev.*
```

# NWApi
## 1. Download and Setup :moyai:
### *You need to download the latest release:*
- Plugin [``AutoEvent.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) move to => ``PluginAPI/plugins/global``

- Plugin [``SCPSLAudioApi.dll``](https://github.com/CedModV2/SCPSLAudioApi/releases/latest)  move to => ``PluginAPI/plugins/global/dependencies``

- [Music](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music) files move to => ``PluginAPI/plugins/global/Music``

- [Maps](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Schematics) folders with a files move to => ``PluginAPI/plugins/global/Schematics``

## 2. Permission :gem:
### *Give permission to your role in ``PluginAPI/plugins/global/AutoEvent-NWApi/configs/autoevent.yml``:*

```
# A list of admins who can run mini-games. Specify the GroupName from the config_remoteadmin
permission_list:
- owner
- admin
- moderator
```
