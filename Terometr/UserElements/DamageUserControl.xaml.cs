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
        }
        //TeraPlayer self;
        //Карта по мобам и по игрокам;
        public Dictionary<ulong, Dictionary<ulong, DamageElement>> db = new Dictionary<ulong,Dictionary<ulong,DamageElement>>();
        public Dictionary<ulong, DamageElement> all = new Dictionary<ulong,DamageElement>();
        private TeraApi.Core.TeraPlayer self;
        //public Dictionary<ulong, DamageElement> players;

        public void addSkill(TeraSkill skill)
        {
            if (skill.skillType == SkillType.Take) return;
            if (skill.value == 0) return;
            if (skill.type != 1) return;
            if (!db.ContainsKey(skill.npc.ulongId))
                db[skill.npc.ulongId] = new Dictionary<ulong, DamageElement>();
            var players = db[skill.npc.ulongId];
            if (!players.ContainsKey(skill.player.id))
                players[skill.player.id] = new DamageElement(skill.player);
            players[skill.player.id].addValue(skill.value,skill.time);
        }

        public void updateData(TeraSkill[] history)
        {
            clear();
            foreach(var el in history)
            {
                addSkill(el);
            }
        }

        public void doEvents()
        {
            updateLayout();
        }

        public void updateLayout()
        {
            ulong id = Convert.ToUInt64(comboBox.SelectedItem);
            Dictionary<ulong,DamageElement> players;
            if (db.TryGetValue(id, out players))
            {
                /*SortedList<double,DamageElement> list = new SortedList<double,DamageElement>(new DuplicateKeyComparer<double>());
                while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
                while (listBox.Items.Count < list.Count) listBox.Items.Add(new PlayerBarElement());
                int i = 0;
                foreach (var pair in list)
                {
                    (listBox.Items[i] as PlayerBarElement).changeData(
                        pair.Value.right / max * 100,
                        pair.Value.left,
                        MetrEngine.generateRight(pair.Value.right, sum),
                        pair.Value.self, pair.Value.playerClass);
                    i++;
                }*/
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
        }

        public string generateTable()
        {
            throw new NotImplementedException();
        }


        public TeraApi.Interfaces.ITeraClient teraClient { get; set; }
    }
}
