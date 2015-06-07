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


        internal void add(TeraSkill skill)
        {
            if (!players.ContainsKey(skill.player.id))
                players[skill.player.id] = new AgroElement(skill.player);
            players[skill.player.id].add(skill.value,skill.time);
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
    }
}
