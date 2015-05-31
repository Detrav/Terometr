using Detrav.Terometr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Detrav.Terometr.UserElements
{
    /// <summary>
    /// Логика взаимодействия для ListBoxBars.xaml
    /// </summary>
    public partial class ListBoxBars : UserControl
    {
        public ListBoxBars()
        {
            InitializeComponent();
        }
        Dictionary<ulong, TeraPlayer> players = new Dictionary<ulong,TeraPlayer>();

        internal void updateDps(ulong selfId)
        {
            SortedList<double, Vector3Str> list = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                double dps = pair.Value.dps;
                sum += dps;
                max = Math.Max(dps, max);
                list.Add(dps, new Vector3Str(pair.Value.name, dps, (pair.Value.id == selfId? PlayerBarElement.clr.me : PlayerBarElement.clr.other)));
            }
            list.Add(0, new Vector3Str("Всего", sum, PlayerBarElement.clr.sum));
            while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(list, max, sum);
        }
        internal void updateDamage(ulong selfId)
        {
            SortedList<double, Vector3Str> list = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                double damage = pair.Value.damage;
                sum += damage;
                max = Math.Max(damage, max);
                list.Add(damage, new Vector3Str(pair.Value.name, damage, (pair.Value.id == selfId ? PlayerBarElement.clr.me : PlayerBarElement.clr.other)));
            }
            list.Add(0, new Vector3Str("Всего", sum, PlayerBarElement.clr.sum));
            while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(list, max, sum);
        }
        internal void updateHps(ulong selfId)
        {
            SortedList<double, Vector3Str> list = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                double hps = pair.Value.hps;
                sum += hps;
                max = Math.Max(hps, max);
                list.Add(hps, new Vector3Str(pair.Value.name, hps, (pair.Value.id == selfId ? PlayerBarElement.clr.me : PlayerBarElement.clr.other)));
            }
            list.Add(0, new Vector3Str("Всего", sum, PlayerBarElement.clr.sum));
            while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(list, max, sum);
        }
        internal void updateHeal(ulong selfId) {
            SortedList<double, Vector3Str> list = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                double heal = pair.Value.heal;
                sum += heal;
                max = Math.Max(heal, max);
                list.Add(heal, new Vector3Str(pair.Value.name, heal, (pair.Value.id == selfId ? PlayerBarElement.clr.me : PlayerBarElement.clr.other)));
            }
            list.Add(0, new Vector3Str("Всего", sum, PlayerBarElement.clr.sum));
            while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(list, max, sum);
        }
        internal void updateDamageTaken(ulong selfId)
        {
            SortedList<double, Vector3Str> list = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                double damageT = pair.Value.damageTaken;
                sum += damageT;
                max = Math.Max(damageT, max);
                list.Add(damageT, new Vector3Str(pair.Value.name, damageT, (pair.Value.id == selfId ? PlayerBarElement.clr.me : PlayerBarElement.clr.other)));
            }
            list.Add(0, new Vector3Str("Всего", sum, PlayerBarElement.clr.sum));
            while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(list, max, sum);
        }
        internal void updateHealTaken(ulong selfId)
        {
            SortedList<double, Vector3Str> list = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                double healT = pair.Value.healTaken;
                sum += healT;
                max = Math.Max(healT, max);
                list.Add(healT, new Vector3Str(pair.Value.name, healT, (pair.Value.id == selfId ? PlayerBarElement.clr.me : PlayerBarElement.clr.other)));
            }
            list.Add(0, new Vector3Str("Всего", sum, PlayerBarElement.clr.sum));
            while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(list, max, sum);
        }
        /// <summary>
        /// Comparer for comparing two keys, handling equality as beeing greater
        /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        public class DuplicateKeyComparer<TKey>:IComparer<TKey> where TKey : IComparable
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
        private void updateData(SortedList<double,Vector3Str> list, double max,double sum)
        {
            int i = 0;
            foreach (var pair in list)
            {
                (listBox.Items[i] as PlayerBarElement).changeData(
                    pair.Value.right / max * 100,
                    pair.Value.left,
                    generateRight(pair.Value.right,sum),
                    pair.Value.self);
                i++;
            }
        }
        static string[] kilos = new string[] { "", "K", "M", "B", "T", "q", "Q", "s", "S", "O", "N", "d" };
        private string generateRight(double val, double sum)
        {
            /*
             * 100.00                       100    100.00
             * 1000.0                     1 000    1000.0
             * 100.00K                   10 000    10000
             * 
             * 1000.0K                  100 000    100.00K
             * 100.00M                1 000 000    1000.0K
             *                       10 000 000    10000K
             *                       
             *                      100 000 000    100.00M
             *                    1 000 000 000    1000.0M
             *                   10 000 000 000    10000M
             *                  100 000 000 000
             *                1 000 000 000 000
             *               10 000 000 000 000
             *              100 000 000 000 000
             *            1 000 000 000 000 000
             *           10 000 000 000 000 000
             *          100 000 000 000 000 000
             *        1 000 000 000 000 000 000
             *       10 000 000 000 000 000 000
             *      100 000 000 000 000 000 000
             *    1 000 000 000 000 000 000 000
             *   10 000 000 000 000 000 000 000
             */

            int num = 0;
            double res = val;
            while (res >= 100000) { res /= 1000.0; num++; }
            int procent = (int)(val / sum * 100.0);
            if (procent < 0) procent = 0;
            else if (procent > 100) procent = 100;

            if (res < 1000)
            {
                return String.Format("{0:0.00}{1}({2}%)", res, kilos[num], procent);
            }
            if (res < 10000)
            {
                return String.Format("{0:0.0}{1}({2}%)", res, kilos[num], procent);
            }
            return String.Format("{0:0}{1}({2}%)", res, kilos[num], procent);
        }
        class Vector3Str
        {
            public string left;
            public double right;
            public PlayerBarElement.clr self;
            public Vector3Str(string left, double right, PlayerBarElement.clr self)
            {
                this.left = left;
                this.right = right;
                this.self = self;
            }
        }

        internal void addPlayer(TeraPlayer teraPlayer)
        {
            TeraPlayer player;
            if(!players.TryGetValue(teraPlayer.id,out player)) players.Add(teraPlayer.id,teraPlayer);
        }

        internal void clear()
        {
            players.Clear();
        }
    }
}
