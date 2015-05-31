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
        public bool inParty;
        public DPSEngine dps;
        public DPSEngine dtps;
        public DPSEngine hps;
        public DPSEngine htps;

        public TeraPlayer(ulong id,string name, bool inParty)
        {
            this.id = id;
            this.name = name;
            this.inParty = inParty;
        }
        public bool updateStatus(bool inParty)
        {
            this.inParty = inParty;
            return this.inParty;
        }
        public void clear()
        {
            dps = null;
            dtps = null;
            hps = null;
            htps = null;
        }
        public void makeSkill(uint v, ushort t)
        {
            if(v == 0) return;
            switch(t)
            {
                case 1:
                    if (dps == null) dps = new DPSEngine();
                    dps.addValue(v);
                    break;
                case 2:
                    if (hps == null) hps = new DPSEngine();
                    hps.addValue(v);
                    break;
            }
        }
        public void takeSkill(uint v,ushort t)
        {
            if (v == 0) return;
            switch (t)
            {
                case 1:
                    if (dtps == null) dtps = new DPSEngine();
                    dtps.addValue(v);
                    break;
                case 2:
                    if (htps == null) htps = new DPSEngine();
                    htps.addValue(v);
                    break;
            }
        }
    }
}
