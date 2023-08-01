# Language
## 你可以在这个文件当中调整它们 {port}-translations.yml
## Chinese
```
auto_event:
# Zombie Infection Game Mode
  zombie_name: '僵尸模式'
  zombie_description: '僵尸模式，目标是感染全部的玩家'
  zombie_before_start: '<color=#D71868><b><i>{name}</i></b></color>

    <color=#ABF000>游戏即将开始 <color=red>剩余{time}秒</color>.</color>'
  zombie_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>剩余人类: <color=green>{count}</color></color>

    <color=yellow>游戏时间 <color=red>{time}</color></color>'
  zombie_extra_time: '额外时间: {extratime}

    <color=yellow>仅剩 <b><i>最后一名</i></b> 人类!</color>

    <color=yellow>游戏时间 <color=red>{time}</color></color>'
  zombie_win: '<color=red>僵尸胜利!</color>

    <color=yellow>游戏时间 <color=red>{time}</color></color>'
  zombie_lose: '<color=yellow><color=#D71868><b><i>人类</i></b></color> 胜利!</color>

    <color=yellow>Event time <color=red>{time}</color></color>'
  # Atomic Escape Game Mode
  escape_name: '原子逃脱'
  escape_description: '使用超音速的SCP-173逃离核爆的设施'
  escape_before_start: '{name}

    在设施爆炸之前有一定时间逃离

    <color=red>剩余时间: {time} 秒</color>'
  escape_cycle: '{name}

    剩余时间: <color=red>{time}</color> 秒'
  escape_end: '{name}

    <color=red> SCP胜利 </color>'
  # Simon's Prison Game Mode
  jail_name: '西蒙的监狱'
  jail_description: '来自CS1.6的监狱模式，你需要自己主持游戏[非常困难].'
  jail_before_start: '<color=yellow><color=red><b><i>{name}</i></b></color>

    <i>通过射击按钮为玩家开门</i>

    游戏即将开始，剩余 <color=red>{time}</color> 秒</color>'
  jail_cycle: '<size=20><color=red>{name}</color>

    <color=yellow>囚犯: {dclasscount}</color> || <color=#14AAF5>狱警: {mtfcount}</color>

    <color=red>{time}</color></size>'
  jail_prisoners_win: '<color=red><b><i>囚犯胜利</i></b></color>

    <color=red>{time}</color>'
  jail_jailers_win: '<color=#14AAF5><b><i>狱警胜利</i></b></color>

    <color=red>{time}</color>'
  # Cock Fights Game Mode
  versus_name: '斗鸡'
  versus_description: '在CS 1.6的35hp地图上，进行1V1对决'
  versus_players_null: '<color=#D71868><b><i>{name}</i></b></color>

    进入竞技场互相对战吧！

    剩余<color=red>{remain}</color>秒'
  versus_class_d_null: '<color=#D71868><b><i>{name}</i></b></color>

    存活下来的玩家是<color=yellow>{scientist}</color>

    进入竞技场 剩余 <color=orange>{remain}</color> 秒'
  versus_scientist_null: '<color=#D71868><b><i>{name}</i></b></color>

    存活下来的玩家是 <color=orange>{classd}</color>

    进入竞技场 剩余 <color=yellow>{remain}</color> 秒'
  versus_players_duel: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>'
  versus_class_d_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=red>CLASS D</color></color>'
  versus_scientist_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=red>SCIENTISTS</color></color>'
  # Knives of Death Game Mode
  knives_name: '死亡之刃'
  knives_description: 在CS 1.6的35hp地图上，进行刀战'
  knives_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=blue>{mtfcount} MTF</color> <color=red>VS</color> <color=green>{chaoscount} CHAOS</color></color>'
  knives_chaos_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=green>CHAOS</color></color>'
  knives_mtf_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=#42AAFF>MTF</color></color>'
  # Deathmatch Game Mode
  deathmatch_name: '死斗'
  deathmatch_description: '在 使命召唤19 的地图 混战码头 上进行热血死斗'
  deathmatch_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>'
  deathmatch_chaos_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=green>CHAOS</color></color>'
  deathmatch_mtf_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=#42AAFF>MTF</color></color>'
  # GunGame Game Mode
  gun_game_name: '快枪手(武器大师)'
  gun_game_description: '在 使命召唤19 的 混战码头地图上进行 快枪手(武器大师).'
  gun_game_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#D71868>{level}</color> 级 <color=#D71868>||</color> 需要击杀 <color=#D71868>{kills}</color> 个玩家升级</color></b>'
  gun_game_winner: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>胜利者: <color=green>{winner}</color></color>'
  # Battle Game Mode
  battle_name: '对决'
  battle_description: 'MTF 对战 CI'
  battle_time_left: '<size=100><color=red>游戏即将开始，剩余 {time} 秒</color></size>'
  battle_ci_win: '<color=#299438>胜利者: 混沌分裂者 </color>

    <color=red>游戏时间: {time} </color>'
  battle_mtf_win: '<color=#14AAF5>胜利者：基金会阵营</color>

    <color=red>Event time: {time} </color>'
  battle_counter: "<color=#14AAF5> MTF: {FoundationForces} </color> vs <color=#299438> CI: {ChaosForces} </color> \n 已经进行时间: {time}"
  # Football Game Mode
  football_name: '足球'
  football_description: '先得到3分胜利'
  football_red_team: '<color=red>你是红队

    </color>'
  football_blue_team: '<color=#14AAF5>你是蓝队

    </color>'
  football_time_left: '<color=#14AAF5>{BluePnt}</color> : <color=red>{RedPnt}</color>

    剩余时间: {eventTime}'
  football_red_wins: '<color=red>红队胜利</color>'
  football_blue_wins: '<color=#14AAF5>蓝队胜利</color>'
  football_draw: '计分板

    <color=#14AAF5>{BluePnt}</color> vs <color=red>{RedPnt}</color>'
  # Dead Jump Game Mode (Glass)
  glass_name: '死亡跳跃'
  glass_description: '跳跃在脆弱的地板上'
  glass_start: "<size=50>死亡跳跃\n只有一边地板是可以踩的</size>\n<size=20>剩余玩家: {plyAlive} \n剩余时间: {eventTime}</size>"
  glass_died: '你掉进了岩浆'
  glass_win_survived: '<color=yellow>人类胜利剩余 {countAlive} 个玩家</color>'
  glass_winner: '<color=red>死亡跳跃</color>

    <color=yellow>胜利者: {winner}</color>'
  glass_fail: '<color=red>死亡跳跃</color>

    <color=yellow>无人幸存</color>'
  # Puzzle Game Mode
  puzzle_name: '谜题'
  puzzle_description: '踩到正确的颜色上,速度将会越来越快.'
  puzzle_start: '<color=red>即将开始: </color>%time%'
  puzzle_stage: '<color=red>回合: </color>%stageNum%<color=red> / </color>%stageFinal%

    <color=yellow>剩余玩家:</color> %plyCount%'
  puzzle_all_died: '<color=red>无人幸存</color>

    游戏结束'
  puzzle_several_survivors: '<color=red>许多玩家幸存</color>

    游戏结束'
  puzzle_winner: '<color=red>胜利者: %plyWinner%</color>

   游戏结束'
  puzzle_died: '<color=red>掉入岩浆</color>'
  # Zombie Survival Game Mode (Zombie 2)
  survival_name: '僵尸生存'
  survival_description: '从感染中幸存'
  survival_before_infection: '<b>%name%</b>

    <color=yellow>感染即将开始，剩余 </color> %time% <color=yellow>秒</color>'
  survival_after_infection: '<b>%name%</b>

    <color=#14AAF5>人类:</color> %humanCount%

    <color=#299438>剩余时间:</color> %time%'
  survival_zombie_win: '<color=red>僵尸感染了全部的人类!</color>'
  survival_human_win: '<color=#14AAF5>人类击杀了全部的僵尸</color>'
  survival_human_win_time: '<color=yellow>人类胜利，但是感染没有消除</color>'
  # Fall Down Game Mode
  fall_name: '掉落'
  fall_description: '所有的平台将会掉落，努力的存过下去'
  fall_broadcast: '%name%

    %time%

    <color=yellow>剩余: </color>%count%<color=yellow> 个玩家</color>'
  fall_winner: '<color=red>胜利者:</color> %winner%'
  fall_died: '<color=red>所有玩家已死亡</color>'
  # Death Line Game Mode
  line_name: '死亡线'
  line_description: '避免碰到红线存活下去.'
  line_broadcast: '%name%

    %time%

    <color=yellow>剩余: </color>%count%<color=yellow> 个玩家</color>'
  line_winner: '<color=red>胜利者:</color> %winner%'
  line_all_died: '<color=red>无人幸存</color>'
  line_died: '你失败了...'
  # Down Cubes Game Mode
  cube_name: '下降方块'
  cube_description: '方块下降....'
  cube_broadcast: '%name%

    %time%

    <color=yellow>剩余: </color>%count%<color=yellow> 个玩家</color>'
  cube_winner: '<color=red>Winner:</color> %winner%'
  cube_all_died: '<color=red>无人幸存</color>'
  cube_died: '你死了...'
  # Hide And Seek Game Mode
  hide_name: '躲猫猫'
  hide_description: '找到所有在地图当中藏起来的玩家.'
  hide_broadcast: '散开

    正在寻找新的抓人的人.

    %time%'
  hide_cycle: '把球棒传给别人

    <color=yellow剩余><b><i>%time%</i></b> 秒</color>'
  hide_hurt: '你没有将球棒及时传出去.'
  hide_more_player: '还有很多玩家在躲着.

    等待重启.

    <color=yellow>游戏已进行 <color=red>%time%</color></color>'
  hide_one_player: '胜利者 %winner%

    <color=yellow>游戏已进行 <color=red>%time%</color></color>'
  hide_all_die: '无人幸存

    游戏结束

    <color=yellow>游戏已进行<color=red>%time%</color></color>'
  # Death Party Game Mode
  death_name: '死亡派对'
  death_description: '从手雷雨中幸存'
  death_cycle: '<color=yellow>远离手雷!</color>

    <color=green>已经坚持了%time%</color>

    <color=red>剩余 %count% 名玩家</color>'
  death_more_player: '<color=red>死亡派对</color>

    <color=yellow>剩余<color=red>%count%</color> 名玩家.</color>

    <color=#ffc0cb>%time%</color>'
  death_one_player: '<color=red>死亡派对</color>

    <color=yellow>胜利者 - <color=red>%winner%</color></color>

    <color=#ffc0cb>%time%</color>'
  death_all_die: '<color=red>死亡派对</color>

    <color=yellow>无人幸存.(((</color>

    <color=#ffc0cb>%time%</color>'
  # FinishWay Game Mode
  finish_way_name: '<color=yellow>到达终点</color>'
  finish_way_description: '到达终点获得胜利.'
  finish_way_cycle: '%name%

    <color=yellow>通过终点!</color>

    剩余时间: %time%'
  finish_way_died: '你没有到达终点'
  finish_way_several_survivors: '<color=red>人类胜利!</color>

    剩余 %count%个人'
  finish_way_one_survived: '<color=red>人类胜利!</color>

    胜利者: %player%'
  finish_way_no_survivors: '<color=red>无人幸存</color>'
  # Zombie Escape Game Mode
  zombie_escape_name: '僵尸逃脱'
  zombie_escape_description: '你需要通过直升飞机逃离僵尸'
  zombie_escape_before_start: '<color=#D71868><b><i>向前跑</i></b></color>

    感染即将开始，剩余 %time%'
  zombie_escape_helicopter: '<color=yellow>%name%</color>

    <color=red>需要呼叫直升机.</color>

    剩余人类: %count%'
  zombie_escape_died: 'Warhead detonated'
  zombie_escape_zombie_win: '<color=red>僵尸胜利!</color>

    所有玩家已死亡'
  zombie_escape_human_win: '<color=#14AAF5>人类胜利!</color>

    人类逃脱'
  # Lava Game Mode
  lava_name: '岩浆射击'
  lava_description: '岩浆地板将会不断地上升，占领高地，捡起武器，存活下去'
  lava_before_start: '<size=100><color=red>%time%</color></size>

    捡起武器，并且往上爬'
  lava_cycle: '<size=20><color=red><b>存活: %count% 名玩家</b></color></size>'
  lava_win: '<color=red><b>胜利者

    玩家 - %winner%</b></color>'
  lava_all_dead: '<color=red><b>无人幸存.</b></color>'
  # Boss Battle Game Mode
  boss_name: '打Boos'
  boss_description: '你需要击败Boos.'
  boss_time_left: '<size=100><color=red>即将开始 {time} </color></size>'
  boss_win: '<color=red><b>Boss 胜利</b></color>

    <color=yellow><color=#14AAF5>人类</color> 被击败</color>

    <b>剩余<color=red>%hp%</color> Hp</b> '
  boss_humans_win: '<color=#14AAF5>人类胜利</color>

    <color=yellow><color=red>Boss</color> 被击败</color>

    <color=#14AAF5>%count%</color> 名玩家存活'
  boss_counter: '<color=red><b>%hp% HP</b></color>

    <color=#14AAF5>%count%</color> 名玩家存活

    剩余时间:<color=green>%time%</color>'
```
