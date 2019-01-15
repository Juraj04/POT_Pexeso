using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLibraryPexeso
{
    public class GameRound
    {
        [Key, Column(Order = 0)]
        public string PlayerNickName { get; set; }
        [Key, Column(Order = 1)]
        public int GameId { get; set; }
        public virtual GamePlayer GamePlayer { get; set; }
        public virtual Game Game { get; set; }
        public Result Result { get; set; }
        public int MovesCount { get; set; }
    }
}