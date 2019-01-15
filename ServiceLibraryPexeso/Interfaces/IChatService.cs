using System.ServiceModel;

namespace ServiceLibraryPexeso.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IClient), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(Message message);

        [OperationContract]
        void JoinLobby(string nickName);

        [OperationContract(IsOneWay = true)]
        void LeaveSession(string nickName);
    }
}
