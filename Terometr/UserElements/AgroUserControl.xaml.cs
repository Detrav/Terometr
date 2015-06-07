using Detrav.TeraApi;
using Detrav.TeraApi.Core;
using Detrav.Terometr.Core;
using Detrav.Terometr.Core.Agro;
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
    public partial class AgroUserControl : UserControl, IDpsEngine
    {
        public AgroUserControl()
        {
            InitializeComponent();
            clear();
        }
        internal Dictionary<ulong, AgroEngine> db = new Dictionary<ulong,AgroEngine>();
        AgroEngine all = new AgroEngine();
        Dictionary<ulong, double> agro = new Dictionary<ulong,double>();
        private TeraApi.Core.TeraPlayer self;
        Config config;

        public void addSkill(TeraSkill skill)
        {
            if (skill.skillType == SkillType.Take) return;
            if (skill.value == 0) return;
            if (skill.type == 2)
            {
                foreach (var pair in db)
                    pair.Value.add(skill);
                all.add(skill);
                return;
            }
            if (skill.type == 1)
            {
                if (skill.npc != null)
                {
                    AgroEngine eng;
                    if (db.ContainsKey(skill.npc.npc.ulongId))
                    {
                        eng = db[skill.npc.npc.ulongId];
                        if (eng.npc != skill.npc.id)
                            eng.multi = true;
                    }
                    else
                    {
                        db[skill.npc.npc.ulongId] = new AgroEngine();
                        eng = db[skill.npc.npc.ulongId];
                        eng.npc = skill.npc.id;
                        eng.npcHp = skill.npc.npc.hp;
                        eng.multi = false;
                        comboBox.Items.Insert(comboBox.Items.Count - 1, new ComboBoxHiddenItem(skill.npc.npc.ulongId, skill.npc.npc.name));
                    }
                    if (!eng.multi && eng.lastTarget != 0 && eng.lastTarget != skill.npc.target)
                    {
                        if (!agro.ContainsKey(eng.lastTarget))
                        {
                            if (teraClient.getPlayerById(eng.lastTarget).playerClass == TeraApi.Enums.PlayerClass.Lancer)
                                agro[eng.lastTarget] = 3.8;
                            else agro[eng.lastTarget] = 1;
                        }
                        if (!agro.ContainsKey(skill.npc.target)) 
                        {
                            if (teraClient.getPlayerById(skill.npc.target).playerClass == TeraApi.Enums.PlayerClass.Lancer)
                                agro[skill.npc.target] = 3.8;
                            else
                                agro[skill.npc.target] = 1;
                        }
                        const double maxAgro = 10;
                        double lastPlayer = eng.getValue(eng.lastTarget);
                        double newPlayer = eng.getValue(skill.npc.target);
                        if (lastPlayer > newPlayer)
                        {
                            double temp = lastPlayer / newPlayer;
                            if (temp < maxAgro)
                                agro[skill.npc.target] = temp - (temp - agro[skill.npc.target]) * 0.314;
                            Logger.info("Agro {0} {1}", skill.npc.target, agro[skill.npc.target]);
                        }
                        else
                        {
                            double temp = newPlayer / lastPlayer;
                            if (temp < maxAgro)
                                agro[eng.lastTarget] = temp - (temp - agro[eng.lastTarget]) * 0.314;
                            Logger.info("Agro {0} {1}", eng.lastTarget, agro[eng.lastTarget]);
                        }
                        //Переагрили и нужно вычислить показатель агра для 2 игроков, 
                        //lastTarget и target
                    }
                    eng.lastTarget = skill.npc.target;
                    eng.add(skill);
                }
                //if (skill.value == 0) return;
                //if (skill.type != 1) return;
                all.add(skill);
            }
        }

        public void selectBam()
        {
            if (checkBox.IsChecked == true)
            {
                int i = 0;
                int max_i = -1;
                uint max = 0;
                foreach (var pair in db)
                {
                    if (!pair.Value.multi)
                    {
                        if (pair.Value.npcHp > max)
                        {
                            max = pair.Value.npcHp;
                            max_i = i;
                        }
                    }
                    i++;
                }
                if (max_i < 0) max_i = i;
                comboBox.SelectedIndex = max_i;
            }
        }

        public void doEvents()
        {
            selectBam();
            if (comboBox.SelectedItem == null) return;
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            AgroEngine eng = null;
            if (id < UInt64.MaxValue)
                if (!db.TryGetValue(id, out eng)) eng = null;
            if (eng == null) eng = all;
            SortedList<double, TeraPlayer> list = new SortedList<double, TeraPlayer>(new DuplicateKeyComparer<double>());
            double max = 0;
            double sum = 0;
            eng.getSortedList(agro, list, out sum, out max);
            while (listBox.Items.Count > list.Count + 1) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count + 1) listBox.Items.Add(new PlayerBarElement());
            int i = 0;
            foreach (var pair in list)
            {
                (listBox.Items[i] as PlayerBarElement).changeData(
                    pair.Key / max * 100,
                    pair.Value.name,
                    MetrEngine.generateRight(pair.Key, sum),
                    (self.id == pair.Value.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.playerClass);
                i++;
            }
            (listBox.Items[i] as PlayerBarElement).changeData(
                    100,
                    "Всего",
                    MetrEngine.generateRight(sum, sum),
                    PlayerBarElement.clr.sum, Detrav.TeraApi.Enums.PlayerClass.Empty);

            if(sum == 0)
            {
                if(comboBox.SelectedIndex<comboBox.Items.Count-1 && comboBox.SelectedIndex>=0)
                {
                    if (db.ContainsKey(id)) db.Remove(id);
                    comboBox.Items.RemoveAt(comboBox.SelectedIndex);
                }
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
