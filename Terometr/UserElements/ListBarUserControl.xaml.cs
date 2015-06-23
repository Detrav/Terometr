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
        public ulong selfId { get; set; }

        BrushConverter br = new BrushConverter();
        Brush green;
        Brush blue;
        Brush black;
        Dictionary<ulong, LocalRow> db = new Dictionary<ulong,LocalRow>();

        double sum;
        double max;
        double maxDps;
        double sumDps;
        double sumCrt;
        double maxCrt;
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
            endUpdate();
        }
        public void beginUpdate()
        {
            db.Clear();
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

        public void endUpdate()
        {
            IOrderedEnumerable<LocalRow> result = null;
            if (toggleButtonClass.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.cls);
            else if (toggleButtonName.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.name);
            else if (toggleButtonCrit.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.crit);
            else if (toggleButtonDamage.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.damage);
            else if (toggleButtonDps.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.dps);
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
            ProgressBar b = (panelProgressBars.Children[num] as ProgressBar);
            if (row.id == selfId) b.Foreground = green;
            else b.Foreground = blue;
            double progressValue = 100;
            if (toggleButtonDamage.IsChecked == true) progressValue = row.damage / max * 100.0;
            else if (toggleButtonDps.IsChecked == true) progressValue = row.dps / maxDps * 100.0;
            else if (toggleButtonCrit.IsChecked == true) progressValue = row.crit / maxCrt * 100.0;
            if (progressValue > 100 || Double.IsInfinity(progressValue) || Double.IsNaN(progressValue)) progressValue = 100;
            else if (progressValue < 0) progressValue = 0;
            b.Value = progressValue;
        }
        private void checkRows()
        {

            while (panelClass.Children.Count < rowCount) panelClass.Children.Add(getImage());
            while (panelClass.Children.Count > rowCount) panelClass.Children.RemoveAt(rowCount);

            while (panelName.Children.Count < rowCount) panelName.Children.Add(getLabelLeft());
            while (panelName.Children.Count > rowCount) panelName.Children.RemoveAt(rowCount);

            while (panelCrit.Children.Count < rowCount) panelCrit.Children.Add(getLabelRight());
            while (panelCrit.Children.Count > rowCount) panelCrit.Children.RemoveAt(rowCount);

            while (panelDamage.Children.Count < rowCount) panelDamage.Children.Add(getLabelRight());
            while (panelDamage.Children.Count > rowCount) panelDamage.Children.RemoveAt(rowCount);

            while (panelDps.Children.Count < rowCount) panelDps.Children.Add(getLabelRight());
            while (panelDps.Children.Count > rowCount) panelDps.Children.RemoveAt(rowCount);

            while (panelProgressBars.Children.Count < rowCount) panelProgressBars.Children.Add(getProgressBar());
            while (panelProgressBars.Children.Count > rowCount) panelProgressBars.Children.RemoveAt(rowCount);
        }

        private Label getLabelLeft()
        {
            Label l = new Label();
            l.Style = labelAll.Style;
            l.MaxWidth = 75;
            l.Height = 16;
            l.Margin = new Thickness(1, 0, 1, 0);
            l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            return l;
        }

        private Label getLabelRight()
        {
            Label l = new Label();
            l.Style = labelAll.Style;
            l.Height = 16;
            l.Margin = new Thickness(1, 0, 1, 0);
            l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
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

        private ProgressBar getProgressBar()
        {
            ProgressBar p = new ProgressBar();
            p.BorderBrush = Brushes.Transparent;
            p.Background = Brushes.Transparent;
            p.Height = 16;
            return p;
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
                this.crit = crit * 100;
                this.damage = damage;
                this.dps = dps;
            }
        }

        internal string generateTable()
        {
            IOrderedEnumerable<LocalRow> result = null;
            if (toggleButtonClass.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.cls);
            else if (toggleButtonName.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.name);
            else if (toggleButtonCrit.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.crit);
            else if (toggleButtonDamage.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.damage);
            else if (toggleButtonDps.IsChecked == true)
                result = db.Values.OrderByDescending(a => a.dps);
            if (result == null) return "Empty!";
            if (result.Count() == 0) return "Empty!";
            TableWriter tw = new TableWriter(5);
            tw.addRow("#","Имя","Шанс Крита","Количетсво","Кол/В сек.");
            int i =0;
            foreach(var el in result)
            {
                tw.addRow(i, el.name, el.critRate, el.value(sum), el.vps(sumDps));
                i++;
            }
            tw.addRow(i, "Всего:", String.Format("{0}%", (int)this.sumCrt), MetrEngine.generateShort(sum), MetrEngine.generateShort(sumDps));
            return tw.ToString();
        }

        internal void clear()
        {
            db.Clear();
            updateSum(0, 0, 0, 0,0,0);
        }

        internal void updateSum(double sum, double max, double sumDps, double maxDps,double sumCrt, double maxCrt)
        {
            this.sum = sum;
            this.max = max;
            this.sumDps = sumDps;
            this.maxDps = maxDps;
            this.sumCrt = sumCrt*100;
            this.maxCrt = maxCrt*100;
            labelDamage.Content = MetrEngine.generateShort(sum);
            labelDps.Content = MetrEngine.generateShort(sumDps);
            labelCrt.Content = String.Format("{0}%",(int)this.sumCrt);
        }

        private void checkClass_Checked(object sender, RoutedEventArgs e)
        {
            gridClass.Visibility = System.Windows.Visibility.Visible;
        }

        private void checkClass_Unchecked(object sender, RoutedEventArgs e)
        {
            gridClass.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void checkCrit_Checked(object sender, RoutedEventArgs e)
        {
            gridCrit.Visibility = System.Windows.Visibility.Visible;
        }

        private void checkCrit_Unchecked(object sender, RoutedEventArgs e)
        {
            gridCrit.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void checkDamage_Checked(object sender, RoutedEventArgs e)
        {
            gridDamage.Visibility = System.Windows.Visibility.Visible;
        }

        private void checkDamage_Unchecked(object sender, RoutedEventArgs e)
        {
            gridDamage.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void checkDps_Checked(object sender, RoutedEventArgs e)
        {
            gridDps.Visibility = System.Windows.Visibility.Visible;
        }

        private void checkDps_Unchecked(object sender, RoutedEventArgs e)
        {
            gridDps.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
