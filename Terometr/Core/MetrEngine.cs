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
        public static string generateShort(double val, double sum, int number = 5)
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
            if (double.IsInfinity(val)) return "Infinity";
            if (double.IsNaN(val)) return "NaN";
            int num = 0;
            double res = val;
            double testVal = Math.Pow(10, number);
            while (res > testVal) { res /= 1000.0; num++; }
            int procent = (int)(val / sum * 100.0);
            if (procent < 0) procent = 0;
            else if (procent > 100) procent = 100;

            int digitsCount = number - GetNumberOfDigits(res);
            if(digitsCount <= 0)
            {
                return String.Format("{0:0}{1}({2}%)", res, kilos[num], procent);
            }
            else
            {
                string format = String.Format("{{0:F{0}}}{{1}}({{2}}%)", digitsCount);
                return String.Format(format, res, kilos[num], procent);
            }

        }
        /// <summary>
        /// По истечению 10.1с можно не следить за показателем
        /// </summary>
        //public static TimeSpan timeOutMetr = TimeSpan.FromSeconds(10.1);

        internal static string generateShort(double val, int number = 5)
        {
            if (double.IsInfinity(val)) return "Infinity";
            if (double.IsNaN(val)) return "NaN";
            int num = 0;
            double res = val;
            double testVal = Math.Pow(10, number);
            while (res > testVal) { res /= 1000.0; num++; }

            int digitsCount = number - GetNumberOfDigits(res);
            if (digitsCount <= 0)
            {
                return String.Format("{0:0}{1}", res, kilos[num]);
            }
            else
            {
                string format = String.Format("{{0:F{0}}}{{1}}", digitsCount);
                return String.Format(format, res, kilos[num]);
            }
        }

        static int GetNumberOfDigits(double d)
        {
            double abs = Math.Abs(d);
            return abs < 1 ? 0 : (int)(Math.Log10(abs) + 1);
        }
    }
}
