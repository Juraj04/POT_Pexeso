using System.Collections.Generic;
using System.ServiceModel;

namespace ServiceLibraryPexeso.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IClient), SessionMode = SessionMode.Required)]
    public interface IGameService
    {
        [OperationContract(IsOneWay = true)]
        void JoinLobby(string nickName);

        [OperationContract(IsOneWay = true)]
        void LeaveSession(string nickName);

        [OperationContract]
        List<string> RefreshWaitingPlayers(string nickName);

        [OperationContract(IsOneWay = true)]
        void SendInvitation(string hostNick, string opponentNick, CardTypes cards);

        [OperationContract]
        void SendFlipped(int row, int column, int gameId);

        [OperationContract]
        void SendTurn(int gameId);

        [OperationContract]
        void SendMyScore(int myScore, int gameId);

        [OperationContract]
        void FinishGame(int gameId, int myScore, int opponentsScore, Result myResult, Result opponentsResult);

        [OperationContract]
        void AddMove(int gameId);
    }
}
