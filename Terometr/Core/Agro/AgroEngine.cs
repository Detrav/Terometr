using Detrav.TeraApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core.Agro
{
    class AgroEngine
    {
        public ulong npc;
        public ulong lastTarget;
        public bool multi;
        public Dictionary<ulong, AgroElement> players = new Dictionary<ulong,AgroElement>();
        public uint npcHp;
        public DateTime lastActive = DateTime.MinValue;
        public static TimeSpan timeOut = TimeSpan.FromSeconds(15.1);
        public bool isActive { get { return DateTime.Now - lastActive < timeOut; } }


        internal void add(TeraSkill skill)
        {
            if (!players.ContainsKey(skill.player.id))
                players[skill.player.id] = new AgroElement(skill.player);
            players[skill.player.id].add(skill.value,skill.time);
            lastActive = DateTime.Now;
        }

        internal void Clear()
        {
            players.Clear();
            npc = 0;
            lastTarget = 0;
            multi = false;
        }

        public double getValue(ulong id)
        {
            if (players.ContainsKey(id))
                return players[id].value(DateTime.Now);
            return 0;
        }

        public void getSortedList(Dictionary<ulong,double> p,SortedList<double,TeraPlayer> list,out double sum, out double max)
        {
            sum = 0;
            max = 0;
            if (list == null) return;
            DateTime now = DateTime.Now;
            foreach (var pair in players)
            {
                double pl; double val = val = pair.Value.value(now);
                if (p.TryGetValue(pair.Value.teraPlayer.id, out pl))
                    val *= pl;
                max = Math.Max(val, max);
                sum += val;
                list.Add(val, pair.Value.teraPlayer);
            }
        }

        public void getSortedList(SortedList<double, TeraPlayer> list, out double sum, out double max)
        {
            sum = 0;
            max = 0;
            if (list == null) return;
            DateTime now = DateTime.Now;
            foreach (var pair in players)
            {
                double pl; double val = val = pair.Value.value(now);
                max = Math.Max(val, max);
                sum += val;
                list.Add(val, pair.Value.teraPlayer);
            }
        }

        internal void addHeal(TeraSkill skill)
        {
            if (!isActive) return;
            if (!players.ContainsKey(skill.player.id))
                players[skill.player.id] = new AgroElement(skill.player);
            players[skill.player.id].add(skill.value, skill.time);
        }

        internal AgroKeyValue[] getList(out double sum, out double max)
        {
            sum = 0;
            max = 0;
            List<AgroKeyValue> list = new List<AgroKeyValue>();
            AgroKeyValue[] result;
            DateTime now = DateTime.Now;
            foreach (var pair in players)
            {
                double val = pair.Value.value(now);
                max = Math.Max(val, max);
                sum += val;
                int num = list.Count;
                for (; num > 0;num-- )
                {
                    if (list[num - 1].value > val) break;
                }
                list.Insert(num, new AgroKeyValue(pair.Value.teraPlayer.id, val, pair.Value.teraPlayer.name, pair.Value.teraPlayer.playerClass));
            }
            result = list.ToArray();
            for (int i = result.Length-1; i > 0; i--)
            {
                if (result[i].id == lastTarget)
                {
                    var temp = result[i - 1];
                    result[i - 1] = result[i];
                    result[i] = temp;
                }
            }
            return result;
        }
    }
}
