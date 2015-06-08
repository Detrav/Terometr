﻿using Detrav.TeraApi.Enums;
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
        public double value;
        public string name;
        public PlayerClass playerClass;

        public DamageKeyValue(ulong id, double value, string name, PlayerClass playerClass)
        {
            this.id = id;
            this.value = value;
            this.name = name;
            this.playerClass = playerClass;
        }
    }
}
