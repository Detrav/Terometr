using Detrav.TeraApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    interface IDpsUIEngine
    {
        void doEvents();
        void setSelf(TeraPlayer self);
        void clear();
        string generateTable();
        void reSetting(Config config);
        TeraApi.Interfaces.ITeraClient teraClient { get; set; }

        void skillResult(TeraApi.Events.SkillResultEventArgs e);
    }
}