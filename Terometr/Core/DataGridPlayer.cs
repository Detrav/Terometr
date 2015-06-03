using Detrav.TeraApi.Enums;
using Detrav.Terometr.UserElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Detrav.Terometr.Core
{
    class DataGridPlayer
    {
        public BitmapImage image { get; set; }
        public string name { get; set; }
        public ulong dps { get; set; }
        public ulong damage { get; set; }
        public ulong hps { get; set; }
        public ulong heal { get; set; }
        public ulong damageTaken { get; set; }
        public ulong healTaken { get; set; }

        public DataGridPlayer(PlayerClass playerClass,string name)
        {
            switch (playerClass)
            {
                case PlayerClass.Archer: image = PlayerBarElement.archer; break;
                case PlayerClass.Berserker: image = PlayerBarElement.berserker; break;
                case PlayerClass.Lancer: image = PlayerBarElement.lancer; break;
                case PlayerClass.Mystic: image = PlayerBarElement.mystic; break;
                case PlayerClass.Priest: image = PlayerBarElement.priest; break;
                case PlayerClass.Reaper: image = PlayerBarElement.reaper; break;
                case PlayerClass.Slayer: image = PlayerBarElement.slayer; break;
                case PlayerClass.Sorcerer: image = PlayerBarElement.sorcerer; break;
                case PlayerClass.Warrior: image = PlayerBarElement.warrior; break;
                default: image = null; break;
            }
            this.name = name;
        }

        public void update(double dps, ulong damage, double hps, ulong heal, ulong damageTaken, ulong healTaken)
        {
            this.dps = Convert.ToUInt64(dps);
            this.damage = damage;
            this.hps = Convert.ToUInt64(hps);
            this.heal = heal;
            this.damageTaken = damageTaken;
            this.healTaken = healTaken;
        }
    }
}