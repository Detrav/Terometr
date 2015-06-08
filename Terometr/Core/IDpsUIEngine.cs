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
        void addSkill(TeraSkill skill);
        //void updateData(TeraSkill[] history);
        void doEvents();
        //void updateLayout();
        void setSelf(TeraPlayer self);
        void clear();
        string generateTable();
        void reSetting(Config config);
        TeraApi.Interfaces.ITeraClient teraClient { get; set; }
    }
}