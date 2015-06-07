using Detrav.TeraApi;
using Detrav.TeraApi.Core;
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
        Config config;
        public IAssetManager assetManager { get; set; }
        public IConfigManager configManager { set { config = new Config(value); } }
        public ITeraClient teraClient { get; set; }
        BitmapImage down;
        BitmapImage up;

        internal List<TeraSkill> history = new List<TeraSkill>();
        //Repository R;
        internal MainWindow()
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
            if (tabControl.SelectedContent is IDpsEngine)
                (tabControl.SelectedContent as IDpsEngine).teraClient = teraClient;
        }

        TeraPlayer self = new TeraPlayer(0, "unknown");

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
            if (tabControl.SelectedContent is IDpsEngine)
                (tabControl.SelectedContent as IDpsEngine).updateData(history.ToArray());
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            history.Clear();
            foreach(var el in tabControl.Items)
            {
                if ((el as TabItem).Content is IDpsEngine)
                    ((el as TabItem).Content as IDpsEngine).clear();
            }
        }

        public void doEvents()
        {
            if (tabControl.SelectedContent is IDpsEngine)
                (tabControl.SelectedContent as IDpsEngine).doEvents();
        }


        void saveCurrentConfig()
        {
            config.left = Left;
            config.top = Top;
            config.height = Height;
            config.width = Width;
            config.prevHeight = prevSize;
            config.hided = hided;
            config.save(self.name);
        }

        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"https://github.com/Detrav/Terometr", "Вам ко мне на GitHub!");
        }

        private void buttonBubble_Click(object sender, RoutedEventArgs e)
        {
            string result = null;
            if (tabControl.SelectedContent is IDpsEngine)
                result = (tabControl.SelectedContent as IDpsEngine).generateTable();
            if (result != null)
                MessageBox.Show(result, @"https://github.com/Detrav/Terometr");
        }

        

        internal void parent_onLogin(object sender, TeraApi.Events.Self.LoginEventArgs e)
        {
            if (self.id > 0)
                saveCurrentConfig();
            self = e.player;
            config.load(self.name);
            Left = config.left;
            Top = config.top;
            Height = config.height;
            Width = config.width;
            prevSize = config.prevHeight;
            hided = config.hided;
            Show();
        }

        internal void parent_onMakeSkillResult(object sender, SkillResultEventArgs e)
        {
            if(e.player.partyId>0)
            {
                TeraSkill skill;
                if (e.targetNpc != null) skill = new TeraSkill(e.player, SkillType.Make, e.type, e.damage, false, e.targetNpc.npc);
                else skill = new TeraSkill(e.player, SkillType.Make, e.type, e.damage);
                history.Add(skill);
                if (tabControl.SelectedContent is IDpsEngine)
                    (tabControl.SelectedContent as IDpsEngine).addSkill(skill);
            }
        }

        internal void parent_onTakeSkillResult(object sender, SkillResultEventArgs e)
        {
            if(e.player.partyId>0)
            {
                TeraSkill skill = new TeraSkill(e.player, SkillType.Take, e.type, e.damage);
                history.Add(skill);
                if (tabControl.SelectedContent is IDpsEngine)
                    (tabControl.SelectedContent as IDpsEngine).addSkill(skill);
            }
        }
    }
}
