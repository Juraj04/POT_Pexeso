using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using DatabasePexeso;
using ServiceLibraryPexeso;
using StatisticsWebPexeso.ViewModels;

namespace StatisticsWebPexeso.Controllers
{
    public class PlayerGamesController : Controller
    {
        // GET: PlayerGames
        public ActionResult PlayerGames(PlayerNameModel model)
        {
            
            ViewBag.Games = GetAllGames(model);
            return View();
        }

        private List<GameRound> GetAllGames(PlayerNameModel model)
        {
            using (var db = new GameDataContext())
            {
                //nechutne

                var query = db.GameRounds.Include(round => round.Game).Include(round => round.GamePlayer);
                if((model.Name == null || model.Name.IsEmpty()) && model.GameID == 0)
                    return query.Take(60).OrderBy(round => round.Game.Id).ToList();
                if (model.Name != null && !model.Name.IsEmpty() && model.GameID != 0)
                    return query.Where(round => round.Game.Id == model.GameID).Where(round =>
                        round.GamePlayer.NickName.ToLower().Contains(model.Name.ToLower())).ToList();
                if(model.Name != null && !model.Name.IsEmpty())
                    return query.Where(round => round.GamePlayer.NickName.ToLower().Contains(model.Name.ToLower())).OrderBy(round => round.Game.Id).ToList();
                return query.Where(round => round.Game.Id == model.GameID).ToList();
            }
        }
    }
}