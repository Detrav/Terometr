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
    public partial class DamageEngineUserControl : UserControl, IDpsUIEngine
    {
        internal DamageEngineUserControl(IDpsUIEngineType myType,string button,string left)
        {
            InitializeComponent();
            clear();
            this.myType = myType;
            this.toggleButtonDps.Content = null;
            this.toggleButtonDps.Content = button;
            this.labelText.Content = null;
            this.labelText.Content = left;
        }
        Dictionary<ulong, DamageEngine> db = new Dictionary<ulong, DamageEngine>();
        private TeraApi.Core.TeraPlayer self;
        Config config;
        bool needToUpdate = true;
        private IDpsUIEngineType myType;


        public void doEvents()
        {
            if (config.autoTarget) autoTarget();
            if (comboBox.SelectedItem == null) return;
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            //Проверяем нужно ли чекать группу и создаём флаг активности
            
            DamageEngine selectDb = null;
            if (db.TryGetValue(id, out selectDb)) return;

            //Дальше чекаем нужно ли обновить данные
            if (!(selectDb.isActive || needToUpdate)) return;

            double max = 0;
            double sum = 0;
            SortedList<double, DamageKeyValue> list = new SortedList<double, DamageKeyValue>(new DuplicateKeyComparer<double>());
            DamageKeyValue[] temp;
            if (toggleButtonDps.IsChecked == true) temp = selectDb.getListDps(out max, out sum);
            else temp = selectDb.getList(out max, out sum);
            foreach (var el in temp) list.Add(el.value, el);



            while (listBox.Items.Count > list.Count + 1) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Count + 1) listBox.Items.Add(new PlayerBarElement());
            int i = 0;
            foreach (var pair in list)
            {
                (listBox.Items[i] as PlayerBarElement).changeData(
                    pair.Value.value / max * 100,
                    String.Format("{0}-{1}%", pair.Value.name, (int)(pair.Value.critRate * 100)),
                    MetrEngine.generateRight(pair.Value.value, sum),
                    (self.id == pair.Value.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), pair.Value.playerClass);
                i++;
            }
            (listBox.Items[i] as PlayerBarElement).changeData(
                    100,
                    "Всего",
                    MetrEngine.generateRight(sum, sum),
                    PlayerBarElement.clr.sum, Detrav.TeraApi.Enums.PlayerClass.Empty);
            needToUpdate = false;
        }


        

        public void setSelf(TeraApi.Core.TeraPlayer self)
        {
            this.self = self;
            needToUpdate = true;
        }

        public void clear()
        {
            db.Clear();
            comboBoxReMake();
            Logger.debug("clear, and add all row");
        }

        public string generateTable()
        {
            if (comboBox.SelectedItem == null) return null;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", comboBox.SelectedItem, Environment.NewLine);
            foreach (var el in listBox.Items)
            {
                if (el is PlayerBarElement)
                    sb.AppendFormat("{0}{1}", (el as PlayerBarElement).getText(), Environment.NewLine);
            }
            return sb.ToString();
        }


        public void reSetting(Config config)
        {
            this.config = config;
            needToUpdate = true;
            //comboBox.Items.Clear();
            comboBoxReMake();
        }


        public void skillTakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            bool self = true;
            TeraEntity who = e.who;
            while (who.parent != null)
            {
                if (who is TeraNpc)
                    self = false;
                who = who.parent;
            }
            TeraNpc npc = null;
            if (who is TeraNpc) npc = who as TeraNpc;
            ulong mId = who.id;
            //Обычная база
            if (!db.ContainsKey(mId))
            {
                if (npc != null) db[mId] = new DamageEngine(mId,npc.npc.hp, npc.safeName,false);
                else db[mId] = new DamageEngine(mId,0, who.safeName,null);
                comboBoxReMake();
            }
            db[mId].add(e.target, e.damage, e.time, self, e.crit);
            //Групповая база

            if (npc != null)
            {
                mId = npc.npc.ulongId;
                if (!db.ContainsKey(mId))
                {
                    db[mId] = new DamageEngine(mId,npc.npc.hp, npc.safeName,true);

                    comboBoxReMake();
                }
                db[mId].add(e.target, e.damage, e.time, self, e.crit);
            }
            //суммарно
            if (!db.ContainsKey(ulong.MaxValue))
                db[ulong.MaxValue] = new DamageEngine(ulong.MaxValue,0, "Суммарно",null);
            db[ulong.MaxValue].add(e.target, e.damage, e.time, self, e.crit);
        }

        public void skillMakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            bool self = true;
            TeraEntity who = e.who;
            while (who.parent != null)
            {
                if (who is TeraNpc)
                    self = false;
                who = who.parent;
            }
            TeraNpc npc = null;
            if (e.target is TeraNpc) npc = e.target as TeraNpc;
            ulong mId = e.target.id;
            //Обычная база
            if (!db.ContainsKey(mId))
            {
                if (npc != null) db[mId] = new DamageEngine(mId, npc.npc.hp, npc.safeName, false);
                else db[mId] = new DamageEngine(mId, 0, e.target.safeName, null);
                comboBoxReMake();
            }
            db[mId].add(who, e.damage, e.time, self, e.crit);
            //Групповая база

            if (npc != null)
            {
                mId = npc.npc.ulongId;
                if (!db.ContainsKey(mId))
                {
                    db[mId] = new DamageEngine(mId, npc.npc.hp, npc.safeName, true);

                    comboBoxReMake();
                }
                db[mId].add(who, e.damage, e.time, self, e.crit);
            }
            //суммарно
            if (!db.ContainsKey(ulong.MaxValue))
                db[ulong.MaxValue] = new DamageEngine(ulong.MaxValue, 0, "Суммарно", null);
            db[ulong.MaxValue].add(e.who, e.damage, e.time, self, e.crit);
        }

        public void comboBoxReMake()
        {
            SortedList<ulong,ComboBoxHiddenItem> items = new SortedList<ulong,ComboBoxHiddenItem>();
            ComboBoxHiddenItem selectedItem = (ComboBoxHiddenItem)comboBox.SelectedItem;
            comboBox.ItemsSource = null;
            foreach(var pair in db)
            {
                if (pair.Value.group == config.group)
                    items.Add(pair.Key,pair.Value.cbhi);
                if(pair.Value.group == null)
                    items.Add(pair.Key,pair.Value.cbhi);
            }
            comboBox.ItemsSource = items.Values;
            comboBox.SelectedItem = selectedItem;
            comboBox.UpdateLayout();
        }

        public void autoTarget()
        {
            int i = 0;
            int max_i = -1;
            uint max = 0;
            foreach (ComboBoxHiddenItem el in comboBox.Items)
            {
                if (db[el.id].isActive)
                    if (db[el.id].npcHp > max)
                    {
                        max = db[el.id].npcHp;
                        max_i = i;
                    }
                i++;
            }
            if (max_i < 0) max_i = i;
            comboBox.SelectedIndex = max_i;
        }

        private void toggleButtonDps_Click(object sender, RoutedEventArgs e)
        {
            needToUpdate = true;
        }


        public void skillResult(TeraApi.Events.SkillResultEventArgs e)
        {
            if (e.damage == 0) return;
            switch(myType)
            {
                case IDpsUIEngineType.damage:
                    if (e.type == 1) skillMakeResult(e);
                    break;
                case IDpsUIEngineType.heal:
                    if (e.type == 2) skillMakeResult(e);
                    break;
                case IDpsUIEngineType.damageTaken:
                    if (e.type == 1) skillTakeResult(e);
                    break;
                case IDpsUIEngineType.healTaken:
                    if (e.type == 2) skillTakeResult(e);
                    break;
            }
        }


    }
}
