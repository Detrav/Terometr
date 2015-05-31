﻿using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    class TeraPlayer
    {
        public ulong id;
        public string name = "Unknown";
        public ulong damage;
        public ulong heal;
        public ulong damageTaken;
        public ulong healTaken;
        public double dps { get { return damage / elapsedTime.TotalSeconds; } }
        public double hps { get { return heal / elapsedTime.TotalSeconds; } }
        public TimeSpan elapsedTime
        {
            get
            {
                if (DateTime.Now - last > timeOut)
                    return last + timeOut - first;
                else return DateTime.Now - first;
            }
        }
        public DateTime first = DateTime.MinValue;//Динамически расчитывается начальный удар - timeOut
        public DateTime last = DateTime.MinValue;//Когда был последний удар
        //public DateTime end = DateTime.MinValue;//Динамически расчитывается конецчный удар + timeOut
        public TimeSpan timeOut = TimeSpan.FromSeconds(3.14);


        public void addDamage(uint v)
        {
            DateTime now = DateTime.Now;
            if (now - last > timeOut)
            {
                first += now - last - timeOut;
                Logger.debug("First = {0}", first);
            }
            damage += v;
            last = now;
        }
        public void addHeal(uint v)
        {
            DateTime now = DateTime.Now;
            if (now - last > timeOut)
            {
                first += now - last - timeOut;
                Logger.debug("First = {0}", first);
            }
            heal += v;
            last = now;
        }

        public TeraPlayer(ulong id,string name)
        {
            this.id = id;
            this.name = name;
        }
        
        public void makeSkill(uint v, ushort t)
        {
            if(v == 0) return;
            Logger.debug("Make skill type {0}", t);
            switch(t)
            {
                case 1:
                    addDamage(v);
                    break;
                case 2:
                    addHeal(v);
                    break;
            }
        }
        public void takeSkill(uint v,ushort t)
        {
            if (v == 0) return;
            Logger.debug("Take skill type {0}", t);
            switch (t)
            {
                case 1:
                    damageTaken += v;
                    break;
                case 2:
                    healTaken += v;
                    break;
            }
        }

        internal void clear()
        {
            first = DateTime.MinValue;
            last = DateTime.MinValue;
            damage = 0;
            heal = 0;
            damageTaken = 0;
            healTaken = 0;
        }
    }
}
