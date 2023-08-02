# Language
## You can change the language in {port}-translations.yml
## English (default)
```
auto_event:
# Zombie Infection Game Mode
  zombie_name: 'Zombie Infection'
  zombie_description: 'Zombie mode, the purpose of which is to infect all players.'
  zombie_before_start: '<color=#D71868><b><i>{name}</i></b></color>

    <color=#ABF000>There are <color=red>{time}</color> seconds left before the game starts.</color>'
  zombie_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Humans left: <color=green>{count}</color></color>

    <color=yellow>Event time <color=red>{time}</color></color>'
  zombie_extra_time: 'Extra time: {extratime}

    <color=yellow>The <b><i>Last</i></b> person left!</color>

    <color=yellow>Event time <color=red>{time}</color></color>'
  zombie_win: '<color=red>Zombie Win!</color>

    <color=yellow>Event time <color=red>{time}</color></color>'
  zombie_lose: '<color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>

    <color=yellow>Event time <color=red>{time}</color></color>'
  # Atomic Escape Game Mode
  escape_name: 'Atomic Escape'
  escape_description: 'Escape from the facility behind SCP-173 at supersonic speed!'
  escape_before_start: '{name}

    Have time to escape from the facility before it explodes!

    <color=red>Before the escape: {time} seconds</color>'
  escape_cycle: '{name}

    Before the explosion: <color=red>{time}</color> seconds'
  escape_end: '{name}

    <color=red> SCP Win </color>'
  # Simon's Prison Game Mode
  jail_name: 'Simon''s Prison'
  jail_description: 'Jail mode from CS 1.6, in which you need to hold events [VERY HARD].'
  jail_before_start: '<color=yellow><color=red><b><i>{name}</i></b></color>

    <i>Open the doors to the players by shooting the button</i>

    Before the start: <color=red>{time}</color> seconds</color>'
  jail_cycle: '<size=20><color=red>{name}</color>

    <color=yellow>Prisoners: {dclasscount}</color> || <color=#14AAF5>Jailers: {mtfcount}</color>

    <color=red>{time}</color></size>'
  jail_prisoners_win: '<color=red><b><i>Prisoners Win</i></b></color>

    <color=red>{time}</color>'
  jail_jailers_win: '<color=#14AAF5><b><i>Jailers Win</i></b></color>

    <color=red>{time}</color>'
  # Cock Fights Game Mode
  versus_name: 'Cock Fights'
  versus_description: 'Duel of players on the 35hp map from cs 1.6'
  versus_players_null: '<color=#D71868><b><i>{name}</i></b></color>

    Go inside the arena to fight each other!

    <color=red>{remain}</color> seconds left'
  versus_class_d_null: '<color=#D71868><b><i>{name}</i></b></color>

    The player left alive <color=yellow>{scientist}</color>

    Go inside in <color=orange>{remain}</color> seconds'
  versus_scientist_null: '<color=#D71868><b><i>{name}</i></b></color>

    The player left alive <color=orange>{classd}</color>

    Go inside in <color=yellow>{remain}</color> seconds'
  versus_players_duel: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>'
  versus_class_d_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=red>CLASS D</color></color>'
  versus_scientist_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=red>SCIENTISTS</color></color>'
  # Knives of Death Game Mode
  knives_name: 'Knives of Death'
  knives_description: 'Knife players against each other on a 35hp map from cs 1.6'
  knives_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=blue>{mtfcount} MTF</color> <color=red>VS</color> <color=green>{chaoscount} CHAOS</color></color>'
  knives_chaos_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=green>CHAOS</color></color>'
  knives_mtf_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>'
  # Deathmatch Game Mode
  deathmatch_name: 'Territory of Death'
  deathmatch_description: 'Cool Deathmatch on the Shipment map from MW19'
  deathmatch_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>'
  deathmatch_chaos_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=green>CHAOS</color></color>'
  deathmatch_mtf_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>'
  # GunGame Game Mode
  gun_game_name: 'Quick Hands'
  gun_game_description: 'Cool GunGame on the Shipment map from MW19.'
  gun_game_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#D71868>{level}</color> LVL <color=#D71868>||</color> Need <color=#D71868>{kills}</color> kills</color>

    <color=#D71868>Leader: <color=yellow>{leadnick}</color> LVL <color=yellow>{leadlevel}</color></color></b>'
  gun_game_winner: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>The Winner of the game: <color=green>{winner}</color></color>'
  # Battle Game Mode
  battle_name: 'Battle'
  battle_description: 'MTF fight against CI'
  battle_time_left: '<size=100><color=red>Starts in {time} </color></size>'
  battle_ci_win: '<color=#299438>Winner: Chaos Insurgency </color>

    <color=red>Event time: {time} </color>'
  battle_mtf_win: '<color=#14AAF5>Winner: Foundation forces</color>

    <color=red>Event time: {time} </color>'
  battle_counter: "<color=#14AAF5> MTF: {FoundationForces} </color> vs <color=#299438> CI: {ChaosForces} </color> \n Elapsed time: {time}"
  # Football Game Mode
  football_name: 'Football'
  football_description: 'Score 3 goals to win'
  football_red_team: '<color=red>You play as Red Team

    </color>'
  football_blue_team: '<color=#14AAF5>You play as Blue Team

    </color>'
  football_time_left: '<color=#14AAF5>{BluePnt}</color> : <color=red>{RedPnt}</color>

    Time left: {eventTime}'
  football_red_wins: '<color=red>Red Team Wins</color>'
  football_blue_wins: '<color=#14AAF5>Blue Team Wins</color>'
  football_draw: 'Draw

    <color=#14AAF5>{BluePnt}</color> vs <color=red>{RedPnt}</color>'
  # Dead Jump Game Mode (Glass)
  glass_name: 'Dead Jump'
  glass_description: 'Jump on fragile platforms'
  glass_start: "<size=50>Dead Jump\nJump on fragile platforms</size>\n<size=20>Alive players: {plyAlive} \nTime left: {eventTime}</size>"
  glass_died: 'You fallen into lava'
  glass_win_survived: '<color=yellow>Human wins! Survived {countAlive} players</color>'
  glass_winner: '<color=red>Dead Jump</color>

    <color=yellow>Winner: {winner}</color>'
  glass_fail: '<color=red>Dead Jump</color>

    <color=yellow>All players died</color>'
  # Puzzle Game Mode
  puzzle_name: 'Puzzle'
  puzzle_description: 'Get up the fastest on the right color.'
  puzzle_start: '<color=red>Starts in: </color>%time%'
  puzzle_stage: '<color=red>Stage: </color>%stageNum%<color=red> / </color>%stageFinal%

    <color=yellow>Remaining players:</color> %plyCount%'
  puzzle_all_died: '<color=red>All players died</color>

    Mini-game ended'
  puzzle_several_survivors: '<color=red>Several people survived</color>

    Mini-game ended'
  puzzle_winner: '<color=red>Winner: %plyWinner%</color>

    Mini-game ended'
  puzzle_died: '<color=red>Burned in Lava</color>'
  # Zombie Survival Game Mode (Zombie 2)
  survival_name: 'Zombie Survival'
  survival_description: 'Humans surviving from zombies'
  survival_before_infection: '<b>%name%</b>

    <color=yellow>There are </color> %time% <color=yellow>second before infection begins</color>'
  survival_after_infection: '<b>%name%</b>

    <color=#14AAF5>Humans:</color> %humanCount%

    <color=#299438>Time to the end:</color> %time%'
  survival_zombie_win: '<color=red>Zombie infected all humans and wins!</color>'
  survival_human_win: '<color=#14AAF5>Humans killed all zombies and stopped infection</color>'
  survival_human_win_time: '<color=yellow>Humans survived, but infection is not stopped</color>'
  # Fall Down Game Mode
  fall_name: 'FallDown'
  fall_description: 'All platforms are destroyed. It is necessary to survive'
  fall_broadcast: '%name%

    %time%

    <color=yellow>Remaining: </color>%count%<color=yellow> players</color>'
  fall_winner: '<color=red>Winner:</color> %winner%'
  fall_died: '<color=red>All players died</color>'
  # Death Line Game Mode
  line_name: 'DeathLine'
  line_description: 'Avoid the spinning platform to survive.'
  line_cycle: '<color=#FF4242>%name%</color>

    <color=#14AAF5>Time to end: %min%</color><color=#4a4a4a>:</color><color=#14AAF5>%sec%</color>

    <color=yellow>Players: %count%</color>'
  line_more_players: '<color=#FF4242>%name%</color>

    <color=yellow>%count% players survived</color>

    <color=red>Congratulate!</color>'
  line_winner: '<color=#FF4242>%name%</color>

    <color=yellow>Winner: %winner%</color>

    <color=red>Congratulate!</color>'
  line_all_died: '<color=red>All players died</color>'
  # Down Cubes Game Mode
  cube_name: 'Down Cubes'
  cube_description: 'Cubes down....'
  cube_broadcast: '%name%

    %time%

    <color=yellow>Remaining: </color>%count%<color=yellow> players</color>'
  cube_winner: '<color=red>Winner:</color> %winner%'
  cube_all_died: '<color=red>All players died</color>'
  cube_died: 'You died...'
  # Hide And Seek Game Mode
  hide_name: 'Hide And Seek'
  hide_description: 'We need to catch up with all the players on the map.'
  hide_broadcast: 'RUN

    Selection of new catching up players.

    %time%'
  hide_cycle: 'Pass the bat to another player

    <color=yellow><b><i>%time%</i></b> seconds left</color>'
  hide_hurt: 'You didn''t have time to pass the bat.'
  hide_more_player: 'There are a lot of players left.

    Waiting for a reboot.

    <color=yellow>Event time <color=red>%time%</color></color>'
  hide_one_player: 'The player won %winner%

    <color=yellow>Event time <color=red>%time%</color></color>'
  hide_all_die: 'No one survived.

    End of the game

    <color=yellow>Event time <color=red>%time%</color></color>'
  # Death Party Game Mode
  death_name: 'Death Party'
  death_description: 'Survive in grenade rain.'
  death_cycle: '<color=yellow>Dodge the grenades!</color>

    <color=green>%time% seconds passed</color>

    <color=red>%count% players left</color>'
  death_more_player: '<color=red>Death Party</color>

    <color=yellow><color=red>%count%</color> players survived.</color>

    <color=#ffc0cb>%time%</color>'
  death_one_player: '<color=red>Death Party</color>

    <color=yellow>Winner - <color=red>%winner%</color></color>

    <color=#ffc0cb>%time%</color>'
  death_all_die: '<color=red>Death Party</color>

    <color=yellow>No one survived.(((</color>

    <color=#ffc0cb>%time%</color>'
  # FinishWay Game Mode
  finish_way_name: 'Finish Way'
  finish_way_description: 'Go to the end of the finish to win.'
  finish_way_cycle: '%name%

    <color=yellow>Pass the finish!</color>

    Time left: %time%'
  finish_way_died: 'You didnt pass the finish'
  finish_way_several_survivors: '<color=red>Human wins!</color>

    Survived %count%'
  finish_way_one_survived: '<color=red>Human wins!</color>

    Winner: %player%'
  finish_way_no_survivors: '<color=red>No one human survived</color>'
  # Zombie Escape Game Mode
  zombie_escape_name: 'Zombie Escape'
  zombie_escape_description: 'Уou need to run away from zombies and escape by helicopter.'
  zombie_escape_before_start: '<color=#D71868><b><i>Run Forward</i></b></color>

    Infection starts in: %time%'
  zombie_escape_helicopter: '<color=yellow>%name%</color>

    <color=red>Need to call helicopter.</color>

    Humans left: %count%'
  zombie_escape_died: 'Warhead detonated'
  zombie_escape_zombie_win: '<color=red>Zombies wins!</color>

    All humans died'
  zombie_escape_human_win: '<color=#14AAF5>Humans wins!</color>

    Humans escaped'
  # Lava Game Mode
  lava_name: 'The floor is LAVA'
  lava_description: 'Survival, in which you need to avoid lava and shoot at others.'
  lava_before_start: '<size=100><color=red>%time%</color></size>

    Take weapons and climb up.'
  lava_cycle: '<size=20><color=red><b>Alive: %count% players</b></color></size>'
  lava_win: '<color=red><b>Winner

    Player - %winner%</b></color>'
  lava_all_dead: '<color=red><b>No one survived to the end.</b></color>'
  # Boss Battle Game Mode
  boss_name: 'Boss Battle'
  boss_description: 'You need kill the Boss.'
  boss_time_left: '<size=100><color=red>Starts in {time} </color></size>'
  boss_win: '<color=red><b>Boss WIN</b></color>

    <color=yellow><color=#14AAF5>Humans</color> has been destroyed</color>

    <b><color=red>%hp%</color> Hp</b> left'
  boss_humans_win: '<color=#14AAF5>Humans WIN</color>

    <color=yellow><color=red>Boss</color> has been destroyed</color>

    <color=#14AAF5>%count%</color> players left'
  boss_counter: '<color=red><b>%hp% HP</b></color>

    <color=#14AAF5>%count%</color> players left

    <color=green>%time%</color> seconds left'
```
