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

namespace Detrav.Terometr.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage down;
        BitmapImage up;
        public MainWindow()
        {
            InitializeComponent();
            ((buttonBubble as Button).Content as Image).Source = ToImage("Detrav.Terometr.assets.images.Bubble.png");
            ((buttonNew as Button).Content as Image).Source = ToImage("Detrav.Terometr.assets.images.New.png");
            ((buttonBack as Button).Content as Image).Source = ToImage("Detrav.Terometr.assets.images.Back.png");
            ((buttonForward as Button).Content as Image).Source = ToImage("Detrav.Terometr.assets.images.Forward.png");
            ((buttonInfo as Button).Content as Image).Source = ToImage("Detrav.Terometr.assets.images.Info.png");
            down = ToImage("Detrav.Terometr.assets.images.Bottom.png");
            up = ToImage("Detrav.Terometr.assets.images.Top.png");
            ((buttonHide as Button).Content as Image).Source = up;
            
        }

        Dictionary<ulong, TeraPlayer> party = new Dictionary<ulong,TeraPlayer>();
        TeraPlayer self = new TeraPlayer(0,"UNKNOWN",true);

        public void changeTitle(string str)
        {
            this.Title = str;
            //this.Title = String.Format("Terometr - {0} - Дпс метр", str);
            this.UpdateLayout();
        }

        public bool close = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = !close;
            this.Hide();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
        }


        bool hided = false;
        double prevSize = 0;
        private void buttonHide_Click(object sender, RoutedEventArgs e)
        {
            if(hided)
            {
                Height = prevSize;
                hided = false;
                ((buttonHide as Button).Content as Image).Source = up;
                return;
            }

            prevSize = Height;
            Height = 16;
            hided = true;
            ((buttonHide as Button).Content as Image).Source = down;
        }

        public BitmapImage ToImage(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = resFilestream;
                image.EndInit();
                return image;

            }
        }

        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == tabControl.Items.Count-1)
            {
                tabControl.SelectedIndex = 0;
                return;
            }
            tabControl.SelectedIndex++;
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                tabControl.SelectedIndex = tabControl.Items.Count - 1;
                return;
            }
            tabControl.SelectedIndex--;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeTitle((tabControl.SelectedItem as TabItem).Header.ToString());
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            foreach(var pair in party)
            {
                pair.Value.clear();
            }
            self.clear();
        }


        public void doEvents()
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    {
                        SortedList<double, TeraPlayer> dpss = new SortedList<double, TeraPlayer>(new DuplicateKeyComparer<double>());
                        foreach (var pair in party)
                        {
                            if (!pair.Value.inParty) continue;
                            if (pair.Value.dps == null) continue;
                            if (pair.Value.dps.perSecond <= 0) continue;
                            dpss.Add(pair.Value.dps.perSecond, pair.Value);
                        }
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }

        /// <summary>
        /// Comparer for comparing two keys, handling equality as beeing greater
        /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        public class DuplicateKeyComparer<TKey>
                        :
                     IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1;   // Handle equality as beeing greater
                else
                    return result;
            }

            #endregion
        }

    }
}
