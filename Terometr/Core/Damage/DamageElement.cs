using Detrav.TeraApi.Core;
using Detrav.TeraApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core.Damage
{
    class DamageElement
    {
        public string name;
        public PlayerClass playerClass;
        public ulong value;
        public double vps { get { return value / elapsedTime.TotalSeconds; } }

        public DamagePlayerType type;

        public TimeSpan elapsedTime
        {
            get
            {
                if (DateTime.Now - last > timeOut)
                    return last + timeOut - first;
                else if (DateTime.Now - first < min)
                    return min;
                return DateTime.Now - first;
            }
        }
        public DateTime first = DateTime.MinValue;//Динамически расчитывается начальный удар - timeOut
        public DateTime last = DateTime.MinValue;//Когда был последний удар
        //public DateTime end = DateTime.MinValue;//Динамически расчитывается конецчный удар + timeOut
        static public TimeSpan timeOut = TimeSpan.FromSeconds(10.1);
        static public TimeSpan min = TimeSpan.FromSeconds(1);

        public double critRate
        {
            get
            {
                if (critCount == 0) return 0;
                return ((double)critCount) / ((double)(count));
            }
        }
        public int count = 0;
        public int critCount = 0;
        public void add(uint v, DateTime now,bool self,bool crit)
        {
            if (now - last > timeOut)
            {
                first += now - last;
                //Logger.debug("First = {0}", first);
            }
            value += v;
            last = now;
            if(self)
            {
                count++;
                if (crit)
                    critCount++;
            }
        }

        public DamageElement(TeraEntity entity,bool group)
        {
            if (group) type = DamagePlayerType.group;
            else type = DamagePlayerType.npc;
            

            if (entity is TeraPlayer)
            {
                type = DamagePlayerType.player;
                playerClass = (entity as TeraPlayer).playerClass;
            }
            else
            {
                playerClass = PlayerClass.Empty;
            }
            if (entity is TeraPartyPlayer) type = DamagePlayerType.party;
            name = entity.safeName;
        }
    }
}
