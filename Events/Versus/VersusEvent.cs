using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Events
{
    /*
    internal class Versus : IEvent
    {
        public string Name => "Петушиные Бои";
        public string Description => "Дуель игроков на карте 35hp из cs 1.6";
        public string Color => "FFFF00";
        public string CommandName => "35hp";
        public Model GameMap { get; set; }
        public Model Doors { get; set; }
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }
        public TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Qurre.Events.Player.Join += OnJoin;
            Qurre.Events.Player.DroppingItem += OnDroppingItem;
            Qurre.Events.Player.PickupItem += OnPickupItem;
            OnEventStarted();
        }
        public void OnStop()
        {
            Qurre.Events.Player.Join -= OnJoin;
            Qurre.Events.Player.DroppingItem -= OnDroppingItem;
            Qurre.Events.Player.PickupItem -= OnPickupItem;
            Timing.CallDelayed(10f, () => EventEnd());
            Plugin.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameMap = CreatingMapFromJson("35Hp.json", new Vector3(145.18f, 930f, -122.97f));
            CreateDoors();

            PlayAudio("FallGuys_FutureUtobeania.f32le", 10, true, "Петухи");
            var count = 0;
            // Map.Broadcast("<color=red>Недостаточно игроков. Мини-игра прекращается!</color>", 5);
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role = RoleType.Scientist;
                    Timing.CallDelayed(2f, () =>
                    {
                        player.Position = GameMap.GameObject.transform.position + new Vector3(13.72f, -3.08f, 1.526f);
                        player.ClearInventory();
                    });
                }
                else
                {
                    player.Role = RoleType.ClassD;
                    Timing.CallDelayed(2f, () =>
                    {
                        player.Position = GameMap.GameObject.transform.position + new Vector3(-28.21f, -3.08f, 1.526f);
                        player.ClearInventory();
                    });
                }
                count++;
            }
            Timing.RunCoroutine(Cycle(), "35hp_time");

        }
        public IEnumerator<float> Cycle()
        {
            for (int time = 10; time > 0; time--)
            {
                BroadcastPlayers($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            // Do players
            var gun = Guns.RandomItem();
            DoScientist(gun);
            DoClassD(gun);
            // cycle
            while (Player.List.Count(r => r.Team == Team.RSC) > 0 && Player.List.Count(r => r.Team == Team.CDP) > 0)
            {
                if (Scientist.Role == RoleType.Spectator && ClassD.Role != RoleType.Spectator)
                {
                    BroadcastPlayers($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                    $"<color=yellow>Победитель боя: <color=red>{ClassD.Nickname}</color></color>", 10);
                    Timing.RunCoroutine(CleanUpAll());

                    yield return Timing.WaitForSeconds(10f);

                    ClassD.Hp = 100;
                    gun = Guns.RandomItem();
                    DoScientist(gun);
                    ClassD.ResetInventory(new List<ItemType> { gun });
                }
                if (ClassD.Role == RoleType.Spectator && Scientist.Role != RoleType.Spectator)
                {
                    BroadcastPlayers($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                    $"<color=yellow>Победитель боя: <color=red>{Scientist.Nickname}</color></color>", 10);
                    Timing.RunCoroutine(CleanUpAll());

                    yield return Timing.WaitForSeconds(10f);

                    Scientist.Hp = 100;
                    gun = Guns.RandomItem();
                    DoClassD(gun);
                    Scientist.ResetInventory(new List<ItemType> { gun });
                }
                BroadcastPlayers($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                $"<color=yellow><color=yellow>{Scientist.Nickname}</color> <color=red>VS</color> <color=orange>{ClassD.Nickname}</color></color>", 1);

                yield return Timing.WaitForSeconds(1f);
            }
            if (Player.List.Count(r => r.Team == Team.RSC) == 0)
            {
                BroadcastPlayers($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИТЕЛИ: <color=red>Д КЛАСС</color></color>", 10);
            }
            else if (Player.List.Count(r => r.Team == Team.CDP) == 0)
            {
                BroadcastPlayers($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИТЕЛИ: <color=red>УЧЕНЫЕ</color></color>", 10);
            }
            OnStop();
            yield break;
        }
        public void DoScientist(ItemType gun)
        {
            Scientist = Player.List.Where(r => r.Team == Team.RSC).ToList().RandomItem();
            Scientist.Position = GameMap.GameObject.transform.position + new Vector3(-1.45f, -3.08f, 1.526f);
            Scientist.ResetInventory(new List<ItemType> { gun });
        }
        public void DoClassD(ItemType gun)
        {
            ClassD = Player.List.Where(r => r.Team == Team.CDP).ToList().RandomItem();
            ClassD.Position = GameMap.GameObject.transform.position + new Vector3(-13.83f, -3.08f, 1.526f);
            ClassD.ResetInventory(new List<ItemType> { gun });
        }
        public void EventEnd()
        {
            if (Audio.Microphone.IsRecording) StopAudio();
            GameObject.Destroy(GameMap.GameObject);
            Doors.Destroy();
            Timing.RunCoroutine(CleanUpAll());
        }
        public void CreateDoors()
        {
            Doors = new Model("PrisonerDoors", GameMap.GameObject.transform.position);
            Doors.AddPart(new ModelPrimitive(Doors, PrimitiveType.Cube, new Color32(85, 87, 85, 51), new Vector3(-15.04f, -2.62f, 1.52f), new Vector3(90, 90, 0), new Vector3(3.1f, 1f, 6.52f)));
            Doors.AddPart(new ModelPrimitive(Doors, PrimitiveType.Cube, new Color32(85, 87, 85, 51), new Vector3(0.83f, -2.62f, 1.52f), new Vector3(90, 90, 0), new Vector3(3.1f, 1f, 6.52f)));
        }
        public List<ItemType> Guns = new List<ItemType>()
        {
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunFSP9,
            ItemType.GunCrossvec,
            ItemType.GunAK,
            ItemType.GunE11SR,
            ItemType.GunRevolver,
            ItemType.GunLogicer,
            ItemType.ParticleDisruptor,
            ItemType.GunShotgun
        };
        public void OnJoin(JoinEvent ev) => ev.Player.Role = RoleType.Spectator;
        public void OnDroppingItem(DroppingItemEvent ev) => ev.Allowed = false;
        public void OnPickupItem(PickupItemEvent ev) => ev.Allowed = false;
    }
    */
}
