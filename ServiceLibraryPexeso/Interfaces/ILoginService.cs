using System.ServiceModel;

namespace ServiceLibraryPexeso.Interfaces
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ILoginService
    {
        [OperationContract]
        bool LoginPlayer(string playerNick);

        [OperationContract]
        bool RegisterPlayer(string playerNick);

        [OperationContract]
        void LogOut(string playerNick);
    }
}
