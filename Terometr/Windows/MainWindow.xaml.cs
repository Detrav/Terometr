﻿using Detrav.TeraApi;
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
        public IConfigManager configManager
        {
            set
            {
                config = new Config(value);
                foreach (var el in tabControl.Items)
                    if ((el as TabItem).Content is IDpsUIEngine)
                        ((el as TabItem).Content as IDpsUIEngine).reSetting(config);
            }
        }
        public ITeraClient teraClient { get; set; }
        BitmapImage down;
        BitmapImage up;

        //internal List<TeraSkill> history = new List<TeraSkill>();
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
            self = new TeraPlayer(0, "unknown");

            tabControl.Items.Insert(tabControl.Items.Count - 1, new TabItem() { Header = "Урон", Content = new DamageEngineUserControl(IDpsUIEngineType.damage,"Цель:",dataGridTable) });
            tabControl.Items.Insert(tabControl.Items.Count - 1, new TabItem() { Header = "Лечение", Content = new DamageEngineUserControl(IDpsUIEngineType.heal, "Цель:", dataGridTable) });
            tabControl.Items.Insert(tabControl.Items.Count - 1, new TabItem() { Header = "Пол. урона", Content = new DamageEngineUserControl(IDpsUIEngineType.damageTaken, "От:", dataGridTable) });
            tabControl.Items.Insert(tabControl.Items.Count - 1, new TabItem() { Header = "Пол. леч.", Content = new DamageEngineUserControl(IDpsUIEngineType.healTaken, "От:", dataGridTable) });
            dataGridTable.dignCount = (int)dignCountSlider.Value;
            dignCountLabel.Content = (int)dignCountSlider.Value;
        }

        TeraPlayer self
        {
            get { return _self; }
            set
            {
                _self = value;
                foreach (var el in tabControl.Items)
                {
                    if ((el as TabItem).Content is IDpsUIEngine)
                        ((el as TabItem).Content as IDpsUIEngine).setSelf(_self);
                }
            }
        }
        TeraPlayer _self;
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
            if (tabControl.SelectedContent is DamageEngineUserControl)
            {
                dataGridTable.clear();
                (tabControl.SelectedContent as DamageEngineUserControl).needToUpdate = true;
            }
            changeTitle((tabControl.SelectedItem as TabItem).Header.ToString());
            /*if (tabControl.SelectedContent is IDpsEngine)
                (tabControl.SelectedContent as IDpsEngine).updateData(history.ToArray());*/
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите очистить?", "Очистка", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            dataGridTable.clear();
            dataGridTable.endUpdate();
            foreach(var el in tabControl.Items)
                if ((el as TabItem).Content is IDpsUIEngine)
                    ((el as TabItem).Content as IDpsUIEngine).clear();
        }

        public void doEvents()
        {
            if (tabControl.SelectedContent is IDpsUIEngine)
                (tabControl.SelectedContent as IDpsUIEngine).doEvents();
        }


        void saveCurrentConfig()
        {
            config.left = Left;
            config.top = Top;
            config.height = Height;
            config.width = Width;
            config.prevHeight = prevSize;
            config.hided = hided;
            config.party = toggleButtonParty.IsChecked == true;
            config.log = toggleButtonLog.IsChecked == true;
            config.player = toggleButtonPlayer.IsChecked == true;
            config.group = toggleButtonGroup.IsChecked == true;
            config.autoTarget = toggleButtonAutoTarget.IsChecked == true;
            config.sortNumber = dataGridTable.rowNumber;
            config.classVisible = dataGridTable.visibleClass;
            config.critVisible = dataGridTable.visibleCrit;
            config.damageVisible = dataGridTable.visibleDamage;
            config.dpsVisible = dataGridTable.visibleDps;
            config.save(self.name);
        }

        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"https://github.com/Detrav/Terometr", "Вам ко мне на GitHub!");
        }

        private void buttonBubble_Click(object sender, RoutedEventArgs e)
        {
            string result = null;
            if (tabControl.SelectedContent is IDpsUIEngine)
                result = (tabControl.SelectedContent as IDpsUIEngine).generateTable();
            if (result == null)
                result = "Ошибка";
            result = String.Format("Terrometr - {0} - {1}{2}{3}{4}", self.safeName,this.Title, Environment.NewLine, result, @"https://github.com/Detrav");
            Clipboard.SetText(result);
#if DEBUG
            MessageBox.Show(result, "Скопированно в буфер обмена");
#else
            MessageBox.Show("Скопированно в буфер обмена", "Скопированно в буфер обмена");
#endif
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
            toggleButtonParty.IsChecked = config.party;
            toggleButtonLog.IsChecked = config.log;
            toggleButtonPlayer.IsChecked = config.player;
            toggleButtonGroup.IsChecked = config.group;
            toggleButtonAutoTarget.IsChecked = config.autoTarget;
            dignCountSlider.Value = config.dignCount;
            dataGridTable.rowNumber = config.sortNumber;
            dataGridTable.visibleClass = config.classVisible;
            dataGridTable.visibleCrit = config.critVisible;
            dataGridTable.visibleDamage = config.damageVisible;
            dataGridTable.visibleDps = config.dpsVisible;
            foreach (var el in tabControl.Items)
                if ((el as TabItem).Content is IDpsUIEngine)
                    ((el as TabItem).Content as IDpsUIEngine).reSetting(config);
            Show();
        }

        internal void parent_onSkillResult(object sender, SkillResultEventArgs e)
        {
            //Отсеиваем нули
            if (e.target == null) return;
            if (e.who == null) return;
            foreach (var el in tabControl.Items)
                if ((el as TabItem).Content is IDpsUIEngine)
                    ((el as TabItem).Content as IDpsUIEngine).skillResult(e);
        }

        /*internal void parent_onMakeSkillResult(object sender, SkillResultEventArgs e)
        {
            if(e.player.partyId>0)
            {
                TeraSkill skill;
                if (e.targetNpc != null) skill = new TeraSkill(e.player, SkillType.Make, e.type, e.damage, false, e.targetNpc);
                else skill = new TeraSkill(e.player, SkillType.Make, e.type, e.damage);
                history.Add(skill);
                //SLogger.debug("new skill {0} {1}", e.player.name, e.damage);
                foreach (var el in tabControl.Items)
                    if ((el as TabItem).Content is IDpsUIEngine)
                        ((el as TabItem).Content as IDpsUIEngine).addSkill(skill);
            }
        }

        internal void parent_onTakeSkillResult(object sender, SkillResultEventArgs e)
        {
            if(e.player.partyId>0)
            {
                TeraSkill skill = new TeraSkill(e.player, SkillType.Take, e.type, e.damage);
                history.Add(skill);
                foreach (var el in tabControl.Items)
                    if ((el as TabItem).Content is IDpsUIEngine)
                        ((el as TabItem).Content as IDpsUIEngine).addSkill(skill);
            }
        }*/

        private void buttonConfigSave_Click(object sender, RoutedEventArgs e)
        {
            if (self.id > 0)
            {
                saveCurrentConfig();
                foreach (var el in tabControl.Items)
                    if ((el as TabItem).Content is IDpsUIEngine)
                        ((el as TabItem).Content as IDpsUIEngine).reSetting(config);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayout();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (config != null)
            {
                dataGridTable.dignCount = config.dignCount;
                dignCountLabel.Content = config.dignCount;
                config.dignCount = (int)dignCountSlider.Value;
                if (self.id > 0)
                {
                    saveCurrentConfig();
                    foreach (var el in tabControl.Items)
                        if ((el as TabItem).Content is IDpsUIEngine)
                            ((el as TabItem).Content as IDpsUIEngine).reSetting(config);
                }
            }
        }
    }
}