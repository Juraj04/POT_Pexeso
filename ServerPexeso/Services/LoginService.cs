using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using DatabasePexeso;
using ServiceLibraryPexeso;
using ServiceLibraryPexeso.Interfaces;

namespace ServerPexeso.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class LoginService : ILoginService
    {
        public HashSet<string> LoggedPlayers { get; set; }

        public LoginService()
        {
            LoggedPlayers = new HashSet<string>();
        }

        public bool LoginPlayer(string playerNick)
        {
            if (LoggedPlayers.Contains(playerNick))
                return false;
            using (var db = new GameDataContext())
            {
                GamePlayer p = db.GamePlayers.Find(playerNick);
                if (p != null)
                    LoggedPlayers.Add(playerNick);
                return p != null;
            }
        }

        public bool RegisterPlayer(string playerNick)
        {
            using (var db = new GameDataContext())
            {
                if (db.GamePlayers.Count(player => player.NickName == playerNick) == 0)
                {
                    var pl = new GamePlayer() { NickName = playerNick };
                    db.GamePlayers.Add(pl);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void LogOut(string playerNick)
        {
            LoggedPlayers.Remove(playerNick);
        }
    }
}
