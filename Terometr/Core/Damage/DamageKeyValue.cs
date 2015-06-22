using Detrav.TeraApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core.Damage
{
    class DamageKeyValue
    {
        public ulong id;
        public double critRate;
        public double inSec;
        public double value;
        public string name;
        public PlayerClass playerClass;
        public DamagePlayerType type;

        public DamageKeyValue(ulong id, string name, double value,double inSec, double critRate, PlayerClass playerClass = PlayerClass.Empty, DamagePlayerType type = DamagePlayerType.npc)
        {
            this.id = id;
            this.value = value;
            this.inSec = inSec;
            this.name = name;
            this.playerClass = playerClass;
            this.critRate = critRate;
            this.type = type;
        }
    }
}
