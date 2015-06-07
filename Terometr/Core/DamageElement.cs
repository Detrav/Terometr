﻿using Detrav.TeraApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
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
        public void addValue(uint v, DateTime now)
        {
            if (now - last > timeOut)
            {
                first += now - last;
                //Logger.debug("First = {0}", first);
            }
            value += v;
            last = now;
        }

        public DamageElement(TeraPlayer player)
        {
            this.player = player;
        }
    }
}
