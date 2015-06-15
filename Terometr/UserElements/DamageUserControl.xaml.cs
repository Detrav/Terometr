﻿using Detrav.TeraApi;
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
        Dictionary<ulong, DamageEngine> db = new Dictionary<ulong, DamageEngine>();
        Dictionary<ulong, DamageEngine> dbGrp = new Dictionary<ulong, DamageEngine>();
        DamageEngine all = new DamageEngine(uint.MaxValue, "Всего");
        private TeraApi.Core.TeraPlayer self;
        Config config;
        bool needToUpdate = true;

        public void skillTakeResult(TeraApi.Events.SkillResultEventArgs e)
        {

        }

        public void skillMakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            if (e.damage == 0) return;
            if (e.type != 1) return;


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
                if (npc != null) db[mId] = new DamageEngine(npc.npc.hp, npc.safeName);
                else db[mId] = new DamageEngine(0, e.target.safeName);
                comboBoxReMake();
            }
            db[mId].add(who, e.damage, e.time, self, e.crit);
            //Групповая база
            if (npc != null) mId = npc.npc.ulongId;
            if (!dbGrp.ContainsKey(mId))
            {
                if (npc != null) db[mId] = new DamageEngine(npc.npc.hp, npc.safeName);
                else db[mId] = new DamageEngine(0, e.target.safeName);
                comboBoxReMake();
            }
            dbGrp[mId].add(who, e.damage, e.time, self, e.crit);
            all.add(who, e.damage, e.time, self, e.crit);
        }

        public void doEvents()
        {
            if (comboBox.SelectedItem == null) return;
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            //Проверяем нужно ли чекать группу и создаём флаг активности
            var selectDb = all;
            if (id != ulong.MaxValue)
            {
                if (config.group) dbGrp.TryGetValue(id, out selectDb);
                else db.TryGetValue(id, out selectDb);
            }

            //Дальше чекаем нужно ли обновить данные
            if (!(selectDb.isActive || needToUpdate)) return;
            //если нужно обновиться, то опять разбиваем всё на группы:
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
            dbGrp.Clear();
            all.Clear();
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
        }

        public void comboBoxReMake()
        {

        }

        public void autoTarget()
        {

        }

        private void toggleButtonDps_Click(object sender, RoutedEventArgs e)
        {
            needToUpdate = true;
        }


    }
}