using System;
using System.Collections.Generic;

namespace ServiceLibraryPexeso
{
    public class Game
    {
        public int Id { get; set; }
        public int CardCount { get; set; }
        public TimeSpan GameDuration { get; set; }

        public virtual ICollection<GameRound> PlayedRounds { get; set; }
    }
}
