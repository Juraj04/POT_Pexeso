using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DatabasePexeso;

namespace StatisticsWebPexeso.Controllers
{
    public class PlayersController : Controller
    {
        private bool _ascending;
        // GET: Players
        public ActionResult Players()
        {
            _ascending = true;
            PutPlayersToBag();
            return View();
        }

        public void PutPlayersToBag()
        {
            ViewBag.Players = GetAllPlayers(_ascending);
            _ascending = !_ascending;
            RedirectToAction("Players");
        }


        private List<string> GetAllPlayers(bool ascending)
        {
            using (var db = new GameDataContext())
            {
                if(ascending)
                    return db.GamePlayers.OrderBy(player => player.NickName).Select(player => player.NickName).ToList();
                return db.GamePlayers.OrderByDescending(player => player.NickName).Select(player => player.NickName).ToList();
            }
        }
    }
}