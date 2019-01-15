using ServiceLibraryPexeso;

namespace DatabasePexeso
{
    using System.Data.Entity;

    public class GameDataContext : DbContext
    {
        // Your context has been configured to use a 'GameDataContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'ServerPexeso.GameDataContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'GameDataContext' 
        // connection string in the application configuration file.
        public GameDataContext()
            : base("name=DataContext")
        {
            //Configuration.ProxyCreationEnabled = false;
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<GamePlayer> GamePlayers { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameRound> GameRounds { get; set; }
    }
}