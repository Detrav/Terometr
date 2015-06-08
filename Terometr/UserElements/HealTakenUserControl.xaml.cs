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
    public partial class HealTakenUserControl : UserControl, IDpsUIEngine
    {
        public HealTakenUserControl()
        {
            InitializeComponent();
            clear();
        }
        internal Dictionary<ulong, DamageElement> all = new Dictionary<ulong, DamageElement>();
        private TeraApi.Core.TeraPlayer self;
        Config config;

        public void addSkill(TeraSkill skill)
        {
            if (skill.skillType == SkillType.Make) return;
            if (skill.value == 0) return;
            if (skill.type != 2) return;
            if (!all.ContainsKey(skill.player.id))
                all[skill.player.id] = new DamageElement(skill.player);
            all[skill.player.id].addValue(skill.value, skill.time);
        }

        public void doEvents()
        {
            if (checkBox.IsChecked == true)
                updateLayoutHps();
            else updateLayoutHeal();
        }

        public void updateLayoutHeal()
        {
            if(comboBox.SelectedItem==null) return;
            //ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            Dictionary<ulong, DamageElement> players = all;
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

        public void updateLayoutHps()
        {
            if (comboBox.SelectedItem == null) return;
            //ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            Dictionary<ulong, DamageElement> players = all;
            if (players != null)
            {
                SortedList<double, DamageElement> list = new SortedList<double, DamageElement>(new DuplicateKeyComparer<double>());
                double max = 0;
                double sum = 0;
                foreach (var pair in players)
                {
                    max = Math.Max(pair.Value.vps, max);
                    sum += pair.Value.vps;
                    list[pair.Value.vps] = pair.Value;
                }
                while (listBox.Items.Count > list.Count + 1) listBox.Items.RemoveAt(0);
                while (listBox.Items.Count < list.Count + 1) listBox.Items.Add(new PlayerBarElement());
                int i = 0;
                foreach (var pair in list)
                {
                    (listBox.Items[i] as PlayerBarElement).changeData(
                        pair.Key / max * 100,
                        pair.Value.player.name,
                        MetrEngine.generateRight(pair.Key, sum),
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
            //db.Clear();
            all.Clear();
            comboBox.Items.Clear();
            comboBox.Items.Add(new ComboBoxHiddenItem(UInt64.MaxValue, "Всего"));
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
