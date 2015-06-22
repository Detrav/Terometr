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
        internal DamageEngineUserControl(IDpsUIEngineType myType,string left)
        {
            InitializeComponent();
            clear();
            this.myType = myType;
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
            if (comboBox.SelectedItem == null) { dataDamageGrid.clear(); return; }
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            //Проверяем нужно ли чекать группу и создаём флаг активности
            
            DamageEngine selectDb = null;
            if (!db.TryGetValue(id, out selectDb)) return;

            //Дальше чекаем нужно ли обновить данные
            //если активен то обновляем
            //если нужно обновить то обновляем
            //если оба равны нулю то необновляем
            if (!selectDb.isActive && !needToUpdate) return;

            double max = 0;
            double sum = 0;
            double maxDps = 0;
            double sumDps = 0;
            SortedList<double, DamageKeyValue> list = new SortedList<double, DamageKeyValue>(new DuplicateKeyComparer<double>());
            DamageKeyValue[] temp;
            temp = selectDb.getList(out max, out sum,out maxDps,out sumDps);
            max = 0;
            sum = 0;
            foreach (var el in temp)
            {
                switch(el.type)
                {
                    case DamagePlayerType.party:
                        max = Math.Max(el.value,max);
                        sum += el.value;
                        list.Add(el.value, el);
                        break;
                    case DamagePlayerType.player:
                        if (!config.party)
                        {
                            max = Math.Max(el.value, max);
                            sum += el.value;
                            list.Add(el.value, el);
                        }
                        break;
                    case DamagePlayerType.group:
                        if(!config.player)
                            if (config.group)
                            {
                                max = Math.Max(el.value, max);
                                sum += el.value;
                                list.Add(el.value, el);
                            }
                        break;
                    case DamagePlayerType.npc:
                        if(!config.player)
                            if (!config.group)
                            {
                                max = Math.Max(el.value, max);
                                sum += el.value;
                                list.Add(el.value, el);
                            }
                        break;
                }
            }
            if (list.Count == 0)
            {
                dataDamageGrid.clear();
            }
            else
            {
                foreach (var pair in list)
                {
                    dataDamageGrid.updateData(pair.Value.id, pair.Value.playerClass, pair.Value.name, pair.Value.critRate, pair.Value.value, pair.Value.inSec);
                }
                dataDamageGrid.updateSum(sum, max, sumDps, maxDps);
                dataDamageGrid.updateLayout();
            }
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
            return dataDamageGrid.generateTable();
        }


        public void reSetting(Config config)
        {
            this.config = config;
            needToUpdate = true;
            //comboBox.Items.Clear();
            comboBoxReMake();
            needToUpdate = true;
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
            {
                db[ulong.MaxValue] = new DamageEngine(ulong.MaxValue, 0, "Суммарно", null);
                comboBoxReMake();
            }
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
            db[mId].add(who, e.damage, e.time, self, e.crit);//Добавляем нормальный тип, если who это игрок то игрока, если npc то npc
            //Групповая база

            if (npc != null)
            {
                mId = npc.npc.ulongId;
                if (!db.ContainsKey(mId))
                {
                    db[mId] = new DamageEngine(mId, npc.npc.hp, npc.safeName, true);

                    comboBoxReMake();
                }
                db[mId].add(who, e.damage, e.time, self, e.crit);//Добавляем груповой тип
            }
            //суммарно
            if (!db.ContainsKey(ulong.MaxValue))
            {
                db[ulong.MaxValue] = new DamageEngine(ulong.MaxValue, 0, "Суммарно", null);
                comboBoxReMake();
            }
            db[ulong.MaxValue].add(who, e.damage, e.time, self, e.crit);//Добавляем 
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
            needToUpdate = true;
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
            if (max_i < 0) max_i = i-1;
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

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            needToUpdate = true;
        }
    }
}
