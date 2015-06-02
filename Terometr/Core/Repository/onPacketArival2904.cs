using Detrav.TeraApi;
using Detrav.TeraApi.Events;
using Detrav.TeraApi.OpCodes;
using Detrav.TeraApi.OpCodes.P2904;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    partial class Repository
    {
        public OpCode2904 P2904(object sender, PacketArrivalEventArgs e)
        {
             switch((OpCode2904)e.packet.opCode)
            {
                case OpCode2904.S_LOGIN:
                    Logger.debug("S_LOGIN");
                    var s_login = (S_LOGIN)PacketCreator.create(e.packet);
                    setSelf(s_login.id, s_login.name);
                    updateParty(self);
                    return OpCode2904.S_LOGIN;
                case OpCode2904.S_PARTY_MEMBER_LIST:
                    Logger.debug("S_PARTY_MEMBER_LIST");
                    var s_party_list = (S_PARTY_MEMBER_LIST)PacketCreator.create(e.packet);
                    List<TeraPlayer> players = new List<TeraPlayer>();
                    foreach(var p in s_party_list.players)
                    {
                        players.Add(new TeraPlayer(p.id, p.name));
                    }
                    updateParty(players.ToArray());
                    return OpCode2904.S_PARTY_MEMBER_LIST;
                case OpCode2904.S_LEAVE_PARTY:
                    Logger.debug("S_LEAVE_PARTY");
                    leaveFromParty();
                    return OpCode2904.S_LEAVE_PARTY;
                case OpCode2904.S_LEAVE_PARTY_MEMBER:
                    var s_leave_member = (S_LEAVE_PARTY_MEMBER)PacketCreator.create(e.packet);
                    Logger.debug("S_LEAVE_PARTY_MEMBER {0}", s_leave_member.name);
                    ulong tempPlayer = getPlayerByName(s_leave_member.name);
                    leaveFromParty(tempPlayer);
                    return OpCode2904.S_LEAVE_PARTY_MEMBER;
                case OpCode2904.S_SPAWN_PROJECTILE:
                    Logger.debug("S_SPAWN_PROJECTILE");
                    var s_spawn_proj = (S_SPAWN_PROJECTILE)PacketCreator.create(e.packet);
                    addProjectile(s_spawn_proj.id, s_spawn_proj.idPlayer);
                    return OpCode2904.S_SPAWN_PROJECTILE;
                case OpCode2904.S_DESPAWN_PROJECTILE:
                    Logger.debug("S_DESPAWN_PROJECTILE");
                    var s_despawn_proj = (S_DESPAWN_PROJECTILE)PacketCreator.create(e.packet);
                    removeProjectile(s_despawn_proj.id);
                    return OpCode2904.S_DESPAWN_PROJECTILE;
                case OpCode2904.S_SPAWN_NPC:
                    Logger.debug("S_SPAWN_NPC");
                    var s_spawn_npc = (S_SPAWN_NPC)PacketCreator.create(e.packet);
                    addNpc(s_spawn_npc.id, s_spawn_npc.parentId);
                    return OpCode2904.S_SPAWN_NPC;
                case OpCode2904.S_DESPAWN_NPC:
                    Logger.debug("S_DESPAWN_NPC");
                    var s_despawn_npc = (S_DESPAWN_NPC)PacketCreator.create(e.packet);
                    removeNpc(s_despawn_npc.id);
                    return OpCode2904.S_DESPAWN_NPC;
                case OpCode2904.S_EACH_SKILL_RESULT:
                        var skill = (S_EACH_SKILL_RESULT)PacketCreator.create(e.packet);
                        skillResult(skill.idWho, skill.idTarget, skill.damage, skill.dType);
                        return OpCode2904.S_EACH_SKILL_RESULT;
            }
             return (OpCode2904)e.packet.opCode;
        }
    }
}
