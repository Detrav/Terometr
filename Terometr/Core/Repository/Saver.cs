using Detrav.TeraApi.Enums;
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
        public IAssetManager assetManager { get; set; }
        public string save()
        {
            if (assetManager != null)
            {
                string file = String.Format("{0}_{1}.txt", self.name, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
                string folder = assetManager.getMyFolder();
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                //id,class,name,dps,damage,hps,heal,damageTaken,healTaken
                TableWriter tw = new TableWriter(9);
                tw.addRow("Id", "Class", "Name", "Dps", "Damage", "Hps", "Heal", "Damage Taken", "Heal Taken");
                foreach (var pl in playersSnapShot)
                {
                    TeraPlayer p = pl.Value;
                    tw.addRow(
                        p.id,
                        p.playerClass,
                        p.name,
                        ListBoxBars.generateRight(p.dps, dpsSum),
                        ListBoxBars.generateRight(p.damage, damageSum),
                        ListBoxBars.generateRight(p.hps, hpsSum),
                        ListBoxBars.generateRight(p.heal, healSum),
                        ListBoxBars.generateRight(p.damageTaken, damageTakenSum),
                        ListBoxBars.generateRight(p.healTaken, healTakenSum));
                }
                tw.addRow(
                        0,
                        PlayerClass.Empty,
                        "Всего",
                        ListBoxBars.generateRight(dpsSum, dpsSum),
                        ListBoxBars.generateRight(damageSum, damageSum),
                        ListBoxBars.generateRight(hpsSum, hpsSum),
                        ListBoxBars.generateRight(healSum, healSum),
                        ListBoxBars.generateRight(damageTakenSum, damageTakenSum),
                        ListBoxBars.generateRight(healTakenSum, healTakenSum));
                using (TextWriter textWriter = new StreamWriter(Path.Combine(folder, file)))
                {
                    return tw.writeToStream(textWriter);
                }
            }
            return null;
        }
    }
}
