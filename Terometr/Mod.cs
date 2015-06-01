using Detrav.TeraApi.Interfaces;
using Detrav.Terometr.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr
{
    class Mod : ITeraMod
    {
        MainWindow window;
        ITeraClient parent;

        public void changeVisible()
        {
            if (window.IsVisible)
                hide();
            else
                show();
        }


        public void hide()
        {
            window.Hide();
        }

        public void load(ITeraClient parent)
        {
            window = new MainWindow(localConfigManager);
            window.close = false;
            this.parent = parent;
            parent.onPacketArrival += parent_onPacketArrival;
            parent.onTick += parent_onTick;
            //show();
        }

        void parent_onTick(object sender, EventArgs e)
        {
            window.doEvents();
        }

        void parent_onPacketArrival(object sender, TeraApi.Events.PacketArrivalEventArgs e)
        {
            window.opPacketArrival(sender, e);
        }

        public void show()
        {
            window.Show();
        }

        public void unLoad()
        {
            window.close = true;
            window.Close();
        }

        /*public static byte[] getModIcon()
        {
            return extractResource("Detrav.Teroniffer.Icon.jpg");
        }

        public static byte[] extractResource(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }*/

        IConfigManager localConfigManager;
        public void init(IConfigManager configManager, IAssetManager assetManager)
        {
            localConfigManager = configManager;
            //PacketStructureManager.assets = assetManager;
        }
    }
}
