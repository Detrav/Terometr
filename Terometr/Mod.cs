using Detrav.TeraApi.Interfaces;
using Detrav.Terometr.Core;
using Detrav.Terometr.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Detrav.Terometr
{
    class Mod : ITeraMod
    {
        MainWindow window;
        ITeraClient parent;
        IConfigManager configManager;
        IAssetManager assetManager;
        //Repository R;

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
            window = new MainWindow();
            window.close = false;
            window.teraClient = parent;
            this.parent = parent;
            //parent.onPacketArrival += parent_onPacketArrival;
            parent.onLogin += window.parent_onLogin;
            //parent.onNewPartyList += R.parent_onNewPartyList;
            parent.onMakeSkillResult += window.parent_onMakeSkillResult;
            parent.onTakeSkillResult += window.parent_onTakeSkillResult;
            parent.onTick += parent_onTick;
            //show();
        }

        void parent_onLogin(object sender, TeraApi.Events.Self.LoginEventArgs e)
        {
            //R.parent_onLogin(sender, e);
            //window.login();
        }

        void parent_onTick(object sender, EventArgs e)
        {
            window.doEvents();
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

        //IConfigManager localConfigManager;
        public void init(IConfigManager configManager, IAssetManager assetManager)
        {
            //R = new Repository();
            window.assetManager = assetManager;
            window.configManager = configManager;
            //R.assetManager = assetManager;
            //localConfigManager = configManager;
            //PacketStructureManager.assets = assetManager;
        }

        public static BitmapImage ToImage(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = resFilestream;
                image.EndInit();
                return image;

            }
        }
    }
}
