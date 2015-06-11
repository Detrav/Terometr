using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Detrav.Terometr.Core
{
    static class MetrEngine
    {
        public static BitmapImage archer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.archer.png");
        public static BitmapImage berserker = Mod.ToImage("Detrav.Terometr.assets.player_class_images.berserker.png");
        public static BitmapImage lancer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.lancer.png");
        public static BitmapImage mystic = Mod.ToImage("Detrav.Terometr.assets.player_class_images.mystic.png");
        public static BitmapImage priest = Mod.ToImage("Detrav.Terometr.assets.player_class_images.priest.png");
        public static BitmapImage reaper = Mod.ToImage("Detrav.Terometr.assets.player_class_images.reaper.png");
        public static BitmapImage slayer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.slayer.png");
        public static BitmapImage sorcerer = Mod.ToImage("Detrav.Terometr.assets.player_class_images.sorcerer.png");
        public static BitmapImage warrior = Mod.ToImage("Detrav.Terometr.assets.player_class_images.warrior.png");
        private static string[] kilos = new string[] { "", "K", "M", "B", "T", "q", "Q", "s", "S", "O", "N", "d" };
        public static string generateRight(double val, double sum)
        {
            /*
             * 100.00                       100    100.00
             * 1000.0                     1 000    1000.0
             * 100.00K                   10 000    10000
             * 
             * 1000.0K                  100 000    100.00K
             * 100.00M                1 000 000    1000.0K
             *                       10 000 000    10000K
             *                       
             *                      100 000 000    100.00M
             *                    1 000 000 000    1000.0M
             *                   10 000 000 000    10000M
             *                  100 000 000 000
             *                1 000 000 000 000
             *               10 000 000 000 000
             *              100 000 000 000 000
             *            1 000 000 000 000 000
             *           10 000 000 000 000 000
             *          100 000 000 000 000 000
             *        1 000 000 000 000 000 000
             *       10 000 000 000 000 000 000
             *      100 000 000 000 000 000 000
             *    1 000 000 000 000 000 000 000
             *   10 000 000 000 000 000 000 000
             */

            int num = 0;
            double res = val;
            while (res >= 100000) { res /= 1000.0; num++; }
            int procent = (int)(val / sum * 100.0);
            if (procent < 0) procent = 0;
            else if (procent > 100) procent = 100;

            if (res < 1000)
            {
                return String.Format("{0:0.00}{1}({2}%)", res, kilos[num], procent);
            }
            if (res < 10000)
            {
                return String.Format("{0:0.0}{1}({2}%)", res, kilos[num], procent);
            }
            return String.Format("{0:0}{1}({2}%)", res, kilos[num], procent);
        }
        /// <summary>
        /// По истечению 10.1с можно не следить за показателем
        /// </summary>
        public static TimeSpan timeOutMetr = TimeSpan.FromSeconds(10.1);
    }
}
