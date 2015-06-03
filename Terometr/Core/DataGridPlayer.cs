using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Detrav.Terometr.Core
{
    class DataGridPlayer
    {
        public BitmapImage image { get; set; }
        public string name { get; set; }
        public ulong dps { get; set; }
        public ulong damage { get; set; }
        public ulong hps { get; set; }
        public ulong heal { get; set; }
        public ulong damageTaken { get; set; }
        public ulong healTaken { get; set; }
    }
}