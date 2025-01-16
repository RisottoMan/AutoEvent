# Installation
## Select only one framework where you install the plugin.
# Exiled
## 1. Download and Setup :moyai:
### *You need to download the latest release:*

- [``AutoEvent.dll``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) move to => ``EXILED/Plugins``

- [``Music``](https://github.com/KoT0XleB/AutoEvent-Exiled/tree/main/Music) files move to => ``EXILED/Configs/AutoEvent/Music``

- [``Schematics.tar.gz``](https://github.com/KoT0XleB/AutoEvent-Exiled/releases/latest) unzip files to => ``EXILED/Configs/AutoEvent/Schematics``

- Ensure that the Config has the following directories, and that they can be accessed to the server.
```yml
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
ev.*           - Main Permission for all AutoEvent commands.
  - ev.list    - View the available events.
  - ev.run      - Run an event.
  - ev.stop     - Stop an event.
  - ev.volume   - Change the volume of all events.
  - ev.language - Change language for translations.
```