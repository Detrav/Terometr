using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    class TableWriter
    {
        List<string[]> table;
        int col;
        int[] widths;
        public TableWriter(int col)
        {
            this.col = col;

            table = new List<string[]>();
            widths = new int[col];
        }

        public void addRow(params object[] strs)
        {
            var row = new string[col];
            if (strs != null)
                for (int i = 0; i < strs.Length && i < col; i++)
                {
                    row[i] = strs[i].ToString();
                    widths[i] = Math.Max(widths[i], row[i].Length);
                }
            table.Add(row);
        }

        public string writeToStream(TextWriter tw)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < widths.Length; i++)
            {
                sb.AppendFormat("{{{0},{1}}}", i, widths[i]);
                if (i == widths.Length - 1)
                    continue;
                sb.Append("|");
            }
            string format = sb.ToString();
            StringBuilder result = new StringBuilder();
            foreach (var el in table)
            {
                tw.WriteLine(format, el);
                result.AppendFormat(format, el);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
    }
}
