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

        public DamageEngine(uint npcHp)
        {
            this.npcHp = npcHp;
        }

        /*internal void add(TeraSkill skill)
        {
            if (!players.ContainsKey(skill.player.id))
                players[skill.player.id] = new DamageElement(skill.player);
            players[skill.player.id].add(skill.value, skill.time);
            lastActive = DateTime.Now;
        }*/

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
