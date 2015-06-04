using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    partial class Repository
    {
        Dictionary<ulong, DpsPlayer> party = new Dictionary<ulong, DpsPlayer>();
        Dictionary<ulong, ulong> projectiles = new Dictionary<ulong, ulong>();
        Dictionary<ulong, ulong> npcs = new Dictionary<ulong, ulong>();
        DpsPlayer self = new DpsPlayer(new TeraApi.Core.TeraPlayer(0,"unknown"));

        public DpsPlayer getSelf() { return self; }
        //public void setSelf(ulong id, string name, TeraApi.Enums.PlayerClass playerClass) { self.id = id; self.name = name; self.playerClass = playerClass; }


        public void clear()
        {
            save();
            playersSnapShot.Clear();
            playersDataGrid.Clear();
            foreach(var pair in party)
            {
                pair.Value.clear();
            }
        }


        public void parent_onNewPartyList(object sender, TeraApi.Events.Party.NewPartyListEventArgs e)
        {
            Dictionary<ulong, DpsPlayer> tempPlayers = new Dictionary<ulong, DpsPlayer>();
            foreach (var p in e.players)
            {
                DpsPlayer player;
                if (party.TryGetValue(p.id, out player))
                {
                    tempPlayers.Add(p.id, player);
                    continue;
                }
                tempPlayers.Add(p.id, new DpsPlayer(p));
            }
            party.Clear();
            foreach (var p in tempPlayers)
            {
                party.Add(p.Key, p.Value);
            }
        }

        public void parent_onLogin(object sender, TeraApi.Events.Self.LoginEventArgs e)
        {
            this.self = new DpsPlayer(e.player);
            party.Clear();
            party.Add(self.player.id,self);
        }
    }
}
