using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    class DPSEngine
    {
        public ulong value;
        public double perSecond { get { return value / elapsedTime.TotalSeconds; } }
        public TimeSpan elapsedTime
        {
            get
            {
                if (DateTime.Now - last > timeOut)
                    return last + timeOut - first;
                else return last - first;
            }
        }
        public DateTime first = DateTime.MinValue;//Динамически расчитывается начальный удар - timeOut
        public DateTime last = DateTime.MinValue;//Когда был последний удар
        //public DateTime end = DateTime.MinValue;//Динамически расчитывается конецчный удар + timeOut
        public TimeSpan timeOut = TimeSpan.FromMilliseconds(3.14);


        public void addValue(uint v)
        {
            DateTime now = DateTime.Now;
            if (now - last > timeOut)
                first += now - last - timeOut;
            value += v;
            last = now;
        }
    }
}
