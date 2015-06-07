using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core.Agro
{
    class AgroElement
    {
        List<DatePlusVal> values = new List<DatePlusVal>();
        public TeraApi.Core.TeraPlayer teraPlayer;
        static TimeSpan timeOut = TimeSpan.FromSeconds(50);

        public double value(DateTime now)
        {
            double result = 0;
            for (int i = 0; i < values.Count; i++)
            {
                var el = values[i];
                if (now - el.time > timeOut)
                {
                    values.RemoveAt(i);
                    i--;
                    continue;
                }
                result +=
                    el.value
                    *
                    Math.Floor(1 - 0.1 * (now - el.time).TotalSeconds / 5);
            }
            return result;
        }

        public AgroElement(TeraApi.Core.TeraPlayer teraPlayer)
        {
            // TODO: Complete member initialization
            this.teraPlayer = teraPlayer;
        }

        internal void add(uint p, DateTime dateTime)
        {
            values.Add(new DatePlusVal(p, dateTime));
        }
    }

    class DatePlusVal
    {
        public DateTime time;
        public uint value;
        public DatePlusVal(uint value,DateTime time)
        {
            this.value = value;
            this.time = time;
        }
    }
}
