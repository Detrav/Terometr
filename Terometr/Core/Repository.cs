﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    partial class Repository
    {
        static private Repository instance;
        static public Repository R
        {
            get
            {
                if (instance == null) instance = new Repository();
                return instance;
            }
        }

        Dictionary<ulong, TeraPlayer> party = new Dictionary<ulong, TeraPlayer>();
        Dictionary<ulong, ulong> projectiles = new Dictionary<ulong, ulong>();
        Dictionary<ulong, ulong> npcs = new Dictionary<ulong, ulong>();
        TeraPlayer self = new TeraPlayer(0, "UNKNOWN");

        public TeraPlayer getSelf() { return self; }
        public void setSelf(ulong id, string name) { self.id = id; self.name = name; }


        private void skillResult(ulong who, ulong target, uint damage, ushort type)
        {
            
        }

        private void removeNpc(ulong id)
        {
            if (npcs.ContainsKey(id)) npcs.Remove(id);
        }

        private void addNpc(ulong id, ulong parent)
        {
            if (party.ContainsKey(parent)) npcs.Add(id, parent);
        }

        private void removeProjectile(ulong id)
        {
            if (projectiles.ContainsKey(id)) projectiles.Remove(id);
        }

        private void addProjectile(ulong id, ulong parent)
        {
            projectiles.Add(id, parent);
        }

        private void leaveFromParty(ulong id)
        {
            TeraPlayer p;
            if (party.TryGetValue(id, out p))
                party.Remove(id);
        }

        private ulong getPlayerByName(string name)
        {
            foreach(var pair in party)
            {
                if (pair.Value.name == name) return pair.Value.id;
            }
            return 0;
        }

        private void leaveFromParty()
        {
            updateParty(self);
        }

        private void updateParty(TeraPlayer[] teraPlayer)
        {
            party.Clear();
            foreach (var p in teraPlayer)
                party.Add(p.id, p);
        }

        private void updateParty(TeraPlayer self)
        {
            this.self = self;
            party.Clear();
            party.Add(self.id, self);
        }

        public void clear()
        {
            foreach(var pair in party)
            {
                pair.Value.clear();
            }
        }
    }
}
