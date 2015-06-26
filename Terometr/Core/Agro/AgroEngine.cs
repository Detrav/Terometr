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
        public string name;

        public AgroEngine(ulong mId,string name, uint maxHp, bool multi)
        {
            // TODO: Complete member initialization
            this.npc = mId;
            this.npcHp = maxHp;
            this.name = name;
            this.multi = multi;
        }
        public bool isActive { get { return DateTime.Now - lastActive < timeOutMetr; } }
        public bool isFullActive { get { return DateTime.Now - lastActive < AgroElement.timeOut; } }
        public static TimeSpan timeOutMetr = TimeSpan.FromSeconds(10.1);

        internal void add(TeraPlayer player,uint value,DateTime time)
        {
            if (!players.ContainsKey(player.id))
                players[player.id] = new AgroElement(player);
            players[player.id].add(value,time);
            lastActive = time;
        }

        internal void Clear()
        {
            lastActive = DateTime.MinValue;
            players.Clear();
            //npc = 0;
            lastTarget = 0;
            multi = false;
            //npcHp = 0;
        }

        public double getValue(ulong id)
        {
            if (players.ContainsKey(id))
                return players[id].value(DateTime.Now);
            return 0;
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

        internal void addHeal(TeraPlayer player,uint p, DateTime dateTime)
        {
            if (!isActive) return;
            if (!players.ContainsKey(player.id))
                players[player.id] = new AgroElement(player);
            players[player.id].add(p,dateTime);
        }
    }
}
