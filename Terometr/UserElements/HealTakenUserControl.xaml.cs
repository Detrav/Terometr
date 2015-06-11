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
    public partial class HealTakenUserControl : UserControl, IDpsUIEngine
    {
        public HealTakenUserControl()
        {
            InitializeComponent();
            clear();
        }
        Dictionary<ulong, DamageEngine> db = new Dictionary<ulong, DamageEngine>();
        DamageEngine all = new DamageEngine(uint.MaxValue, "Всего");
        private TeraApi.Core.TeraPlayer self;
        Config config;

        public void skillResult(TeraApi.Events.SkillResultEventArgs e)
        {
            throw new NotImplementedException();
        }
        /*public void addSkill(TeraSkill skill)
        {
            if (skill.skillType == SkillType.Make) return;
            if (skill.value == 0) return;
            if (skill.type != 2) return;
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

            var selectDb = db;

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


        public void skillTakeResult(TeraApi.Events.SkillResultEventArgs e)
        {
            if (e.damage == 0) return;
            if (e.type != 2) return;
            if (!(e.target is TeraPlayer)) return;
            ulong mId;
            mId = e.who.id;
            if (!db.ContainsKey(mId))
            {
                db[mId] = new DamageEngine(0, e.who.safeName);
                comboBoxReMake();
            }
            db[mId].add((e.target as TeraPlayer), e.damage, e.time, true, e.crit);
            all.add((e.target as TeraPlayer), e.damage, e.time, true, e.crit);
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
    }
}
