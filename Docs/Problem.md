# Problems :trollface: - *How can I fix my problem?*
## 1) You do not have permission to use this command:
![image](https://github.com/user-attachments/assets/b96bbf64-e981-4f9a-8200-eb1aab1b8014)
- Read here how to set permissions for your role: [(Press me)](https://github.com/RisottoMan/AutoEvent/blob/main/Docs/Installation.md)
---
## 2) I only have one mini-game. What should I do?
![image](https://github.com/user-attachments/assets/c40ac4d8-7753-4627-bf39-d514d53c3b98)
- You haven't installed MapEditorReborn, so you have mini-games that won't need the maps.
- Install the MapEditorReborn plugin so that the plugin can load maps: [(Github MER)](https://github.com/Michal78900/MapEditorReborn/releases/latest)

![image](https://github.com/user-attachments/assets/312135a9-9106-4b86-805f-5c74d8024ff2)
---
## 3) MapEditorReborn was not detected. AutoEvent will not be loaded until you install MapEditorReborn.
![image](https://github.com/user-attachments/assets/818a486e-d6f2-4a32-80af-434f24515625)
- This is the same problem as in point 2. 
- Install MapEditorReborn so that the plugin can load maps: [(Github MER)](https://github.com/Michal78900/MapEditorReborn/releases/latest)

![image](https://github.com/user-attachments/assets/312135a9-9106-4b86-805f-5c74d8024ff2)
---
## 4) You have installed the old version of 'MapEditorReborn' and cannot run this mini-game
![image](https://github.com/user-attachments/assets/e66573f4-1899-43a7-9724-01d3c9cd97ec)
- It says what it means. You have an old version of MapEditorReborn. There are new features or a new version of SCP has been released. Therefore, errors appear.
- Install the new version of MapEditorReborn so that the plugin can load schematics: [(Github MER)](https://github.com/Michal78900/MapEditorReborn/releases/latest)

![image](https://github.com/user-attachments/assets/312135a9-9106-4b86-805f-5c74d8024ff2)
- If the problem persists, download the latest release (or testing release) from [(Discord MER)](https://discord.gg/JwAfeSd79u): 
![image](https://github.com/user-attachments/assets/d6ed4a8d-5dfc-42a4-818e-5924b6215438)

---
## 5) You need to download the map (something) to run this mini-game.
![image](https://github.com/user-attachments/assets/1a71fb4f-08b3-4411-a693-25ac9aae26f6)
- It says what it means. This map is not exist on your server, so the mini-game cannot be run.
- You need to download *``Schematics.tar.gz``* from the [latest release](https://github.com/RisottoMan/AutoEvent/releases/latest).

![image](https://github.com/user-attachments/assets/469eab25-2f94-4414-87dc-7402a5068aaf)
- Unzip *``Schematics.tar.gz``* to ``EXILED/Configs/AutoEvent/Schematics`` folder.

![image](https://github.com/user-attachments/assets/1797ee0b-ed3d-42a5-9fea-546bdf8bca12)
---
## 6) Caught an exception at Event.OnStart().

![image](https://github.com/user-attachments/assets/934b43a1-8802-48be-9c95-b84fe25103b9)
- If earlier errors referred to the fact that you did not install something correctly and all responsibility lay with you as a plugin user, now this error refers to me as a plugin developer.
- Write to the issue detailing the problem:
![image](https://github.com/user-attachments/assets/2a47ffca-c06e-42d1-9516-71d7018abfbd)
- I will fix the problem as soon as I find some free time.
---
## 7) The mini-game (something) is not found:

![image](https://github.com/user-attachments/assets/7c828cec-1c5c-4f50-a4d1-9e22ebd961e7)

- Enter the ev list command in the console:

![image](https://github.com/user-attachments/assets/a25398ca-15d1-452f-b555-7a4ad5522db1)
- Find the name of the command in square brackets:

![image](https://github.com/user-attachments/assets/432b6513-ca13-496c-858a-95a7b2b90866)
- Enter this command in ev run:

![image](https://github.com/user-attachments/assets/fff98a27-b4ac-47e4-8610-a05c3f0f40a6)
---- 
## Enabling Debug Mode (debug-output.log): 
Autoevent has 2 methods of logging debug outputs. By default neither modes of logging are on. They can be enabled with their respective config options.
- Method 1 - Console logging (`debug`):
   - Console logging logs all errors to the console directly.
- Method 2 - Debug File Logging (`auto_log_debug`):
   - Debug file logging stores all errors to a debug file in the base autoevent directory. 
     - For NWApi: `~/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/debug-output.log`
     - For Exiled: `~/.config/EXILED/Configs/AutoEvent/debug-output.log`
