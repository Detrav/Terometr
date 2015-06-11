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
    public partial class AgroUserControl : UserControl, IDpsUIEngine
    {
        public AgroUserControl()
        {
            InitializeComponent();
            clear();
        }
        internal Dictionary<ulong, AgroEngine> db = new Dictionary<ulong, AgroEngine>();
        internal Dictionary<ulong, AgroEngine> dbGrp = new Dictionary<ulong, AgroEngine>();
        AgroEngine all = new AgroEngine(ulong.MaxValue, "Всего", uint.MaxValue, false);
        //Dictionary<ulong, double> agro = new Dictionary<ulong,double>();
        private TeraApi.Core.TeraPlayer self;
        Config config;

        public void skillResult(TeraApi.Events.SkillResultEventArgs e)
        {
            TeraEntity who = e.who;
             //Отсеиваем NPC и ищеи игрока
            while (who.parent != null)
            {
                if (who is TeraNpc) return;
                who = who.parent;
            }
            //Если главный не игрок то уходим
            if (!(who is TeraPlayer)) return;
            //Теперь у нас атакует точно игрок, значит отсеиваем лишнее
            //Убираем урон в 0, у меня нет описание скилов поэтому нафиг он не нужен
            if (e.damage == 0) return;
            //если хил то добавляем агр как хилу
            if (e.type == 2)
            {
                foreach (var pair in db)
                    pair.Value.addHeal(who as TeraPlayer,e.damage,e.time);
                all.addHeal(who as TeraPlayer,e.damage, e.time);
                return;
            }
            if (e.type == 1)
            {
                //Убираем если таргет не NPC
                if (!(e.target is TeraNpc)) return;
                TeraNpc npc = e.target as TeraNpc;
                AgroEngine eng;
                ulong mId;
                #region GeneralDB
                mId = npc.id;
                if (db.ContainsKey(mId))
                {
                    eng = db[mId];
                }
                else
                {
                    db[mId] = new AgroEngine(mId,npc.safeName, npc.npc.hp, false);
                    eng = db[mId];
                    comboBoxReMake();
                }
                eng.lastTarget = npc.id;
                eng.add(who as TeraPlayer, e.damage, e.time);
                #endregion GeneralDB
                #region GroupedDB
                mId = npc.npc.ulongId;
                if (db.ContainsKey(mId))
                {
                    eng = db[mId];
                    if (eng.lastTarget != npc.id)
                        eng.multi = true;
                }
                else
                {
                    db[mId] = new AgroEngine(mId, npc.safeName, npc.npc.hp, false);
                    eng = db[mId];
                    comboBoxReMake();
                }
                eng.lastTarget = npc.id;
                eng.add(who as TeraPlayer, e.damage, e.time);
                #endregion GroupedDB
                all.add(who as TeraPlayer, e.damage, e.time);
            }
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
                    if (!pair.Value.multi)
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

        public void doEvents()
        {
            comboBoxReMake();
            selectBam();
            if (comboBox.SelectedItem == null) return;
            ulong id = (comboBox.SelectedItem as ComboBoxHiddenItem).id;

            var selectDb = db;
            if (toggleButtonGroup.IsChecked == true) selectDb = dbGrp;

            AgroEngine eng = null;
            if (id < UInt64.MaxValue)
                if (!selectDb.TryGetValue(id, out eng)) eng = null;
            if (eng == null) eng = all;
            //SortedList<double, TeraPlayer> list = new SortedList<double, TeraPlayer>(new DuplicateKeyComparer<double>());
            //List<AgroKeyValue> list = new List<AgroKeyValue>();
            double max = 0;
            double sum = 0;
            //eng.getSortedList(agro, list, out sum, out max);
            var list = eng.getList( out sum, out max);
            while (listBox.Items.Count > list.Length + 1) listBox.Items.RemoveAt(0);
            while (listBox.Items.Count < list.Length + 1) listBox.Items.Add(new PlayerBarElement());
            int i = 0;
            foreach (var el in list)
            {
                (listBox.Items[i] as PlayerBarElement).changeData(
                    el.value / max * 100,
                    el.name,
                    MetrEngine.generateRight(el.value, sum),
                    (self.id == el.id ? PlayerBarElement.clr.me : PlayerBarElement.clr.other), el.playerClass);
                i++;
            }
            (listBox.Items[i] as PlayerBarElement).changeData(
                    100,
                    "Всего",
                    MetrEngine.generateRight(sum, sum),
                    PlayerBarElement.clr.sum, Detrav.TeraApi.Enums.PlayerClass.Empty);
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
                comboBox.Items.Add(new ComboBoxHiddenItem(UInt64.MaxValue, "Всего"));
            var selectDb = db;
            if (toggleButtonGroup.IsChecked == true) selectDb = dbGrp;
            while (comboBox.Items.Count > selectDb.Count + 1) comboBox.Items.RemoveAt(0);
            while (comboBox.Items.Count < selectDb.Count + 1) comboBox.Items.Insert(comboBox.Items.Count - 1, new ComboBoxHiddenItem(0, null));

            int i = 0;
            foreach(var key in selectDb.Keys.ToArray())
            {
                if (!selectDb[key].isFullActive)
                {
                    selectDb.Remove(key);
                    comboBox.Items.RemoveAt(i);
                    continue;
                }
                var comboBoxItem = comboBox.Items[i] as ComboBoxHiddenItem;
                if(comboBoxItem!=null)
                {
                    if(key!=comboBoxItem.id)
                    {
                        comboBoxItem.id = key;
                        comboBoxItem.text = selectDb[key].name;
                    }
                }
                i++;
            }
            if(comboBox.SelectedItem==null) comboBox.SelectedIndex = comboBox.Items.Count-1;
        }
    }
}
