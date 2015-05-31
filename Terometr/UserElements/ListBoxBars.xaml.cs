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

        public void updateDps(Dictionary<ulong, TeraPlayer> players,ulong selfId)
        {
            SortedList<double, Vector3Str> dpss = new SortedList<double, Vector3Str>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            foreach (var pair in players)
            {
                sum += pair.Value.dps.perSecond;
                max = Math.Max(pair.Value.dps.perSecond, max);
                dpss.Add(pair.Value.dps.perSecond, new Vector3Str(pair.Value.name,pair.Value.dps.perSecond,pair.Value.id == selfId));
            }
            while (listBox.Items.Count > dpss.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < dpss.Count) listBox.Items.Add(new PlayerBarElement());
            updateData(dpss, max, sum);
        }
        public void updateDamage(Dictionary<ulong, TeraPlayer> players, ulong selfId)
        {
            SortedList<double, TeraPlayer> dpss = new SortedList<double, TeraPlayer>(new DuplicateKeyComparer<double>());
            double max = 0;
            foreach (var pair in players)
            {
                max = Math.Max(pair.Value.dps.perSecond, max);
                dpss.Add(pair.Value.dps.perSecond, pair.Value);
            }
            while (listBox.Items.Count > dpss.Count) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < dpss.Count) listBox.Items.Add(new PlayerBarElement());
            int i = 0;
            foreach (var pair in dpss)
            {
                (listBox.Items[i] as PlayerBarElement).changeData(
                    pair.Value.dps.perSecond / max * 100,
                    pair.Value.name,
                    pair.Value.dps.perSecond.ToString(),
                    pair.Value.id == selfId);
                i++;
            }

        }
        public void updateHps(Dictionary<ulong, TeraPlayer> players, ulong selfId) { }
        public void updateHeal(Dictionary<ulong, TeraPlayer> players, ulong selfId) { }
        public void updateDpsTaken(Dictionary<ulong, TeraPlayer> players, ulong selfId) { }
        public void updateDamageTaken(Dictionary<ulong, TeraPlayer> players, ulong selfId) { }
        public void updateHpsTaken(Dictionary<ulong, TeraPlayer> players, ulong selfId) { }
        public void updateHealTaken(Dictionary<ulong, TeraPlayer> players, ulong selfId) { }

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

        private string generateRight(double val, double sum)
        {
            
        }
        class Vector3Str
        {
            public string left;
            public double right;
            public bool self;
            public Vector3Str(string left, double right, bool self)
            {
                this.left = left;
                this.right = right;
                this.self = self;
            }
        }
    }
}
