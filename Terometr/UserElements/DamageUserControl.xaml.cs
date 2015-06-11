using Detrav.TeraApi;
using Detrav.TeraApi.Core;
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
        Dictionary<ulong, DamageEngine> db = new Dictionary<ulong, DamageEngine>();
        Dictionary<ulong, DamageEngine> dbGrp = new Dictionary<ulong, DamageEngine>();
        DamageEngine all = new DamageEngine(uint.MaxValue,"Всего");
        private TeraApi.Core.TeraPlayer self;
        Config config;
        //public Dictionary<ulong, DamageElement> players;

        public void skillTakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void skillMakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            if (e.damage == 0) return;
            if (e.type != 1) return;


            TeraEntity who = e.who;
            bool self = true;
             //Отсеиваем NPC и ищеи игрока
            while (who.parent != null)
            {
                if (who is TeraNpc) self = false;
                who = who.parent;
            }
            //Если главный не игрок то уходим
            if (!(who is TeraPlayer)) return;
            TeraPlayer player = who as TeraPlayer;
            
            TeraNpc npc = null;
            if (e.target is TeraNpc)
                npc = e.target as TeraNpc;
            ulong mId;
            mId = e.target.id;
            if (!db.ContainsKey(mId))
            {
                if (npc != null)
                    db[mId] = new DamageEngine(npc.npc.hp, npc.safeName);
                else
                    db[mId] = new DamageEngine(0, e.target.safeName);
            }
            db[mId].add(player, e.damage, e.time, self, e.crit);
            mId = e.target.id;
            if (npc != null)
                mId = npc.npc.ulongId;
            if(!dbGrp.ContainsKey(mId))
            {
                if (npc != null)
                    dbGrp[mId] = new DamageEngine(npc.npc.hp, npc.safeName);
                else
                    dbGrp[mId] = new DamageEngine(0, e.target.safeName);
            }
            dbGrp[mId].add(player, e.damage, e.time, self, e.crit);
            all.add(player, e.damage, e.time, self, e.crit);
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
            dbGrp.Clear();
            all.Clear();
            comboBoxReMake();
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

        public void comboBoxReMake()
        {
            //если пусто то добавляем всего строку
            if (comboBox.Items.Count == 0)
                comboBox.Items.Add(new ComboBoxHiddenItem(UInt64.MaxValue, "Суммарно"));
            var selectDb = db;
            if (toggleButtonGroup.IsChecked == true) selectDb = dbGrp;
            while (comboBox.Items.Count > selectDb.Count + 1) comboBox.Items.RemoveAt(0);
            while (comboBox.Items.Count < selectDb.Count + 1) comboBox.Items.Insert(comboBox.Items.Count - 1, new ComboBoxHiddenItem(0, null));

            int i = 0;
            foreach (var key in selectDb.Keys.ToArray())
            {
                var comboBoxItem = comboBox.Items[i] as ComboBoxHiddenItem;
                if (comboBoxItem != null)
                {
                    if (key != comboBoxItem.id)
                    {
                        comboBoxItem.id = key;
                        comboBoxItem.text = selectDb[key].name;
                    }
                }
                i++;
            }
            if (comboBox.SelectedItem == null) comboBox.SelectedIndex = comboBox.Items.Count - 1;
        }

        public void selectBam()
        {
            var selectDb = db;
            if (toggleButtonGroup.IsChecked == true) selectDb = dbGrp;
            if (toggleButtonBAM.IsChecked == true)
            {
                int i = 0;
                int max_i = -1;
                uint max = 0;
                foreach (var pair in selectDb)
                {
                    if (pair.Value.isActive)
                        if (pair.Value.npcHp > max)
                        {
                            max = pair.Value.npcHp;
                            max_i = i;
                        }
                    i++;
                }
                if (max_i < 0) max_i = i;
                comboBox.SelectedIndex = max_i;
            }
        }
    }
}