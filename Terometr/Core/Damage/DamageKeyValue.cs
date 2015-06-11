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
        public double value;
        public string name;
        public PlayerClass playerClass;

        public DamageKeyValue(ulong id, string name, double value, double critRate, PlayerClass playerClass = PlayerClass.Empty)
        {
            this.id = id;
            this.value = value;
            this.name = name;
            this.playerClass = playerClass;
            this.critRate = critRate;
        }
    }
}
