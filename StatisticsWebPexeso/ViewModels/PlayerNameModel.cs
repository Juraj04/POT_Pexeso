using System.ComponentModel;

namespace StatisticsWebPexeso.ViewModels
{
    public class PlayerNameModel
    {
        [DisplayName("Enter player nick name")]
        public string Name { get; set; } = "";

        [DisplayName("Enter game id")]
        public int GameID { get; set; }
    }
}