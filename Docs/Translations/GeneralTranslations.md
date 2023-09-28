# General Translations
## This page contains information for people who are adding / maintaining translations. 
This page doesn't offer the default translations, but it does show information about the translations, such as what variables can be used.
We greatly appreciate your help.

## General
There are several general option that are on every event.

| Options             | Description                                                               |
|---------------------|---------------------------------------------------------------------------|
| [Event]Name         | A name for the event. (Spaces can be used.)                               |
| [Event]CommandName  | The command that is used to execute. (No spaces - it breaks the command.) |
| [Event]Description  | A description of what the event is. (Spaces can be used.)                 |
<br>

## Variables:
Many event translations use variables as a method for you to insert applicable information.
Variables are surrounded by a `{` and end with a `}`. For example: `{time}`

Here are a list of common variables:

| Name    | Description                                                                                                                                | Example     |
|---------|--------------------------------------------------------------------------------------------------------------------------------------------|-------------|
| Time    | The remaining time left / elapsed time. If there is a pre-round text, this will be the time until the round starts. (10 -> 9 -> 8, etc...) | `{time}`    |
| Players | The remaining players alive.                                                                                                               | `{players}` |


# Events:

## Battle
| Translations   | Description                                                   | Variables                                       |
|----------------|---------------------------------------------------------------|-------------------------------------------------|
| BattleTimeLeft | Pre-Round information text.                                   | `{time}`                                        |
| BattleCounter  | Displayed while the round is running.                         | `{time}`, `{FoundationForces}`, `{ChaosForces}` |
| BattleCiWin    | Displayed when the game is finished and Chaos Insurgency win. | `{time}`                                        |
| BattleMtfWin   | Displayed when the game is finished and  Mtf Win.             | `{time}`                                        |

| Variables            | Description                                        |
|----------------------|----------------------------------------------------|
| `{time}`             | The time elapsed.                                  |
| `{FoundationForces}` | The amount of players on the MTF team remaining.   |
| `{ChaosForces}`      | The amount of players on the Chaos team remaining. |
<br>

## Boss
| Translations   | Description                                                | Variables                   |
|----------------|------------------------------------------------------------|-----------------------------|
| BossTimeLeft   | Pre-Round information text.                                | `{time}`                    |
| BossCounter    | Displayed while the round is running.                      | `{time}`, `{hp}`, `{count}` |
| BossWin        | Displayed when the game is finished and the boss(es) wins. | `{hp}`                      | 
| BossHumansWin  | Displayed when the game is finished and the humans win.    | `{count}`                   |

| Variables | Description                            |
|-----------|----------------------------------------|
| `{time}`  | The time elapsed.                      |
| `{hp}`    | The combined health of all the bosses. |
| `{count}` | The number of non-boss players left.   |
<br>

## Deathmatch
| Translations       | Description                                                  | Variables                            |
|--------------------|--------------------------------------------------------------|--------------------------------------|
| DeathmatchCycle    | Displayed while the round is running.                        | `{name}`, `{mtftext}`, `{chaostext}` |
| DeathmatchChaosWin | Displayed when the game is finished and the Chaos team wins. | `{name}`                             |
| DeathmatchMtfWin   | Displayed when the game is finished and the Mtf Team wins.   | `{name}`                             |

| Variables     | Description                                   |
|---------------|-----------------------------------------------|
| `{name}`      | The name of the event.                        |
| `{mtftext}`   | The amount of kills need for the mtf to win.  |
| `{chaostext}` | The amount of kills for the chaos to win.     |
Note - `{mtftext}` and `{chaostext}` has a status bar with squares to demonstrate how many kills the current team has.

<br>

## DeathParty
| Translations    | Description                                                        | Variables            |
|-----------------|--------------------------------------------------------------------|----------------------|
| DeathCycle      | Displayed while the round is running.                              | `{count}`, `{time}`  |
| DeathMorePlayer | Displayed when the game is finished and multiple players survived. | `{count}`, `{time}`  |
| DeathOnePlayer  | Displayed when the game is finished and only one player survived.  | `{winner}`, `{time}` |
| DeathAllDie     | Displayed when the game is finished and nobody survived.           | `{time}`             |

| Variables  | Description                       |
|------------|-----------------------------------|
| `{time}`   | The time elapsed.                 |
| `{count}`  | The number of alive players left. |
| `{winner}` | The name of the player that won.  |
<br>

## Escape
| Translations      | Description                           | Variables             |     |
|-------------------|---------------------------------------|-----------------------|-----|
| EscapeBeforeStart | Pre-Round information text.           | `{name}`, `{time}`    |     |
| EscapeCycle       | Displayed while the round is running. | `{name}`, `{time}`    |     |
| EscapeEnd         | Displayed at the end of the game.     | `{name}`, `{players}` |     |

| Variables | Description                              |
|-----------|------------------------------------------|
| `{name}`  | The name of the event.                   |
| `{time}`  | The time elapsed.                        |
| `{count}` | The number of non-infected players left. |
<br>

## FallDown
| Translations  | Description                                                       | Variables                     |
|---------------|-------------------------------------------------------------------|-------------------------------|
| FallBroadcast | Displayed while the round is running.                             | `{name}`, `{time}`, `{count}` |
| FallWinner    | Displayed when the game is finished and only one player survived. | `{winner}`                    |
| FallDied      | Displayed when the game is finished and everyone died.            |                               |

| Variables  | Description                       |
|------------|-----------------------------------|
| `{name}`   | The name of the event.            |
| `{time}`   | The time elapsed.                 |
| `{count}`  | The number of alive players left. |
| `{winner}` | The name of the player that won.  |
<br>

## FinishWay
| Translations             | Description                                                        | Variables          |
|--------------------------|--------------------------------------------------------------------|--------------------|
| FinishWayCycle           | Displayed while the round is running.                              | `{name}`, `{time}` |
| FinishWayDied            | Displayed when a player dies.                                      |                    |
| FinishWaySeveralSurvived | Displayed when the game is finished and multiple players survived. | `{count}`          |
| FinishWayOneSurvived     | Displayed when the game is finished and one player survived.       | `{player}`         |
| FinishWayNoSurvivors     | Displayed when the game is finished and no players survived.       |                    |

| Variables  | Description                              |
|------------|------------------------------------------|
| `{name}`   | The name of the event.                   |
| `{time}`   | The time remaining. - Not elapsed time   |
| `{count}`  | The number of non-infected players left. |
| `{player}` | The name of the players that won.        |
<br>

## Football
| Translations     | Description                                                                       | Variables                         |
|------------------|-----------------------------------------------------------------------------------|-----------------------------------|
| FootballRedTeam  | Pre-Round info text for the red team.                                             |                                   |
| FootballBlueTeam | Pre-Round info text for the blue team.                                            |                                   |
| FootballTimeLeft | Displayed while the round is running.                                             | `{BluePnt}`, `{RedPnt}`, `{time}` |
| FootballRedWins  | Displayed when the game is finished and the red team wins.                        |                                   |
| FootballBlueWins | Displayed when the game is finished and the blue team wins.                       |                                   |
| FootballDraw     | Displayed when the game is finished and the teams have the same number of points. | `{BluePnt}`, `{RedPnt}`           |

| Variables   | Description                                  |
|-------------|----------------------------------------------|
| `{time}`    | The time remaining. - Not elapsed time.      |
| `{RedPnt}`  | The number of points that the red team has.  |
| `{BluePnt}` | The number of points that the blue team has. |
<br>

## Glass
| Translations     | Description                                                        | Variables              |
|------------------|--------------------------------------------------------------------|------------------------|
| GlassStart       | Displayed while the round is running.                              | `{plyAlive}`, `{time}` |
| GlassDied        | Displayed to a player when they die.                               |                        |
| GlassWinSurvived | Displayed when the game is finished and multiple players survived. | `{plyAlive}`           |
| GlassWinner      | Displayed when the game is finished and one player survived.       | `{winner}`             |
| GlassFail        | Displayed when the game is finished and all players died.          |                        |

| Variables    | Description                           |
|--------------|---------------------------------------|
| `{plyAlive}` | The number of players that are alive. |
| `{time}`     | The time elapsed.                     |
| `{winner}`   | The name of the player that won.      |
<br>

## GunGame
| Translations  | Description                                            | Variables                                               |
|---------------|--------------------------------------------------------|---------------------------------------------------------|
| GunGameCycle  | Displayed while the round is running.                  | `{name}`, `{gun}`, `{kills}`, `{leadgun}`, `{leadnick}` |
| GunGameWinner | Displayed when the game is finished and a player wins. | `{name}`, `{winner}`                                    |

| Variables    | Description                                                             |
|--------------|-------------------------------------------------------------------------|
| `{name}`     | The name of the event.                                                  |
| `{gun}`      | The current gun of the player.                                          |
| `{kills}`    | The number of kills that the player needs in order to get the next gun. |
| `{leadgun}`  | The gun that the player who is in the lead with the most kills has.     |
| `{leadnick}` | The name of the player who is in the lead with the most kills.          |
<br>

## HideAndSeek
| Translations  | Description                                                                          | Variables            |
|---------------|--------------------------------------------------------------------------------------|----------------------|
| HideBroadcast | Displayed while in "break mode" (this is the time which taggers are being selected). | `{time}`             |
| HideCycle     | Displayed while taggers are hunting.                                                 | `{time}`             |
| HideHurt      | Displayed to a player when they die because they didn't tag someone else in time.    |                      | 
| HideOnePlayer | Displayed when the game is finished and one players is left alive.                   | `{time}`, `{winner}` | 
| HideAllDie    | Displayed when the game is finished and everyone is dead (winner left).              | `{time}`             |

| Variables  | Description                                                           |
|------------|-----------------------------------------------------------------------|
| `{name}`   | The name of the event.                                                |
| `{time}`   | The time remaining until the taggers die or the taggers are selected. |
| `{winner}` | The name of the player that won.                                      |
<br>

## Infection
| Translations      | Description                                                    | Variables               |
|-------------------|----------------------------------------------------------------|-------------------------|
| ZombieBeforeStart | Pre-round information text.                                    | `{time}`, `{name}`      |
| ZombieCycle       | Displayed while the round is running.                          | `{count}`, `{name}`     |
| ZombieExtraTime   | Displayed when one person is left or zombies are in "overtime" | `{time}`, `{extratime}` |
| ZombieWin         | Displayed when the game is finished and the zombie team wins.  | `{time}`                |
| ZombieLose        | Displayed when the game is finished and the zombie team loses. | `{time}`                |

| Variables     | Description                              |
|---------------|------------------------------------------|
| `{name}`      | The name of the event.                   |
| `{time}`      | The time elapsed.                        |
| `{extratime}` | The time remaining in "overtime" mode.   |
| `{count}`     | The number of non-infected players left. |
<br>

## Jail
| Translations             | Description                                                                   | Variables                                         |
|--------------------------|-------------------------------------------------------------------------------|---------------------------------------------------|
| JailBeforeStart          | Pre-Round information text for the jailer team.                               | `{time}`, `{name}`                                |
| JailBeforeStartPrisoners | Pre-Round information text for the prisoner team.                             | `{time}`, `{name}`                                |
| JailCycle                | Displayed while the round is running.                                         | `{time}`, `{name}`. `{dclasscount}`, `{mtfcount}` |
| JailLockdownOnCooldown   | Displayed to a user when they try to use the lockdown, but it is on cooldown. | `{cooldown}`                                      |
| JailLivesRemaining       | Displayed to a prisoner when they die but still have lives remaining.         | `{lives}`                                         |
| JailNoLivesRemaining     | Displayed to a prisoner when they die but have no lives remaining.            |                                                   |
| JailPrisonersWin         | Displayed when the game is finished and prisoners win.                        | `{time}`                                          |
| JailJailersWin           | Displayed when the game is finished and the jailers win.                      | `{time}`                                          |

| Variables       | Description                                 |
|-----------------|---------------------------------------------|
| `{name}`        | The name of the event.                      |
| `{time}`        | The time elapsed.                           |
| `{cooldown}`    | The duration of cooldown remaining.         |
| `{dclasscount}` | The number of dclass still alive.           |
| `{mtfcount}`    | The number of jailers still alive.          |
| `{lives}`       | The number of lives a player has remaining. |
<br>

## Knives
| Translations   | Description                                                  | Variables                              |
|----------------|--------------------------------------------------------------|----------------------------------------|
| KnivesCycle    | Displayed while the round is running.                        | `{name}`, `{mtfcount}`, `{chaoscount}` |
| KnivesChaosWin | Displayed when the game is finished and the Chaos team wins. | `{name}`                               |
| KnivesMtfWin   | Displayed when the game is finished and the Mtf Team wins.   | `{name}`                               |

| Variables      | Description                                          |
|----------------|------------------------------------------------------|
| `{name}`       | The name of the event.                               |
| `{mtfcount}`   | The number of players still alive on the Mtf team.   |
| `{chaoscount}` | The number of players still alive on the Chaos team. |
<br>

## Lava
| Translations    | Description                                                       | Variables  |
|-----------------|-------------------------------------------------------------------|------------|
| LavaBeforeStart | Pre-Round information text.                                       | `{time}`   |
| LavaCycle       | Displayed when the game is finished and one person is left alive. | `{winner}` |
| LavaAllDead     | Displayed when the game is finished and everyone died.            |            |

| Variables  | Description                                |
|------------|--------------------------------------------|
| `{time}`   | The time elapsed.                          |
| `{count}`  | The number of players who are still alive. |
| `{winner}` | The name of the player who won.            |
<br>

## Line
| Translations    | Description                                                     | Variables                     |
|-----------------|-----------------------------------------------------------------|-------------------------------|
| LineCycle       | Displayed while the round is running.                           | `{name}`, `{time}`, `{count}` |
| LineMorePlayers | Displayed when the game is finished and multiple players lived. | `{name}`, `{count}`           |
| LineWinner      | Displayed when the game is finished and only one player lived.  | `{name}`, `{winner}`          |
| LineAllDied     | Displayed when the game is finished and nobody survived.        |                               |

| Variables  | Description                                |
|------------|--------------------------------------------|
| `{name}`   | The name of the event.                     |
| `{time}`   | The time elapsed.                          |
| `{count}`  | The number of players who are still alive. |
| `{winner}` | The name of the player that won.           |
<br>

## Puzzle
| Translations           | Description                                                       | Variables                                  |
|------------------------|-------------------------------------------------------------------|--------------------------------------------|
| PuzzleStart            | Pre-Round information text.                                       | `{time}`                                   |
| PuzzleStage            | Displayed while the game is running.                              | `{stageNum}`, `{stageFinal}`, `{plyCount}` |
| PuzzleDied             | Displayed to a player when they die.                              |                                            |
| PuzzleAllDied          | Displayed when the game is finished and nobody survives.          |                                            |
| PuzzleSeveralSurvivors | Displayed when the game is finished and several players survived. | `{winner}`                                 |
| PuzzleWinner           | Displayed when the game is finished and only one player survived. |                                            |

| Variables | Description                              |
|-----------|------------------------------------------|
| `{name}`  | The name of the event.                   |
| `{time}`  | The time elapsed.                        |
| `{count}` | The number of non-infected players left. |
<br>

## Survival
| Translations            | Description                                                                   | Variables                          |
|-------------------------|-------------------------------------------------------------------------------|------------------------------------|
| SurvivalBeforeInfection | Pre-Round information text.                                                   | `{name}`, `{time}`                 |
| SurvivalAfterInfection  | Displayed while the game is running.                                          | `{name}`, `{humanCount}`, `{time}` |
| SurvivalZombieWin       | Displayed when the game is finished and zombies infected all players.         |                                    |
| SurvivalHumanWin        | Displayed when the game is finished and all zombies were killed.              |                                    |
| SurvivalHumanWinTime    | Displayed when the game is finished and some zombies and some players remain. |                                    |

| Variables      | Description                              |
|----------------|------------------------------------------|
| `{name}`       | The name of the event.                   |
| `{time}`       | The time remaining.                      |
| `{humancount}` | The number of non-infected players left. |
<br>

## Versus
| Translations        | Description                                                              | Variables                            |
|---------------------|--------------------------------------------------------------------------|--------------------------------------|
| VersusPlayersNull   | Displayed when neither team has a player inside the arena.               | `{name}`, `{remain}`                 |
| VersusClassDNull    | Displayed when the ClassD team doesn't have a player inside the arena.   | `{name}`, `{remain}`, `{scientist}`  |
| VersusScientistNull | Displayed when the Scientist team doesnt have a player inside the arena. | `{name}`, `{remain}`, `{classd}`     |
| VersusPlayersDuel   | Displayed while two players are dueling inside the arena.                | `{name}`, `{scientist}`, `{classd}`  |
| VersusClassDWin     | Displayed when the game is finished and the ClassD team wins.            | `{name}`                             |
| VersusScientistWin  | Displayed when the game is finished and the Scientist team wins.         | `{name}`                             |

| Variables     | Description                                                  |
|---------------|--------------------------------------------------------------|
| `{name}`      | The name of the event.                                       |
| `{remain}`    | The time remaining before a player is auto-selected to duel. |
| `{scientist}` | The scientist who is in the arena.                           |
| `{classd}`    | The classd who is in the arena.                              |

<br>



## ZombieEscape
| Translations            | Description                                                                                                         | Variables           |
|-------------------------|---------------------------------------------------------------------------------------------------------------------|---------------------|
| ZombieEscapeBeforeStart | Pre-Round information text.                                                                                         | `{name}`, `{time}`  |
| ZombieEscapeHelicopter  | Displayed while the round is running.                                                                               | `{name}`, `{count}` |
| ZombieEscapeDied        | Displayed when the game is finished, and all players who aren't at the end are killed. (This is the damage reason.) |                     |
| ZombieEscapeZombieWin   | Displayed when the game is finished and Zombie's infected all players.                                              |                     |
| ZombieEscapeHumanWin    | Displayed when the game is finished and Humans manage to escape.                                                    |                     |

| Variables | Description                              |
|-----------|------------------------------------------|
| `{name}`  | The name of the event.                   |
| `{time}`  | The time elapsed.                        |
| `{count}` | The number of non-infected players left. |
<br>