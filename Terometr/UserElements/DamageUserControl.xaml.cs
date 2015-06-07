using Detrav.TeraApi;
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

namespace Detrav.Terometr.UserElements
{
    /// <summary>
    /// Логика взаимодействия для DamageUserControl.xaml
    /// </summary>
    public partial class DamageUserControl : UserControl, IDpsEngine
    {
        public DamageUserControl()
        {
            InitializeComponent();
            clear();
        }
        //TeraPlayer self;
        //Карта по мобам и по игрокам;
        internal Dictionary<ulong, Dictionary<ulong, DamageElement>> db = new Dictionary<ulong,Dictionary<ulong,DamageElement>>();
        internal Dictionary<ulong, DamageElement> all = new Dictionary<ulong, DamageElement>();
        private TeraApi.Core.TeraPlayer self;
        //public Dictionary<ulong, DamageElement> players;

        public void addSkill(TeraSkill skill)
        {
            if (skill.skillType == SkillType.Take) return;
            if (skill.value == 0) return;
            if (skill.type != 1) return;
            if (skill.npc != null)
            {
                if (!db.ContainsKey(skill.npc.ulongId))
                {
                    Logger.debug("new Npc {0}", skill.npc.name);
                    db[skill.npc.ulongId] = new Dictionary<ulong, DamageElement>();
                    comboBox.Items.Insert(comboBox.Items.Count - 1, new ComboBoxHiddenItem(skill.npc.ulongId, skill.npc.name));
                }
                var players = db[skill.npc.ulongId];
                if (!players.ContainsKey(skill.player.id))
                    players[skill.player.id] = new DamageElement(skill.player);
                players[skill.player.id].addValue(skill.value, skill.time);
            }
            if (!all.ContainsKey(skill.player.id))
                all[skill.player.id] = new DamageElement(skill.player);
            all[skill.player.id].addValue(skill.value, skill.time);
        }

        public void updateData(TeraSkill[] history)
        {
            /*clear();
            foreach(var el in history)
            {
                addSkill(el);
            }*/
        }

        public void doEvents()
        {
            updateLayout();
        }

        public void updateLayout()
        {
            if(comboBox.SelectedItem==null) return;
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            Dictionary<ulong, DamageElement> players = null;
            if (id < UInt64.MaxValue) db.TryGetValue(id, out players);
            else players = all;
            if (players!=null)
            {
                SortedList<double,DamageElement> list = new SortedList<double,DamageElement>(new DuplicateKeyComparer<double>());
                double max = 0;
                double sum = 0;
                foreach (var pair in players)
                {
                    max = Math.Max(pair.Value.value, max);
                    sum += pair.Value.value;
                    list[pair.Value.value] = pair.Value;
                }
                while (listBox.Items.Count > list.Count+1) listBox.Items.RemoveAt(0);
                while (listBox.Items.Count < list.Count+1) listBox.Items.Add(new PlayerBarElement());
                int i = 0;
                foreach (var pair in list)
                {
                    (listBox.Items[i] as PlayerBarElement).changeData(
                        pair.Value.value / max * 100,
                        pair.Value.player.name,
                        MetrEngine.generateRight(pair.Value.value, sum),
                        (self.id == pair.Value.player.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.player.playerClass);
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
            comboBox.Items.Add(new ComboBoxHiddenItem(UInt64.MaxValue, "Всего"));
            Logger.debug("clear, and add all row");
        }

        public string generateTable()
        {
            throw new NotImplementedException();
        }


        public TeraApi.Interfaces.ITeraClient teraClient { get; set; }
    }
}
