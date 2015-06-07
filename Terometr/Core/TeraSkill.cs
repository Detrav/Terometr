using Detrav.TeraApi.Core;
using Detrav.TeraApi.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    public class TeraSkill
    {
        public TeraPlayer player;
        public SkillType skillType;
        public NpcDataBase npc;
        public ushort type;//1 - атака 2-хил, вроде так
        public uint value;
        public bool crit;
        public DateTime time;

        public TeraSkill(TeraPlayer player, SkillType skillType, ushort type, uint value, bool crit = false, NpcDataBase npc = null)
        {
            this.player = player;
            this.skillType = skillType;
            this.type = type;
            this.value = value;
            this.crit = crit;
            this.npc = npc;
            time = DateTime.Now;
        }
    }

    public enum SkillType
    {
        Take, Make
    }
}
