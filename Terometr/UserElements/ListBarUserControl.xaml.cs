using Detrav.TeraApi.Enums;
using Detrav.Terometr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для ListBarUserControl.xaml
    /// </summary>
    public partial class ListBarUserControl : UserControl
    {
        public ListBarUserControl()
        {
            InitializeComponent();
            green = (Brush)br.ConvertFrom("#FF10AE00");
            blue = (Brush)br.ConvertFrom("#FF1000AE");
            black = (Brush)br.ConvertFrom("#FF000000");
        }

        BrushConverter br = new BrushConverter();
        Brush green;
        Brush blue;
        Brush black;
        Dictionary<ulong, LocalRow> db = new Dictionary<ulong,LocalRow>();

        double sum;
        double max;

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            toggleButtonClass.IsChecked = false;
            toggleButtonName.IsChecked = false;
            toggleButtonCrit.IsChecked = false;
            toggleButtonDamage.IsChecked = false;
            toggleButtonDps.IsChecked = false;

            if (sender is ToggleButton)
                (sender as ToggleButton).IsChecked = true;
        }

        public void updateData(ulong id, PlayerClass cls, string name, double crit,double damage,double dps)
        {
            LocalRow local;
            if(!db.TryGetValue(id, out local))
                db[id] = new LocalRow(id,cls,name);
            local.updateValue(crit,damage,dps);
        }

        public void updateSum(double sum,double max)
        {
            this.sum = sum;
            this.max = max;
        }

        public void updateLayout()
        {

        }

        private class LocalRow
        {
            public ulong id;
            public ImageSource cls;
            public string name;
            public double crit;
            public double damage;
            public double dps;
            public LocalRow(ulong id, PlayerClass cls, string name)
            {
                this.id = id;
                switch (cls)
                {
                    case PlayerClass.Archer: this.cls = MetrEngine.archer; break;
                    case PlayerClass.Berserker: this.cls = MetrEngine.berserker; break;
                    case PlayerClass.Lancer: this.cls = MetrEngine.lancer; break;
                    case PlayerClass.Mystic: this.cls = MetrEngine.mystic; break;
                    case PlayerClass.Priest: this.cls = MetrEngine.priest; break;
                    case PlayerClass.Reaper: this.cls = MetrEngine.reaper; break;
                    case PlayerClass.Slayer: this.cls = MetrEngine.slayer; break;
                    case PlayerClass.Sorcerer: this.cls = MetrEngine.sorcerer; break;
                    case PlayerClass.Warrior: this.cls = MetrEngine.warrior; break;
                    default: this.cls = null; break;
                }
                this.name = name;
            }
            public void updateValue(double crit,double damage,double dps)
            {
                this.crit = crit;
                this.damage = damage;
                this.dps = dps;
            }
        }

        internal string generateTable()
        {
            throw new NotImplementedException();
        }

        internal void clear()
        {
            throw new NotImplementedException();
        }

        internal void updateSum(double sum, double max, double sumDps, double maxDps)
        {
            throw new NotImplementedException();
        }
    }
}
