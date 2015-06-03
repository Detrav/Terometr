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
        public double top = Double.NaN;
        public double left = Double.NaN;
        public double width = 100;
        public double height = 100;
        public bool hided = false;
        public double prevHeight = 100;

        public static void setConfigManager(IConfigManager cfgManager)
        {
            configManager = cfgManager;
        }

        public static void save(string playerName)
        {
            if (configManager != null)
            {
                if (config != null) configManager.savePlayer(playerName, config);
            }
        }
        public static void load(string playerName)
        {
            if (configManager != null)
            {
                var conf = configManager.loadPlayer(playerName, typeof(Config));
                if (conf == null) config = new Config();
                else config = conf as Config;
            }
        }
    }
}
