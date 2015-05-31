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
    /// Логика взаимодействия для PlayerBarElement.xaml
    /// </summary>
    public partial class PlayerBarElement : UserControl
    {
        public PlayerBarElement()
        {
            InitializeComponent();
            green = (Brush)br.ConvertFrom("#FF10AE00");
            blue = (Brush)br.ConvertFrom("#FF1000AE");
            black = (Brush)br.ConvertFrom("#FF000000");
        }

        BrushConverter br = new BrushConverter();
        Brush green;
        Brush blue;
        Brush black;
        

        public void changeData(double progressValue,string left,string right, clr me)
        {
            switch (me)
            {
                case clr.me: progressBar.Foreground = green; break;
                case clr.sum: progressBar.Foreground = black; break;
                default: progressBar.Foreground = blue; break; 
            }
            if (progressValue > 100 || Double.IsInfinity(progressValue) || Double.IsNaN(progressValue)) progressBar.Value = 100;
            else if (progressValue < 0 ) progressValue = 0;
            else progressBar.Value = progressValue;
            labelLeft.Content = left;
            labelRight.Content = right;
        }

        public enum clr { me,other,sum }
    }
}
