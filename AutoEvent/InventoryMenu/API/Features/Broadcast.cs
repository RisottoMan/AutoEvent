/*
 * Taken from Riptide Events. Thanks Riptide!
 */
using System.Diagnostics;
using MEC;
using PluginAPI.Core;

namespace InventoryMenu.API.Features
{
    public enum BroadcastPriority { Lowest, VeryLow, Low, Medium, High, VeryHigh, Highest };

    public sealed class BroadcastOverride
    {
        static BroadcastOverride()
        {
        }

        class BroadcastInfo
        {
            class Line
            {
                public int size = 44;
                public BroadcastPriority priority = BroadcastPriority.Lowest;
                public float duration = -1.0f;
                public string msg = "";
            }

            List<Line> lines = new List<Line>();
            Stopwatch stop_watch = new Stopwatch();
            bool dirty = false;
            CoroutineHandle handle = new CoroutineHandle();

            public BroadcastInfo()
            {
                stop_watch.Start();
            }

            public void SetEvenLineSizes(int line_count)
            {
                UpdateDuration();

                List<int> sizes = new List<int> { 178, 89, 59, 44, 35, 29, 25, 22, 19, 17, 16, 14, 12, 11 };
                int dif = line_count - lines.Count;
                for (int i = 0; i < dif; i++)
                    lines.Add(new Line());

                if (lines.Count > line_count)
                    lines.RemoveRange(line_count, lines.Count - line_count);

                foreach (Line line in lines)
                    line.size = sizes[line_count - 1];

                dirty = true;
            }

            public void SetCustomLineSizes(IEnumerable<int> sizes)
            {
                UpdateDuration();

                int dif = sizes.Count() - lines.Count;
                for (int i = 0; i < dif; i++)
                    lines.Add(new Line());

                if (lines.Count > sizes.Count())
                    lines.RemoveRange(sizes.Count(), lines.Count - sizes.Count());

                for (int i = 0; i < sizes.Count(); i++)
                    lines[i].size = sizes.ElementAt(i);

                dirty = true;
            }

            public void BroadcastLine(int line, float duration, BroadcastPriority priority, string msg)
            {
                if (line >= 1 && line <= lines.Count)
                {
                    if (lines[line - 1].duration <= 0.0f || lines[line - 1].priority <= priority)
                    {
                        UpdateDuration();
                        lines[line - 1].priority = priority;
                        lines[line - 1].duration = duration;
                        lines[line - 1].msg = msg;
                        dirty = true;
                    }
                }
            }

            public void ClearLines(BroadcastPriority priority)
            {
                UpdateDuration();

                foreach (Line line in lines)
                    if (line.priority <= priority)
                        line.duration = -1.0f;

                dirty = true;
            }

            public void ClearLine(int line, BroadcastPriority priority)
            {
                if (line >= 1 && line <= lines.Count)
                {
                    if (lines[line - 1].priority <= priority)
                    {
                        UpdateDuration();
                        lines[line - 1].duration = -1.0f;
                        dirty = true;
                    }
                }
            }

            public void UpdateDuration()
            {
                float delta = (float)stop_watch.Elapsed.TotalSeconds;
                stop_watch.Restart();

                foreach (Line line in lines)
                    line.duration -= delta;
            }

            public float Update(Player player, float delta)
            {
                UpdateDuration();

                string msg = "";
                foreach (Line line in lines)
                {
                    if (line.duration > 0.0f)
                        msg += "<size=" + line.size.ToString() + ">" + line.msg + "</size>\n";
                    //else
                    //    msg += "<size=" + line.size.ToString() + "> </size>\n";
                }

                player.SendBroadcast(msg, 300, Broadcast.BroadcastFlags.Truncated, true);

                float min = 300.0f;
                bool any_active = false;
                foreach (Line line in lines)
                {
                    if (line.duration > 0.0f)
                    {
                        min = Math.Min(min, line.duration);
                        any_active = true;
                    }
                }
                if (any_active)
                    return min;
                else
                    return -1.0f;
            }

            public void UpdateIfDirty(Player player)
            {
                if (dirty)
                {
                    dirty = false;
                    if (handle.IsValid)
                        Timing.KillCoroutines(handle);
                    stop_watch.Restart();
                    handle = Timing.RunCoroutine(_Update(player));
                }
            }


            public IEnumerator<float> _Update(Player player)
            {
                float delta = 1.0f;
                while (delta > 0.0f)
                {
                    delta = Update(player, (float)stop_watch.Elapsed.TotalSeconds);
                    yield return Timing.WaitForSeconds(delta);
                }
                yield break;
            }
        }

        static Dictionary<int, BroadcastInfo> broadcast_info = new Dictionary<int, BroadcastInfo>();

        public static void SetEvenLineSizes(Player player, int line_count)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info[player.PlayerId].SetEvenLineSizes(line_count);
        }

        public static void SetCustomLineSizes(Player player, IEnumerable<int> sizes)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info[player.PlayerId].SetCustomLineSizes(sizes);
        }

        public static void RegisterPlayer(Player player)
        {
            if (!broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info.Add(player.PlayerId, new BroadcastInfo());
        }

        public static void UnregisterPlayer(Player player)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info.Remove(player.PlayerId);
        }

        public static void Reset()
        {
            broadcast_info.Clear();
        }

        public static void BroadcastLine(Player player, int line, float duration, BroadcastPriority priority, string msg)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info[player.PlayerId].BroadcastLine(line, duration, priority, msg);
        }

        public static void BroadcastLine(int line, float duration, BroadcastPriority priority, string msg)
        {
            foreach (Player player in Player.GetPlayers())
                if (player.Role != PlayerRoles.RoleTypeId.None)
                    BroadcastLine(player, line, duration, priority, msg);
        }

        public static void BroadcastLines(Player player, int line, float duration, BroadcastPriority priority, List<string> msgs)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
            {
                foreach (string msg in msgs)
                {
                    broadcast_info[player.PlayerId].BroadcastLine(line, duration, priority, msg);
                    line++;
                }
            }
        }

        public static void BroadcastLines(int line, float duration, BroadcastPriority priority, List<string> msgs)
        {
            foreach (Player player in Player.GetPlayers())
                if (player.Role != PlayerRoles.RoleTypeId.None)
                    BroadcastLines(player, line, duration, priority, msgs);
        }

        public static void ClearLines(Player player, BroadcastPriority priority)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info[player.PlayerId].ClearLines(priority);
        }

        public static void ClearLines(BroadcastPriority priority)
        {
            foreach (Player player in Player.GetPlayers())
                if (player.Role != PlayerRoles.RoleTypeId.None)
                    ClearLines(player, priority);
        }

        public static void ClearLine(Player player, int line, BroadcastPriority priority)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info[player.PlayerId].ClearLine(line, priority);
        }

        public static void ClearLine(int line, BroadcastPriority priority)
        {
            foreach (Player player in Player.GetPlayers())
                if (player.Role != PlayerRoles.RoleTypeId.None)
                    ClearLine(player, line, priority);
        }

        public static void UpdateIfDirty(Player player)
        {
            if (broadcast_info.ContainsKey(player.PlayerId))
                broadcast_info[player.PlayerId].UpdateIfDirty(player);
        }

        public static void UpdateAllDirty()
        {
            foreach (Player player in Player.GetPlayers())
                if (player.Role != PlayerRoles.RoleTypeId.None)
                    UpdateIfDirty(player);
        }

    }
}