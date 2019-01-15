using System.ServiceModel;

namespace ServiceLibraryPexeso.Interfaces
{
    [ServiceContract]
    public interface IClient
    {
        string NickName { get; set; }

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(Message message);

        [OperationContract]
        bool ReceiveInvitation(int gameId, CardTypes cards, string hostNick);

        [OperationContract(IsOneWay = true)]
        void StartGame(int gameId, char[][] cards, string opponentNick, bool turn);

        [OperationContract]
        void ReceiveFlipped(int row, int column);

        [OperationContract]
        void ReceiveTurn();

        [OperationContract]
        void ReceiveOpponentsScore(int opponentsScore);

        [OperationContract]
        void FinishGame();

        [OperationContract]
        void ForceFinishGame();
    }
}
