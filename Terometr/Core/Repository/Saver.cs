using Detrav.TeraApi.Interfaces;
using Detrav.Terometr.UserElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    partial class Repository
    {
        IAssetManager assetManager;
        public void save()
        {
            string file = String.Format("{0}_{1}.txt",self.name,DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
            string folder = assetManager.getMyFolder();
            //id,class,name,dps,damage,hps,heal,damageTaken,healTaken
            string[,] table = new string[9, playersSnapShot.Count+2];
            addToTable(ref table,0,"Id","Class","Name","Dps","Damage","Hps","Heal","Damage Taken","Heal Taken");
            int num = 0;
            foreach(var pl in playersSnapShot)
            {
                num++;//Так и должно быть, первый = 1
                TeraPlayer p = pl.Value;
                addToTable(ref table, num,
                    p.id,
                    p.playerClass,
                    p.name,
                    ListBoxBars.generateRight(p.dps,dpsSum),
                    ListBoxBars.generateRight(p.damage, damageSum),
                    ListBoxBars.generateRight(p.hps, hpsSum),
                    ListBoxBars.generateRight(p.heal, healSum),
                    ListBoxBars.generateRight(p.damageTaken, damageTakenSum),
                    ListBoxBars.generateRight(p.healTaken, healTakenSum));
            }
            using(TextWriter tw = new StreamWriter(Path.Combine(folder,file)))
            {
                
            }
        }

        private void addToTable(ref string[,] table,int num, params object[] strs)
        {
            for(int i = 0; i<strs.Length;i++)
            {
                table[i, num] = strs[i].ToString();
            }
        }
    }
}
