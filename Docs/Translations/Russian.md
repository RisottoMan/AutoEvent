# Language
## Вы должны изменить текст в файле {port}-translations.yml
## Russian
```
auto_event:
# Zombie Infection Game Mode
  zombie_name: 'Зомби заражение'
  zombie_description: 'Зомби должны заразить всех людей'
  zombie_before_start: '<color=#D71868><b><i>{name}</i></b></color>

    <color=#ABF000>До начала игры осталось: <color=red>{time}</color></color>'
  zombie_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Осталось людей: <color=green>{count}</color></color>

    <color=yellow>Время <color=red>{time}</color></color>'
  zombie_extra_time: 'Дополнительное время: {extratime}

    <color=yellow><b><i>Последний</i></b> выживший!</color>

    <color=yellow>Время <color=red>{time}</color></color>'
  zombie_win: '<color=red>Победа Зомби!</color>

    <color=yellow>Время <color=red>{time}</color></color>'
  zombie_lose: '<color=yellow><color=#D71868><b><i>Люди</i></b></color> выиграли!</color>

    <color=yellow>Время <color=red>{time}</color></color>'
  # Atomic Escape Game Mode
  escape_name: 'Атомный побег'
  escape_description: 'Сбегите из комплекса до взрыва боеголовки!'
  escape_before_start: '{name}

    Сбегите из комплекса до взрыва боеголовки!

    <color=red>Старт через: {time}</color>'
  escape_cycle: '{name}

    До взрыва: <color=red>{time}</color>'
  escape_end: '{name}

    <color=red> SCP победили </color>'
  # Simon's Prison Game Mode
  jail_name: 'Тюрьма Саймона'
  jail_description: 'Тюрьма из CS 1.6 в которой нужно проводить ивенты [VERY HARD].'
  jail_before_start: '<color=yellow><color=red><b><i>{name}</i></b></color>

    <i>Откройте двери клеток выстрелом в кнопку</i>

    До начала: <color=red>{time}</color></color>'
  jail_cycle: '<size=20><color=red>{name}</color>

    <color=yellow>Заключенные: {dclasscount}</color> || <color=blue>Надзиратели: {mtfcount}</color>

    <color=red>{time}</color></size>'
  jail_prisoners_win: '<color=red><b><i>Победа Заключенных</i></b></color>

    <color=red>{time}</color>'
  jail_jailers_win: '<color=blue><b><i>Надзиратели выиграли</i></b></color>

    <color=red>{time}</color>'
  # Cock Fights Game Mode
  versus_name: 'Петушиные бои'
  versus_description: 'Дуэль на карте 35hp из CS 1.6'
  versus_players_null: '<color=#D71868><b><i>{name}</i></b></color>

    Зайдите на арену для начала боя!'
  versus_class_d_null: '<color=#D71868><b><i>{name}</i></b></color>

    Игрок на арене <color=yellow>{scientist}</color>'
  versus_scientist_null: '<color=#D71868><b><i>{name}</i></b></color>

    Игрок на арене <color=orange>{classd}</color>'
  versus_players_duel: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>'
  versus_class_d_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победители: <color=red>CLASS D</color></color>'
  versus_scientist_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победители: <color=red>SCIENTISTS</color></color>'
  # Knives of Death Game Mode
  knives_name: 'Ножи смерти'
  knives_description: 'Деритесь на карте 35hp из CS 1.6'
  knives_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow><color=blue>{mtfcount} МОГ</color> <color=red>VS</color> <color=green>{chaoscount} ПХ</color></color>'
  knives_chaos_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победители: <color=green>ПХ</color></color>'
  knives_mtf_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победители: <color=#42AAFF>МОГ</color></color>'
  # Deathmatch Game Mode
  deathmatch_name: 'Зона смерти'
  deathmatch_description: 'Дезматч на карте Shipment из MW19'
  deathmatch_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>'
  deathmatch_chaos_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победители: <color=green>ПХ</color></color>'
  deathmatch_mtf_win: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победители: <color=#42AAFF>МОГ</color></color>'
  # GunGame Game Mode
  gun_game_name: 'Гонка вооружений'
  gun_game_description: 'Гонка вооружений на карте Shipment из MW19'
  gun_game_cycle: '<color=#D71868><b><i>{name}</i></b></color>

    <b><color=yellow><color=#D71868>{level}</color> LVL <color=#D71868>||</color> Нужно <color=#D71868>{kills}</color> убийства</color></b>'
  gun_game_winner: '<color=#D71868><b><i>{name}</i></b></color>

    <color=yellow>Победитель: <color=green>{winner}</color></color>'
  # Battle Game Mode
  battle_name: 'Баттл'
  battle_description: 'МОГ против Хаоса'
  battle_time_left: '<size=100><color=red>Starts in {time} </color></size>'
  battle_ci_win: '<color=green>Победа Повстанцев Хаоса </color>

    <color=red>Время: {time} </color>'
  battle_mtf_win: '<color=blue>Победа Фонда</color>

    <color=red>Время: {time} </color>'
  battle_counter: "<color=blue> МОГ: {FoundationForces} </color> vs <color=green> ПХ: {ChaosForces} </color> \n Прошло: {time}"
  # Football Game Mode
  football_name: 'Футбол'
  football_description: 'Забей 3 гола для победы'
  football_red_team: '<color=red>Вы играете за Красную команду

    </color>'
  football_blue_team: '<color=blue>Вы играете за Синюю команду

    </color>'
  football_time_left: '<color=blue>{BluePnt}</color> : <color=red>{RedPnt}</color>

    Осталось: {eventTime}'
  football_red_wins: '<color=red>Красные победили</color>'
  football_blue_wins: '<color=blue>Синие победили</color>'
  football_tie: 'Ничья

    <color=blue>{BluePnt}</color> vs <color=red>{RedPnt}</color>'
  # Dead Jump Game Mode (Glass)
  glass_name: 'Прыжок смерти'
  glass_description: 'Прыгайте по хрупким плафтормам'
  glass_start: "<size=50>Прыжок смерти\nПрыгайте по стеклянным плафтормам</size>\n<size=20>Живых игроков: {plyAlive} \nОсталось времени: {eventTime}</size>"
  glass_died: 'Сгорел в лаве'
  glass_win_survived: '<color=yellow>Победа людей! Выжило {countAlive} игроков</color>'
  glass_winner: '<color=red>Прыжок смерти</color>

    <color=yellow>Победил: {winner}</color>'
  glass_fail: '<color=red>Прыжок смерти</color>

    <color=yellow>Все игроки погибли</color>'
  # Puzzle Game Mode
  puzzle_name: 'Пазл'
  puzzle_description: 'Встаньте на верный цвет'
  puzzle_start: '<color=red>Начало через: </color>%time%'
  puzzle_stage: '<color=red>Уровень: </color>%stageNum%<color=red> / </color>%stageFinal%

    <color=yellow>Осталось живых:</color> %plyCount%'
  puzzle_all_died: '<color=red>Все игроки погибли</color>

    Конец игры.'
  puzzle_several_survivors: '<color=red>Несколько выживших игроков</color>

    Конец игры'
  puzzle_winner: '<color=red>Победитель: %plyWinner%</color>

    Конец игры'
  puzzle_died: '<color=red>Сгорел в лаве</color>'
  # Zombie Survival Game Mode (Zombie 2)
  survival_name: 'Зомби Выживание'
  survival_description: 'Выживание в окружении зомби'
  survival_before_infection: '<b><color=red>Зомби Выживание</color></b>

    <color=yellow>Осталось</color> %time% <color=yellow>секунд до начала заражения</color>'
  survival_after_infection: '<b>%name%</b>

    <color=red>Люди:</color> %humanCount%

    <color=green>Время до конца:</color> %time%'
  survival_zombie_win: '<color=red>Зомби заразили всех людей!</color>'
  survival_human_win: '<color=blue>Зомби убили всех зомби и остановили инфекцию</color>'
  survival_human_win_time: '<color=yellow>Люди выжили, но не остановили инфекцию</color>'
  # Fall Down Game Mode
  fall_name: 'Падение'
  fall_description: 'Платформы разрушаются под вашими ногами.'
  fall_broadcast: '%name%

    %time%

    <color=yellow>Осталось: </color>%count%<color=yellow> игроков</color>'
  fall_winner: '<color=red>Победитель:</color> %winner%'
  fall_died: '<color=red>Все игроки погибли</color>'
  # Death Line Game Mode
  line_name: 'Линия смерти'
  line_description: 'Избегай крутящейся платформы чтобы выживания.'
  line_broadcast: '%name%

    %time%

    <color=yellow>Осталось: </color>%count%<color=yellow> игроков</color>'
  line_winner: '<color=red>Победил:</color> %winner%'
  line_all_died: '<color=red>Все игроки погибли</color>'
  line_died: 'Вы погибли'
  # Down Cubes Game Mode
  cube_name: 'Обвал кубов'
  cube_description: 'Кубы обваливаются, необходимо выжить'
  cube_broadcast: '%name%

    %time%

    <color=yellow>Осталось: </color>%count%<color=yellow> игроков</color>'
  cube_winner: '<color=red>Победитель:</color> %winner%'
  cube_all_died: '<color=red>Все погибли</color>'
  cube_died: 'Вы погибли'
  # Hide And Seek Game Mode
  hide_name: 'Прятки'
  hide_description: 'Поймайте всех игроков'
  hide_broadcast: 'Бегите!

    Выбираем ищущих игроков

    %time%'
  hide_cycle: 'Передайте дубинку другому игроку

    <color=yellow><b><i>%time%</i></b> осталось</color>'
  hide_hurt: 'Вы не успели передать дубинку'
  hide_more_player: 'Осталось много игроков.

    Ищем нового игрока

    <color=yellow>Время <color=red>%time%</color></color>'
  hide_one_player: 'Победил %winner%

    <color=yellow>Время <color=red>%time%</color></color>'
  hide_all_die: 'Никто не выжил.

    Конец игры.

    <color=yellow>Время <color=red>%time%</color></color>'
  # Death Party Game Mode
  death_name: 'Смертельная вечеринка'
  death_description: 'Выживание при гранатном дожде'
  death_cycle: '<color=yellow>Избегай гранат!</color>

    <color=green>Прошло %time% секунд</color>

    <color=red>Осталось %count% игроков</color>'
  death_more_player: '<color=red>Смертельная вечеринка</color>

    <color=yellow>Выжило<color=red>%count%</color>игроков</color>

    <color=#ffc0cb>%time%</color>'
  death_one_player: '<color=red>Смертельная вечеринка</color>

    <color=yellow>Победитель - <color=red>%winner%</color></color>

    <color=#ffc0cb>%time%</color>'
  death_all_die: '<color=red>Смертельная вечеринка</color>

    <color=yellow>Все игроки погибли</color>

    <color=#ffc0cb>%time%</color>'
  # FinishWay Game Mode
  finish_way_name: '<color=yellow>Дорога к финишу</color>'
  finish_way_description: 'Добегите до конца для победы!'
  finish_way_cycle: '%name%

    <color=yellow>Дойдите до финиша</color>

    Time left: %time%'
  finish_way_died: 'Не прошел до финиша'
  finish_way_several_survivors: '<color=red>Люди выиграли!</color>

    Выжило %count%'
  finish_way_one_survived: '<color=red>Люди выиграли!</color>

    Победил: %player%'
  finish_way_no_survivors: '<color=red>Никто не выжил</color>'
  # Zombie Escape Game Mode
  zombie_escape_name: 'Зомби Побег'
  zombie_escape_description: 'Сбегите от зомби на вертолете'
  zombie_escape_before_start: '<color=#D71868><b><i>%name%</i></b></color>

    Начало через: %time%'
  zombie_escape_helicopter: '<color=yellow>%name%</color>

    <color=red>Вызовите вертолет!</color>

    Осталось людей: %count%'
  zombie_escape_died: 'Взрыв боеголовки'
  zombie_escape_zombie_win: '<color=red>Зомби выиграли!

    </color>Все люди погибли'
  zombie_escape_human_win: '<color=blue>Люди выиграли!</color>Люди сбежали'
  # Lava Game Mode
  lava_name: 'Пол это Лава'
  lava_description: 'Избегайте лавы и уничтожьте других игроков'
  lava_before_start: '<size=100><color=red>%time%</color></size>

    Поднимите оружие и поднимитесь наверх'
  lava_cycle: '<size=20><color=red><b>Выжило: %count% игроков</b></color></size>'
  lava_win: '<color=red><b>Победил:

    %winner%</b></color>'
  lava_all_dead: '<color=red><b>Никто не выжил</b></color>'
```
