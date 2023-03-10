# Installation
## 1. Download and Setup
### *You need to download the latest release:*

- Plugin [``AutoEvent.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/tag/1.0.2) move to => ``EXILED/Plugins``

- Plugin [``MapEditorReborn.dll``](https://github.com/Michal78900/MapEditorReborn) (or [update in discord](https://discord.gg/sQcSSPjf8p)) move to => ``EXILED/Plugins``

- Plugin [``SCPSLAudioApi.dll``](https://github.com/CedModV2/SCPSLAudioApi/releases/latest)  move to => ``EXILED/Plugins/dependencies``

- [Music](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music) files move to => ``EXILED/Configs/Music``

- [Maps](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Schematics) files move to => ``EXILED/MapEditorReborn/Schematics``

## 2. Permission
### *Give permission to your role in ``EXILED/Configs/permissions.yml``:*

```
owner:
  inheritance: [ ]
  permissions:
    - ev.*
```

## 3. Problems
### You may encounter a problem because of which the mini-games will not run.
### So I left hints on how to fix the problems:

 >  You do not have permission to use this command
#### You didn't grant the rights for the role. Look at the point "2. Permission"

 >  Only 1 argument is needed - the command name of the event!
#### You have run the command incorrectly. (Follow this link to understand)

 >  The event was not found, nothing happened.
### This is the most common cause, but it's easy to fix. There may be several reasons:
#### 1) You have installed the plugins incorrectly. Try again.
#### 2) Or the problem is that you have not installed Maps and Music.
#### 3) The problem may be in MapEditorReborn, which depends on the Exiled version. Make sure that MER is working.
