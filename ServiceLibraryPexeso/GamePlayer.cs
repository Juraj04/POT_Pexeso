using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLibraryPexeso
{
    public class GamePlayer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string NickName { get; set; }

        public virtual ICollection<GameRound> PlayerRounds { get; set; }

    }
}
