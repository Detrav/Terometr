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
    public partial class DamageTakenUserControl : UserControl, IDpsUIEngine
    {
        public DamageTakenUserControl()
        {
            InitializeComponent();
            clear();
        }
        Dictionary<ulong, DamageEngine> db = new Dictionary<ulong, DamageEngine>();
        Dictionary<ulong, DamageEngine> dbGrp = new Dictionary<ulong, DamageEngine>();
        DamageEngine all = new DamageEngine(uint.MaxValue, "Всего");
        private TeraApi.Core.TeraPlayer self;
        Config config;

       

        public void doEvents()
        {
            if (comboBox.SelectedItem == null) return;
            selectBam();
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;
            double max;
            double sum;
            SortedList<double, DamageKeyValue> list = new SortedList<double, DamageKeyValue>(new DuplicateKeyComparer<double>());
            DamageEngine players;

            var selectDb = db;
            if (toggleButtonGroup.IsChecked == true) selectDb = dbGrp;

            if (id < UInt64.MaxValue) selectDb.TryGetValue(id, out players);
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
                        (toggleButtonCrit.IsChecked != true ? pair.Value.name : String.Format("{0}-{1}%", pair.Value.name, (int)(pair.Value.critRate * 100))),
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


        public TeraApi.Interfaces.ITeraClient teraClient { get; set; }


        public void reSetting(Config config)
        {
            this.config = config;
        }


        public void skillTakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            if (e.damage == 0) return;
            if (e.type != 1) return;
            if(!(e.target is TeraPlayer)) return;
            TeraPlayer player = e.target as TeraPlayer;
            bool self = true;
            
            TeraNpc npc = null;
            if (e.who is TeraNpc)
                npc = e.who as TeraNpc;
            ulong mId;
            mId = e.who.id;
            if (!db.ContainsKey(mId))
            {
                if (npc != null)
                    db[mId] = new DamageEngine(npc.npc.hp, npc.safeName);
                else
                    db[mId] = new DamageEngine(0, e.who.safeName);
                comboBoxReMake();
            }
            db[mId].add(player, e.damage, e.time, self, e.crit);
            mId = e.target.id;
            if (npc != null)
                mId = npc.npc.ulongId;
            if (!dbGrp.ContainsKey(mId))
            {
                if (npc != null)
                    dbGrp[mId] = new DamageEngine(npc.npc.hp, npc.safeName);
                else
                    dbGrp[mId] = new DamageEngine(0, e.who.safeName);
                comboBoxReMake();
            }
            dbGrp[mId].add(player, e.damage, e.time, self, e.crit);
            all.add(player, e.damage, e.time, self, e.crit);
        }

        public void skillMakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            //throw new NotImplementedException();
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
            comboBox.UpdateLayout();
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

        private void toggleButtonGroup_Click(object sender, RoutedEventArgs e)
        {
            comboBoxReMake();
        }
    }
}
