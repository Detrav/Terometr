using Detrav.Terometr.UserElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    partial class Repository
    {
        public Dictionary<ulong, DpsPlayer> playersSnapShot = new Dictionary<ulong, DpsPlayer>();

        public SortedList<double, Vector3Str> dpsList = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
        public double dpsMax;
        public double dpsSum;
        public SortedList<double, Vector3Str> damageList = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
        public double damageMax;
        public double damageSum;
        public SortedList<double, Vector3Str> hpsList = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
        public double hpsMax; 
        public double hpsSum;
        public SortedList<double, Vector3Str> healList = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
        public double healMax; 
        public double healSum;
        public SortedList<double, Vector3Str> damageTakenList = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
        public double damageTakenMax; 
        public double damageTakenSum;
        public SortedList<double, Vector3Str> healTakenList = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
        public double healTakenMax; 
        public double healTakenSum;

        internal void doEvents()
        {
            
            dpsList.Clear(); dpsMax = 0; dpsSum = 0;
            damageList.Clear(); damageMax = 0; damageSum = 0;
            hpsList.Clear(); hpsMax = 0; hpsSum = 0;
            healList.Clear(); healMax = 0; healSum = 0;
            damageTakenList.Clear(); damageTakenMax = 0; damageTakenSum = 0;
            healTakenList.Clear(); healTakenMax = 0; healTakenSum = 0;

            foreach (var pair in party)
            {
                if (pair.Value.damage > 0)
                {
                    playersSnapShot[pair.Key] = pair.Value;
                    continue;
                }
                if (pair.Value.heal > 0)
                {
                    playersSnapShot[pair.Key] = pair.Value;
                    continue;
                }
                if (pair.Value.damageTaken > 0)
                {
                    playersSnapShot[pair.Key] = pair.Value;
                    continue;
                }
                if (pair.Value.healTaken > 0)
                {
                    playersSnapShot[pair.Key] = pair.Value;
                    continue;
                }
            }

            foreach (var pair in playersSnapShot)
            {
                double dps = pair.Value.dps;
                if (dps > 0)
                {
                    dpsSum += dps;
                    dpsMax = Math.Max(dps, dpsMax);
                    dpsList.Add(dps, new Vector3Str(pair.Value.player.name, dps, (pair.Value.player.id == self.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass));
                }

                double damage = pair.Value.damage;
                if (damage > 0)
                {
                    damageSum += damage;
                    damageMax = Math.Max(damage, damageMax);
                    damageList.Add(damage, new Vector3Str(pair.Value.player.name, damage, (pair.Value.player.id == self.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass));
                }

                double hps = pair.Value.hps;
                if (hps > 0)
                {
                    hpsSum += hps;
                    hpsMax = Math.Max(hps, hpsMax);
                    hpsList.Add(hps, new Vector3Str(pair.Value.player.name, hps, (pair.Value.player.id == self.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass));
                }

                double heal = pair.Value.heal;
                if (heal > 0)
                {
                    healSum += heal;
                    healMax = Math.Max(heal, healMax);
                    healList.Add(heal, new Vector3Str(pair.Value.player.name, heal, (pair.Value.player.id == self.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass));
                }

                double damageTaken = pair.Value.damageTaken;
                if (damageTaken > 0)
                {
                    damageTakenSum += damageTaken;
                    damageTakenMax = Math.Max(damageTaken, damageTakenMax);
                    damageTakenList.Add(damageTaken, new Vector3Str(pair.Value.player.name, damageTaken, (pair.Value.player.id == self.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass));
                }

                double healTaken = pair.Value.healTaken;
                if (healTaken > 0)
                {
                    healTakenSum += healTaken;
                    healTakenMax = Math.Max(healTaken, healTakenMax);
                    healTakenList.Add(healTaken, new Vector3Str(pair.Value.player.name, healTaken, (pair.Value.player.id == self.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass));
                }
            }

            dpsList.Add(0, new Vector3Str("Всего", dpsSum, PlayerBarElement.clr.sum, TeraApi.Enums.PlayerClass.Empty));
            damageList.Add(0, new Vector3Str("Всего", damageSum, PlayerBarElement.clr.sum, TeraApi.Enums.PlayerClass.Empty));
            hpsList.Add(0, new Vector3Str("Всего", hpsSum, PlayerBarElement.clr.sum, TeraApi.Enums.PlayerClass.Empty));
            healList.Add(0, new Vector3Str("Всего", healSum, PlayerBarElement.clr.sum, TeraApi.Enums.PlayerClass.Empty));
            damageTakenList.Add(0, new Vector3Str("Всего", damageTakenSum, PlayerBarElement.clr.sum, TeraApi.Enums.PlayerClass.Empty));
            healTakenList.Add(0, new Vector3Str("Всего", healTakenSum, PlayerBarElement.clr.sum, TeraApi.Enums.PlayerClass.Empty));
        }

        public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = y.CompareTo(x);

                if (result == 0)
                    return 1;   // Handle equality as beeing greater
                else
                    return result;
            }

            #endregion
        }

        public Dictionary<ulong, DataGridPlayer> playersDataGrid = new Dictionary<ulong, DataGridPlayer>();
        public DataGridPlayer summary = new DataGridPlayer(TeraApi.Enums.PlayerClass.Empty, "Всего");
        public DataGridPlayer[] getAllArrayDataGridPlayer()
        {
            foreach(var pair in playersSnapShot)
            {
                DpsPlayer p = pair.Value;
                DataGridPlayer player;
                if (!playersDataGrid.TryGetValue(pair.Key, out player))
                {
                    player = new DataGridPlayer(p.player.playerClass, p.player.name);
                    playersDataGrid[pair.Key] = player;
                }
                player.update(p.dps, p.damage, p.hps, p.heal, p.damageTaken, p.healTaken);
            }
            playersDataGrid[ulong.MaxValue] = summary;
            summary.update(dpsSum, (ulong)damageSum, hpsSum, (ulong)healSum, (ulong)damageTakenSum, (ulong)healTakenSum);
            return playersDataGrid.Values.ToArray();
        }
    }
}