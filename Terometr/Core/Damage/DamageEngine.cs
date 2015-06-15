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
        Dictionary<ulong, DamageElement> elements = new Dictionary<ulong, DamageElement>();
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
        internal void add(TeraEntity entity,uint value,DateTime time,bool self,bool crit)
        {
            if (!elements.ContainsKey(entity.id))
                elements[entity.id] = new DamageElement(entity);
            elements[entity.id].add(value, time,self,crit);
            lastActive = DateTime.Now;
        }

        internal void Clear()
        {
            lastActive = DateTime.MinValue;
            elements.Clear();
        }

        public DamageKeyValue[] getListDps(out double max, out double sum)
        {
            max = 0;
            sum = 0;
            List<DamageKeyValue> result = new List<DamageKeyValue>();
            foreach(var pair in elements)
            {
                var dkv = new DamageKeyValue(pair.Key,
                    pair.Value.name,
                    pair.Value.vps,
                    pair.Value.critRate,
                    pair.Value.playerClass);
                result.Add(dkv);
                max = Math.Max(max, dkv.value);
                sum += dkv.value;
            }
            return result.ToArray();
        }

        public DamageKeyValue[] getList(out double max, out double sum)
        {
            max = 0;
            sum = 0;
            List<DamageKeyValue> result = new List<DamageKeyValue>();
            foreach (var pair in elements)
            {
                var dkv = new DamageKeyValue(pair.Key,
                    pair.Value.name,
                    pair.Value.value,
                    pair.Value.critRate,
                    pair.Value.playerClass);
                result.Add(dkv);
                max = Math.Max(max, dkv.value);
                sum += dkv.value;
            }
            return result.ToArray();
        }
    }
   
}
