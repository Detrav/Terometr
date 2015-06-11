using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    public class ComboBoxHiddenItem
    {
        public ComboBoxHiddenItem(ulong id, string text)
        {
            this.id = id;
            this.text = text;
        }
        public override string ToString()
        {
            return text;
        }

        public ulong id { get; set; }
        public string text { get; set; }
    }
}
