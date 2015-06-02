using Detrav.TeraApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    class Config
    {
        static private IConfigManager configManager;
        private static Config config;
        public static Config c
        {
            get
            {
                if (config == null) config = new Config();
                return config;
            }
        }
        public double top;
        public double left;
        public double width;
        public double height;
        public bool hided;
        public double prevHeight;

        public static void setConfigManager(IConfigManager cfgManager)
        {
            configManager = cfgManager;
        }

        public static void save(string playerName)
        {
            if (configManager == null)
            {
                if (config != null) configManager.savePlayer(playerName, config);
            }
        }
        public static void load(string playerName)
        {
            if (configManager == null)
            {
                var conf = configManager.loadPlayer(playerName, typeof(Config));
                if (conf == null) config = new Config();
                else config = conf as Config;
            }
        }
    }
}
