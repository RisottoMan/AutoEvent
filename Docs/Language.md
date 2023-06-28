# Language
## You can change the language in {port}-translations.yml
## English (default)
```
auto_event:
  # Zombie Infection Game Mode
  zombie_name: Zombie Infection
  zombie_description: Zombie mode, the purpose of which is to infect all players.
  zombie_before_start: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=#ABF000>There are <color=red>{time}</color> seconds left before the game starts.</color>
  zombie_cycle: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Humans left: <color=green>{count}</color></color>

    <color=yellow>Event time <color=red>{time}</color></color>
  zombie_extra_time: >-
    Extra time: {extratime}

    <color=yellow>The <b><i>Last</i></b> person left!</color>

    <color=yellow>Event time <color=red>{time}</color></color>
  zombie_win: >-
    <color=red>Zombie Win!</color>

    <color=yellow>Event time <color=red>{time}</color></color>
  zombie_lose: >-
    <color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>

    <color=yellow>Event time <color=red>{time}</color></color>
  # Atomic Escape Game Mode
  escape_name: Atomic Escape
  escape_description: Escape from the facility behind SCP-173 at supersonic speed!
  escape_before_start: {name}\nHave time to escape from the facility before it explodes!\n<color=red>Before the escape: {time} seconds</color>
  escape_cycle: >-
    {name}

    Before the explosion: <color=red>{time}</color> seconds
  escape_end: >-
    {name}

    <color=red> SCP Win </color>
  # Simon's Prison Game Mode
  jail_name: Simon's Prison
  jail_description: Jail mode from CS 1.6, in which you need to hold events [VERY HARD].
  jail_before_start: >-
    <color=yellow><color=red><b><i>{name}</i></b></color>

    <i>Open the doors to the players by shooting the button</i>

    Before the start: <color=red>{time}</color> seconds</color>
  jail_cycle: >-
    <size=20><color=red>{name}</color>

    <color=yellow>Prisoners: {dclasscount}</color> || <color=blue>Jailers: {mtfcount}</color>

    <color=red>{time}</color></size>
  jail_prisoners_win: >-
    <color=red><b><i>Prisoners Win</i></b></color>

    <color=red>{time}</color>
  jail_jailers_win: >-
    <color=blue><b><i>Jailers Win</i></b></color>

    <color=red>{time}</color>
  # Cock Fights Game Mode
  versus_name: Cock Fights
  versus_description: Duel of players on the 35hp map from cs 1.6
  versus_players_null: >-
    <color=#D71868><b><i>{name}</i></b></color>

    Go inside the arena to fight each other!
  versus_class_d_null: >-
    <color=#D71868><b><i>{name}</i></b></color>

    The player left alive <color=yellow>{scientist}</color>
  versus_scientist_null: >-
    <color=#D71868><b><i>{name}</i></b></color>

    The player left alive <color=orange>{classd}</color>
  versus_players_duel: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>
  versus_class_d_win: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=red>CLASS D</color></color>
  versus_scientist_win: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=red>SCIENTISTS</color></color>
  # Knives of Death Game Mode
  knives_name: Knives of Death
  knives_description: Knife players against each other on a 35hp map from cs 1.6
  knives_cycle: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=blue>{mtfcount} MTF</color> <color=red>VS</color> <color=green>{chaoscount} CHAOS</color></color>
  knives_chaos_win: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=green>CHAOS</color></color>
  knives_mtf_win: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>
  # Deathmatch Game Mode
  deathmatch_name: Territory of Death
  deathmatch_description: Cool Deathmatch on the Shipment map from MW19
  deathmatch_cycle: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>
  deathmatch_chaos_win: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=green>CHAOS</color></color>
  deathmatch_mtf_win: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>
  # GunGame Game Mode
  gun_game_name: Quick Hands
  gun_game_description: Cool GunGame on the Shipment map from MW19.
  gun_game_cycle: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#D71868>{level}</color> LVL <color=#D71868>||</color> Need <color=#D71868>{kills}</color> kills</color></b>
  gun_game_winner: >-
    <color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>The Winner of the game: <color=green>{winner}</color></color>
```
## Russian
Jesus not again
