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
        double maxDps;
        double sumDps;
        int rowCount;

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
            if (!db.TryGetValue(id, out local))
            {
                local = new LocalRow(id, cls, name);
                db[id] = local;
            }
            local.updateValue(crit,damage,dps);
        }

        public void updateLayout()
        {
            IOrderedEnumerable<LocalRow> result = null;
            if (toggleButtonClass.IsChecked == true)
                result = db.Values.OrderBy(a => a.cls);
            else if (toggleButtonName.IsChecked == true)
                result = db.Values.OrderBy(a => a.name);
            else if (toggleButtonCrit.IsChecked == true)
                result = db.Values.OrderBy(a => a.crit);
            else if (toggleButtonDamage.IsChecked == true)
                result = db.Values.OrderBy(a => a.damage);
            else if (toggleButtonDps.IsChecked == true)
                result = db.Values.OrderBy(a => a.dps);
            if (result == null)
            {
                rowCount = 0;
                checkRows();
            }
            if(result!=null)
            {
                rowCount = result.Count();
                checkRows();
                int i = 0;
                foreach(var el in result)
                {
                    updateRow(i, el);
                    i++;
                }
            }
        }
        private void updateRow(int num, LocalRow row)
        {
            (panelClass.Children[num] as Image).Source = row.cls;
            (panelName.Children[num] as Label).Content = row.name;
            (panelCrit.Children[num] as Label).Content = row.critRate;
            (panelDamage.Children[num] as Label).Content = row.value(sum);
            (panelDps.Children[num] as Label).Content = row.vps(sumDps);
        }
        private void checkRows()
        {

            while (panelClass.Children.Count < rowCount) panelClass.Children.Add(getImage());
            while (panelClass.Children.Count > rowCount) panelClass.Children.RemoveAt(rowCount);

            while (panelName.Children.Count < rowCount) panelName.Children.Add(getLabel());
            while (panelName.Children.Count > rowCount) panelName.Children.RemoveAt(rowCount);

            while (panelCrit.Children.Count < rowCount) panelCrit.Children.Add(getLabel());
            while (panelCrit.Children.Count > rowCount) panelCrit.Children.RemoveAt(rowCount);

            while (panelDamage.Children.Count < rowCount) panelDamage.Children.Add(getLabel());
            while (panelDamage.Children.Count > rowCount) panelDamage.Children.RemoveAt(rowCount);

            while (panelDps.Children.Count < rowCount) panelDps.Children.Add(getLabel());
            while (panelDps.Children.Count > rowCount) panelDps.Children.RemoveAt(rowCount);
        }

        private Label getLabel()
        {
            Label l = new Label();
            l.Style = labelAll.Style;
            return l;
        }
        private Image getImage()
        {
            Image img = new Image();
            img.Margin = new Thickness(0, 0, 0, 0);
            img.Width = 16;
            img.Height = 16;
            return img;
        }


        private class LocalRow
        {
            public ulong id;
            public ImageSource cls;
            public string name;
            public string critRate { get { return String.Format("{0}%", (int)crit); } }
            public string value(double sum) { return MetrEngine.generateShort(damage,sum); }
            public string vps(double sum) { return MetrEngine.generateShort(dps, sum); } 
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
            return "";
        }

        internal void clear()
        {
            db.Clear();
            updateSum(0, 0, 0, 0);
        }

        internal void updateSum(double sum, double max, double sumDps, double maxDps)
        {
            this.sum = sum;
            this.max = max;
            this.sumDps = sumDps;
            this.maxDps = maxDps;
            labelDamage.Content = sum;
            labelDps.Content = sumDps;
        }
    }
}
