using Detrav.TeraApi;
using Detrav.TeraApi.Events;
using Detrav.TeraApi.Interfaces;
using Detrav.TeraApi.OpCodes;
using Detrav.Terometr.Core;
using Detrav.Terometr.UserElements;
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
            ((buttonBubble as Button).Content as Image).Source = Mod.ToImage("Detrav.Terometr.assets.images.Bubble.png");
            ((buttonNew as Button).Content as Image).Source = Mod.ToImage("Detrav.Terometr.assets.images.New.png");
            ((buttonBack as Button).Content as Image).Source = Mod.ToImage("Detrav.Terometr.assets.images.Back.png");
            ((buttonForward as Button).Content as Image).Source = Mod.ToImage("Detrav.Terometr.assets.images.Forward.png");
            ((buttonInfo as Button).Content as Image).Source = Mod.ToImage("Detrav.Terometr.assets.images.Info.png");
            down = Mod.ToImage("Detrav.Terometr.assets.images.Bottom.png");
            up = Mod.ToImage("Detrav.Terometr.assets.images.Top.png");
            ((buttonHide as Button).Content as Image).Source = up;
        }

        TeraPlayer self = new TeraPlayer(0,"UNKNOWN", TeraApi.Enums.PlayerClass.Empty);

        public void changeTitle(string str)
        {
            this.Title = str;
            //this.Title = String.Format("Terometr - {0} - {1}", self.name, str);
            this.UpdateLayout();
        }

        public bool close = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveCurrentConfig();
            e.Cancel = !close;
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
            Repository.R.clear();
            /*listDps.clear();
            listHps.clear();
            listDamage.clear();
            listHeal.clear();
            listDamageTaken.clear();
            listHealTaken.clear();
            foreach(var pair in party)
            {
                pair.Value.clear();
            }
            self.clear();*/
        }

        //bool valueNotPerSecond;
        public void doEvents()
        {
            Repository.R.doEvents();
            switch (tabControl.SelectedIndex)
            {
                case 0: listDps.updateData(Repository.R.dpsList, Repository.R.dpsMax, Repository.R.dpsSum); break;
                case 1: listDamage.updateData(Repository.R.damageList, Repository.R.damageMax, Repository.R.damageSum); break;
                case 2: listHps.updateData(Repository.R.hpsList, Repository.R.hpsMax, Repository.R.hpsSum); break;
                case 3: listHeal.updateData(Repository.R.healList, Repository.R.healMax, Repository.R.healSum); break;
                case 4: listDamageTaken.updateData(Repository.R.damageTakenList, Repository.R.damageTakenMax, Repository.R.damageTakenSum); break;
                case 5: listHealTaken.updateData(Repository.R.healTakenList, Repository.R.healTakenMax, Repository.R.healTakenSum); break;
            }
        }




        internal void opPacketArrival(object sender, PacketArrivalEventArgs e)
        {
            switch(Repository.R.P2904(sender,e))
            {
                case OpCode2904.S_LOGIN: self = Repository.R.getSelf(); login(); break;
            }
            //TeraApi.OpCodes.
        }

        //IConfigManager config;
        //Config localConfig = null;
        void login()
        {
            Config.load(self.name);
            Left = Config.c.left;
            Top = Config.c.top;
            Height = Config.c.height;
            Width = Config.c.width;
            prevSize = Config.c.prevHeight;
            hided = Config.c.hided;
            Show();
        }

        void saveCurrentConfig()
        {

            Config.c.left = Left;
            Config.c.top = Top;
            Config.c.height = Height;
            Config.c.width = Width;
            Config.c.prevHeight = prevSize;
            Config.c.hided = hided;
            Config.save(self.name);
        }

        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"https://github.com/Detrav/Terometr", "Вам ко мне на GitHub!");
        }

        private void buttonBubble_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            PlayerBarElement [] list = null;
            switch(tabControl.SelectedIndex)
            {
                case 0: list = listDps.getList(); break;
                case 1: list = listDamage.getList(); break;
                case 2: list = listHps.getList(); break;
                case 3: list = listHeal.getList(); break;
                case 4: list = listDamageTaken.getList(); break;
                case 5: list = listHealTaken.getList(); break;
            }
            if (list == null) return;
            //sb.AppendFormat("Terometr - {0} - {1}", self.name, (tabControl.SelectedItem as TabItem).Header); sb.Append(Environment.NewLine);
            string title = String.Format("Terometr - {0} - {1}", self.name, (tabControl.SelectedItem as TabItem).Header); sb.Append(Environment.NewLine);
            foreach(var el in list)
            {
                sb.Append(el.getText()); sb.Append(Environment.NewLine);
            }
            sb.Append("Постоянные обновления на:"); sb.Append(Environment.NewLine);
            sb.Append(@"https://github.com/Detrav/Terometr");
            MessageBox.Show(sb.ToString(), title);
        }
    }
}
