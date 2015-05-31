using Detrav.TeraApi;
using Detrav.TeraApi.Events;
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
        Dictionary<ulong, ulong> projectiles = new Dictionary<ulong, ulong>();
        TeraPlayer self = new TeraPlayer(0,"UNKNOWN");
        Dictionary<ulong, TeraPlayer> dpsPlayers = new Dictionary<ulong, TeraPlayer>();
        Dictionary<ulong, TeraPlayer> hpsPlayers = new Dictionary<ulong, TeraPlayer>();

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
            dpsPlayers.Clear();
            hpsPlayers.Clear();
            foreach(var pair in party)
            {
                pair.Value.clear();
            }
            self.clear();
        }

        bool valueNotPerSecond;
        public void doEvents()
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    {
                        TeraPlayer player;
                        
                        foreach (var pair in party)
                        {
                            if (pair.Value.dps == null) continue;
                            if (pair.Value.dps.perSecond <= 0) continue;
                            if (!dpsPlayers.TryGetValue(pair.Key, out player))
                                dpsPlayers.Add(pair.Key, pair.Value);
                        }
                        
                    }
                    break;
                case 1:
                    {
                        TeraPlayer player;
                        SortedList<double, TeraPlayer> hpss = new SortedList<double, TeraPlayer>(new DuplicateKeyComparer<double>());
                        foreach (var pair in party)
                        {
                            if (pair.Value.hps == null) continue;
                            if (pair.Value.hps.perSecond <= 0) continue;
                            if (!hpsPlayers.TryGetValue(pair.Key, out player))
                                hpsPlayers.Add(pair.Key, pair.Value);
                        }
                        double max = 0;
                        foreach (var pair in hpsPlayers)
                        {
                            max = Math.Max(pair.Value.hps.perSecond, max);
                            hpss.Add(pair.Value.hps.perSecond, pair.Value);
                        }
                        while (listBoxHps.Items.Count > hpss.Count) listBoxHps.Items.RemoveAt(0);
                        while (listBoxHps.Items.Count < hpss.Count) listBoxHps.Items.Add(new PlayerBarElement());
                        int i = 0;
                        foreach (var pair in hpss)
                        {
                            (listBoxHps.Items[i] as PlayerBarElement).changeData(pair.Value.hps.perSecond / max * 100, pair.Value.name, pair.Value.hps.perSecond.ToString(), pair.Value.id == self.id);
                            i++;
                        }
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }




        internal void opPacketArrival(object sender, PacketArrivalEventArgs e)
        {
            switch((OpCode2904)e.packet.opCode)
            {
                case OpCode2904.S_LOGIN:
                    var s_login = (S_LOGIN)PacketCreator.create(e.packet);
                    self = new TeraPlayer(s_login.id, s_login.name);
                    party.Add(self.id, self);
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
                case OpCode2904.S_EACH_SKILL_RESULT:
                    {
                        var skill = (S_EACH_SKILL_RESULT)PacketCreator.create(e.packet);
                        #region ИгрокАтакует
                        {
                            ulong projectile;
                            if (!projectiles.TryGetValue(skill.idWho, out projectile)) projectile = 0;
                            TeraPlayer p;
                            if (projectile > 0)
                            {
                                if (!party.TryGetValue(projectile, out p)) p = null;
                            }
                            else
                            {
                                if (!party.TryGetValue(skill.idWho, out p)) p = null;
                            }
                            if(p!=null)
                            {
                                Logger.debug("Player Attack {0}", p.name);
                                p.makeSkill(skill.damage, skill.dType);
                                return;
                            }
                        }
                        #endregion ИгрокАтакует
                        #region ИгрокаАтакуют
                        {
                            ulong projectile;
                            if (!projectiles.TryGetValue(skill.idTarget, out projectile)) projectile = 0;
                            TeraPlayer p;
                            if (projectile > 0)
                            {
                                if (!party.TryGetValue(projectile, out p)) p = null;
                            }
                            else
                            {
                                if (!party.TryGetValue(skill.idTarget, out p)) p = null;
                            }
                            if (p != null)
                            {
                                p.takeSkill(skill.damage, skill.dType);
                                return;
                            }
                        }
                        #endregion ИгрокаАтакуют
                    }
                    break;
            }
            //TeraApi.OpCodes.
        }
    }
}
