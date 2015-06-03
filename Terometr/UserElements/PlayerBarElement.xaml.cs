using Detrav.TeraApi.Enums;
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

        public static BitmapImage archer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.archer.png");
        public static BitmapImage berserker = Mod.ToImage("Detrav.Terometr.assets.player_class_images.berserker.png");
        public static BitmapImage lancer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.lancer.png");
        public static BitmapImage mystic = Mod.ToImage("Detrav.Terometr.assets.player_class_images.mystic.png");
        public static BitmapImage priest = Mod.ToImage("Detrav.Terometr.assets.player_class_images.priest.png");
        public static BitmapImage reaper = Mod.ToImage("Detrav.Terometr.assets.player_class_images.reaper.png");
        public static BitmapImage slayer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.slayer.png");
        public static BitmapImage sorcerer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.sorcerer.png");
        public static BitmapImage warrior = Mod.ToImage("Detrav.Terometr.assets.player_class_images.warrior.png");
        

        public void changeData(double progressValue,string left,string right, clr me, PlayerClass playerClass)
        {
            switch(playerClass)
            {
                case PlayerClass.Archer: imageClass.Source = archer; break;
                case PlayerClass.Berserker: imageClass.Source = berserker; break;
                case PlayerClass.Lancer: imageClass.Source = lancer; break;
                case PlayerClass.Mystic: imageClass.Source = mystic; break;
                case PlayerClass.Priest: imageClass.Source = priest; break;
                case PlayerClass.Reaper: imageClass.Source = reaper; break;
                case PlayerClass.Slayer: imageClass.Source = slayer; break;
                case PlayerClass.Sorcerer: imageClass.Source = sorcerer; break;
                case PlayerClass.Warrior: imageClass.Source = warrior; break;
                default: imageClass.Source = null; break;
            }
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

        public string getText()
        {
            return String.Format("{0} - {1}", labelLeft.Content, labelRight.Content);
        }
    }
}
