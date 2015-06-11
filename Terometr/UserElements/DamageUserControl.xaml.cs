using Detrav.TeraApi;
using Detrav.Terometr.Core;
using Detrav.Terometr.Core.Damage;
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
    /// Логика взаимодействия для DamageUserControl.xaml
    /// </summary>
    public partial class DamageUserControl : UserControl, IDpsUIEngine
    {
        public DamageUserControl()
        {
            InitializeComponent();
            clear();
        }
        //TeraPlayer self;
        //Карта по мобам и по игрокам;
        internal Dictionary<ulong, DamageEngine> db = new Dictionary<ulong, DamageEngine>();
        DamageEngine all = new DamageEngine(uint.MaxValue);
        private TeraApi.Core.TeraPlayer self;
        Config config;
        //public Dictionary<ulong, DamageElement> players;

        public void skillResult(TeraApi.Events.SkillResultEventArgs e)
        {
            throw new NotImplementedException();
        }
       /* public void addSkill(TeraSkill skill)
        {
            if (skill.skillType == SkillType.Take) return;
            if (skill.value == 0) return;
            if (skill.type != 1) return;
            //ulong mId 
            /*if (skill.npc != null)
            {
                if (!db.ContainsKey(skill.npc.npc.ulongId))
                {
                    Logger.debug("new Npc {0}", skill.npc.npc.name);
                    db[skill.npc.npc.ulongId] = new Dictionary<ulong, DamageElement>();
                    comboBox.Items.Insert(comboBox.Items.Count - 1, new ComboBoxHiddenItem(skill.npc.npc.ulongId, skill.npc.npc.name));
                }
                var players = db[skill.npc.npc.ulongId];
                if (!players.ContainsKey(skill.player.id))
                    players[skill.player.id] = new DamageElement(skill.player);
                players[skill.player.id].addValue(skill.value, skill.time);
            }
            if (!all.ContainsKey(skill.player.id))
                all[skill.player.id] = new DamageElement(skill.player);*
            all.add(skill);
        }*/


        public void doEvents()
        {
            if (comboBox.SelectedItem == null) return;
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            double max;
            double sum;
            SortedList<double, DamageKeyValue> list = new SortedList<double, DamageKeyValue>(new DuplicateKeyComparer<double>());
            DamageEngine players;
            if (id < UInt64.MaxValue) db.TryGetValue(id, out players);
            else players = all;

            if (players != null)
            {
                if (toggleButtonDps.IsChecked == true)
                    players.getListDps(list, out max, out sum);
                else players.getList(list, out max, out sum);


                while (listBox.Items.Count > list.Count + 1) listBox.Items.RemoveAt(0);
                while (listBox.Items.Count < list.Count + 1) listBox.Items.Add(new PlayerBarElement());
                int i = 0;
                foreach (var pair in list)
                {
                    (listBox.Items[i] as PlayerBarElement).changeData(
                        pair.Value.value / max * 100,
                        pair.Value.name,
                        MetrEngine.generateRight(pair.Value.value, sum),
                        (self.id == pair.Value.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.playerClass);
                    i++;
                }
                (listBox.Items[i] as PlayerBarElement).changeData(
                        100,
                        "Всего",
                        MetrEngine.generateRight(sum, sum),
                        PlayerBarElement.clr.sum, Detrav.TeraApi.Enums.PlayerClass.Empty);
            }
        }

        public void setSelf(TeraApi.Core.TeraPlayer self)
        {
            this.self = self;
        }

        public void clear()
        {
            db.Clear();
            all.Clear();
            comboBox.Items.Clear();
            comboBox.Items.Add(new ComboBoxHiddenItem(UInt64.MaxValue, "Суммарно"));
            comboBox.SelectedIndex = 0;
            Logger.debug("clear, and add all row");
        }

        public string generateTable()
        {
            throw new NotImplementedException();
        }


        public TeraApi.Interfaces.ITeraClient teraClient { get; set; }


        public void reSetting(Config config)
        {
            this.config = config;
        }
    }
}
