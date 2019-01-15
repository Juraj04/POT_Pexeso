using ServiceLibraryPexeso.Interfaces;

namespace ServerPexeso
{
    class ConnectedPlayer
    {
        public IClient Client { get; set; }
        public bool Playing { get; set; } = false;
        public string NickName { get; set; }
        public int MovesCount { get; set; }

        public ConnectedPlayer(IClient client, string nickName)
        {
            Client = client;
            NickName = nickName;
            MovesCount = 0;
        }
    }
}
