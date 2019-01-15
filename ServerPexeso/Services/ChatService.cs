using System.Collections.Generic;
using System.ServiceModel;
using ServiceLibraryPexeso;
using ServiceLibraryPexeso.Interfaces;

namespace ServerPexeso.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        private static IClient ClientCallback => OperationContext.Current.GetCallbackChannel<IClient>();
        public Dictionary<string, IClient> AllPlayers { get; set; } = new Dictionary<string, IClient>();


        public void SendMessage(Message message)
        {
            if (AllPlayers.ContainsKey(message.Receiver))
                AllPlayers[message.Receiver].ReceiveMessage(message);
        }

        public void JoinLobby(string nickName)
        {
            if (!AllPlayers.ContainsKey(nickName))
                AllPlayers.Add(nickName, ClientCallback);
        }

        public void LeaveSession(string nickName)
        {
            if (AllPlayers.ContainsKey(nickName))
                AllPlayers.Remove(nickName);
        }
    }
}
