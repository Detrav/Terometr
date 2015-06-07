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

        public void addSkill(TeraSkill skill)
        {
            throw new NotImplementedException();
        }

        public void updateData(TeraSkill[] history)
        {
            throw new NotImplementedException();
        }

        public void doEvents()
        {
            updateLayout();
        }

        public void updateLayout()
        {
            /*while (listBox.Items.Count > list.Count) listBox.Items.RemoveAt(0);
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

        public void setSelf(TeraApi.Core.TeraPlayer self)
        {
            throw new NotImplementedException();
        }

        public void clear()
        {
            throw new NotImplementedException();
        }

        public string generateTable()
        {
            throw new NotImplementedException();
        }


        public TeraApi.Interfaces.ITeraClient teraClient
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
