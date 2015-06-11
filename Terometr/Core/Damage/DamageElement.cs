using Detrav.TeraApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core.Damage
{
    class DamageElement
    {
        public TeraPlayer player;
        public ulong value;
        public double vps { get { return value / elapsedTime.TotalSeconds; } }
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
        static public TimeSpan timeOut = TimeSpan.FromSeconds(5.01);
        static public TimeSpan min = TimeSpan.FromSeconds(1);

        public double critRate { get { return ((double)critCount) / ((double)(count)); } }
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

        public DamageElement(TeraPlayer player)
        {
            this.player = player;
        }
    }
}
