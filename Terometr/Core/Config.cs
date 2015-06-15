using Detrav.TeraApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    public class Config
    {
        public Config(IConfigManager configManager)
        {
            this.configManager = configManager;
        }
        public double top = Double.NaN;
        public double left = Double.NaN;
        public double width = 100;
        public double height = 100;
        public bool hided = false;
        public double prevHeight = 100;
        private IConfigManager configManager;
        public bool party = true;
        public bool log = false;
        public bool player = true;
        public bool autoTarget = false;
        public bool group = true;
        

        public void save(string playerName)
        {
            if (configManager != null)
            {
                configManager.savePlayer(playerName, this);
            }
        }
        public void load(string playerName)
        {
            if (configManager != null)
            {
                var conf = configManager.loadPlayer(playerName, typeof(Config)) as Config;
                if (conf == null) return;
                top = conf.top;
                left = conf.left;
                width = conf.width;
                height = conf.height;
                hided = conf.hided;
                prevHeight = conf.prevHeight;
                log = conf.log;
                party = conf.party;
                player = conf.player;
                autoTarget = conf.autoTarget;
                group = conf.group;
            }
        }
    }
}