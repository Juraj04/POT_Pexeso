using System;
using System.ServiceModel;
using ServerPexeso.Services;

namespace ServerPexeso
{
    class Program
    {
        private static ServiceHost _loginHost;
        private static ServiceHost _gameHost;
        private static ServiceHost _chatHost;

        static void Main()
        {
            HostLogin();
            HostGame();
            HostChat();
            Console.ReadLine();
            _loginHost.Close();
            _gameHost.Close();
            _chatHost.Close();
        }

        private static void HostChat()
        {
            _chatHost = new ServiceHost(typeof(ChatService));
            try
            {
                _chatHost.Open();
                Console.WriteLine("The chat service is ready. Press <ENTER> to terminate service.");

            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                _chatHost.Abort();
            }
        }

        private static void HostGame()
        {
            _gameHost = new ServiceHost(typeof(GameService));
            try
            {
                _gameHost.Open();
                Console.WriteLine("The game service is ready. Press <ENTER> to terminate service.");

            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                _gameHost.Abort();
            }
        }
        private static void HostLogin()
        {
            _loginHost = new ServiceHost(typeof(LoginService));
            try
            {
                _loginHost.Open();
                Console.WriteLine("The login service is ready. Press <ENTER> to terminate service.");

            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                _loginHost.Abort();
            }
        }
    }
}
