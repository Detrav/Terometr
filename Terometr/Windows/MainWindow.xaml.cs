using Detrav.TeraApi;
using Detrav.TeraApi.Events;
using Detrav.TeraApi.Interfaces;
using Detrav.TeraApi.OpCodes;
using Detrav.TeraApi.OpCodes.P2904;
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
        public MainWindow(IConfigManager config)
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
            this.config = config;
        }

        Dictionary<ulong, TeraPlayer> party = new Dictionary<ulong,TeraPlayer>();
        Dictionary<ulong, ulong> projectiles = new Dictionary<ulong, ulong>();
        Dictionary<ulong, ulong> npcs = new Dictionary<ulong, ulong>();
        TeraPlayer self = new TeraPlayer(0,"UNKNOWN");

        public void changeTitle(string str)
        {
            this.Title = str;
            //this.Title = String.Format("Terometr - {0} - {1}", self.name, str);
            this.UpdateLayout();
        }

        public bool close = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
            listDps.clear();
            listHps.clear();
            listDamage.clear();
            listHeal.clear();
            listDamageTaken.clear();
            listHealTaken.clear();
            foreach(var pair in party)
            {
                pair.Value.clear();
            }
            self.clear();
        }

        //bool valueNotPerSecond;
        public void doEvents()
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    foreach (var pair in party)
                        if (pair.Value.damage > 0) listDps.addPlayer(pair.Value);
                    listDps.updateDps(self.id);
                    break;
                case 1:
                    foreach (var pair in party)
                        if (pair.Value.damage > 0) listDamage.addPlayer(pair.Value);
                    listDamage.updateDamage(self.id);
                    break;
                case 2:
                    foreach (var pair in party)
                        if (pair.Value.heal > 0) listHps.addPlayer(pair.Value);
                    listHps.updateHps(self.id);
                    break;
                case 3:
                    foreach (var pair in party)
                        if (pair.Value.heal > 0) listHeal.addPlayer(pair.Value);
                    listHeal.updateHeal(self.id);
                    break;
                case 4:
                    foreach (var pair in party)
                        if (pair.Value.damageTaken > 0) listDamageTaken.addPlayer(pair.Value);
                    listDamageTaken.updateDamageTaken(self.id);
                    break;
                case 5:
                    foreach (var pair in party)
                        if (pair.Value.healTaken > 0) listHealTaken.addPlayer(pair.Value);
                    listHealTaken.updateHealTaken(self.id);
                    break;
            }
        }




        internal void opPacketArrival(object sender, PacketArrivalEventArgs e)
        {
            switch((OpCode2904)e.packet.opCode)
            {
                case OpCode2904.S_LOGIN:
                    saveCurrentConfig();
                    var s_login = (S_LOGIN)PacketCreator.create(e.packet);
                    self = new TeraPlayer(s_login.id, s_login.name);
                    party.Add(self.id, self);
                    login();
                    break;
                case OpCode2904.S_PARTY_MEMBER_LIST:
                    Logger.debug("S_PARTY_MEMBER_LIST");
                    var s_party_list = (S_PARTY_MEMBER_LIST)PacketCreator.create(e.packet);
                    party.Clear();
                    foreach(var p in s_party_list.players)
                    {
                        Logger.debug("AddtoParty {0}", p.name);
                        party.Add(p.id, new TeraPlayer(p.id, p.name));
                    }
                    break;
                case OpCode2904.S_LEAVE_PARTY:
                    Logger.debug("S_LEAVE_PARTY");
                    party.Clear();
                    party.Add(self.id, self);
                    break;
                case OpCode2904.S_LEAVE_PARTY_MEMBER:
                    var s_leave_member = (S_LEAVE_PARTY_MEMBER)PacketCreator.create(e.packet);
                    ulong tempPlayer = 0;
                    foreach(var pair in party)
                    {
                        if (pair.Value.name == s_leave_member.name)
                        {
                            Logger.debug("S_LEAVE_PARTY_MEMBER", s_leave_member.name);
                            tempPlayer = pair.Key;
                            break;
                        }
                    }
                    if (tempPlayer > 0)
                        party.Remove(tempPlayer);
                    break;
                case OpCode2904.S_SPAWN_PROJECTILE:
                    
                    var s_spawn_proj = (S_SPAWN_PROJECTILE)PacketCreator.create(e.packet);
                    projectiles.Add(s_spawn_proj.id, s_spawn_proj.idPlayer);
                    break;
                case OpCode2904.S_DESPAWN_PROJECTILE:
                    var s_despawn_proj = (S_DESPAWN_PROJECTILE)PacketCreator.create(e.packet);
                    if (projectiles.Keys.Contains(s_despawn_proj.id))
                        projectiles.Remove(s_despawn_proj.id);
                    break;
                case OpCode2904.S_SPAWN_NPC:
                    var s_spawn_npc = (S_SPAWN_NPC)PacketCreator.create(e.packet);
                    if (s_spawn_npc.parentId > 0)
                        if (party.Keys.Contains(s_spawn_npc.parentId))
                            if (!npcs.Keys.Contains(s_spawn_npc.id))
                                npcs.Add(s_spawn_npc.id, s_spawn_npc.parentId);
                    break;
                case OpCode2904.S_DESPAWN_NPC:
                    var s_despawn_npc = (S_DESPAWN_NPC)PacketCreator.create(e.packet);
                    if (npcs.Keys.Contains(s_despawn_npc.id))
                        npcs.Remove(s_despawn_npc.id);
                    break;
                case OpCode2904.S_EACH_SKILL_RESULT:
                    {
                        /*
                         * Теперь проверяем так, если атакует Прожетил, то чекает нпс, если чекнули нпс то чекаем игрока, нужно вынесты в отдельные функции
                         */
                        var skill = (S_EACH_SKILL_RESULT)PacketCreator.create(e.packet);
                        #region ИгрокАтакует
                        {
                            ulong projectile;//Если нету прожектила то просто ищем по скилу, который присваеваем прожектилу
                            if (!projectiles.TryGetValue(skill.idWho, out projectile)) projectile = skill.idWho;
                            TeraPlayer p;
                            if (!npcs.TryGetValue(projectile, out projectile)) projectile = skill.idWho;
                            if (!party.TryGetValue(projectile, out p)) p = null;
                            if (p != null)
                            {
                                Logger.debug("Player Attack {0}", p.name);
                                p.makeSkill(skill.damage, skill.dType);
                                //return;
                            }
                        }
                        #endregion ИгрокАтакует
                        #region ИгрокаАтакуют
                        {

                            TeraPlayer p;
                            if (!party.TryGetValue(skill.idTarget, out p)) p = null;
                            if (p != null)
                            {
                                Logger.debug("Player Take Attack {0}", p.name);
                                p.takeSkill(skill.damage, skill.dType);
                            }
                        }
                        #endregion ИгрокаАтакуют
                    }
                    break;
            }
            //TeraApi.OpCodes.
        }

        IConfigManager config;
        Config localConfig = null;
        void login()
        {
            if (localConfig == null)
            {
                localConfig = new Config()
                {
                    left = Left,
                    top = Top,
                    height = Height,
                    width = Width,
                    prevHeight = prevSize,
                    hided = this.hided
                };
            }
            var conf = config.loadPlayer(self.name, localConfig.GetType());
            if (conf == null) config.savePlayer(self.name, localConfig);
            else localConfig = conf as Config;

            Left = localConfig.left;
            Top = localConfig.top;
            Height = localConfig.height;
            Width = localConfig.width;
            prevSize = localConfig.prevHeight;
            hided = localConfig.hided;
            Show();
        }

        void saveCurrentConfig()
        {
            if (localConfig == null) return;
            localConfig = new Config()
            {
                left = Left,
                top = Top,
                height = Height,
                width = Width,
                prevHeight = prevSize,
                hided = this.hided
            };
            config.savePlayer(self.name, localConfig);
        }
    }
}
