using Detrav.TeraApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core.Damage
{
    class DamageEngine
    {
        Dictionary<ulong, DamageElement> players = new Dictionary<ulong, DamageElement>();
        public DateTime lastActive = DateTime.MinValue;
        public uint npcHp;
        public bool isActive { get { return DateTime.Now - lastActive < MetrEngine.timeOutMetr; } }
        public string name;

        public DamageEngine(uint npcHp,string name)
        {
            this.npcHp = npcHp;
            this.name = name;
        }

        /// <summary>
        /// Добавляет уорн в дамаг метр
        /// </summary>
        /// <param name="player">игрок</param>
        /// <param name="value">значение</param>
        /// <param name="time">время</param>
        /// <param name="self">бьёт ли сам или с помощью npc</param>
        /// <param name="crit">является ли критом</param>
        internal void add(TeraPlayer player,uint value,DateTime time,bool self,bool crit)
        {
            if (!players.ContainsKey(player.id))
                players[player.id] = new DamageElement(player);
            players[player.id].add(value, time,self,crit);
            lastActive = DateTime.Now;
        }

        internal void Clear()
        {
            lastActive = DateTime.MinValue;
            players.Clear();
        }

        internal void getListDps(SortedList<double, DamageKeyValue> list, out double max, out double sum)
        {
            throw new NotImplementedException();
        }

        internal void getList(SortedList<double, DamageKeyValue> list, out double max, out double sum)
        {
            throw new NotImplementedException();
        }
    }
   
}
