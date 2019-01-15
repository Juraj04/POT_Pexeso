using System;
using System.Collections.Generic;
using System.ServiceModel;
using ServiceLibraryPexeso;
using ServiceLibraryPexeso.Interfaces;

namespace GuiPexeso
{
    class GameClient : IClient
    {
        ///public GamePlayer GamePlayer { get; set; }
        public string NickName { get; set; }
        private IGameService _gameService;
        private IChatService _chatService;

        public delegate void MessageReceived(string message, string sender);
        public event MessageReceived OnMessageReceived;

        public delegate void CardsReceived(char[][] cards, string opponentNick, bool turn);
        public event CardsReceived OnCardsReceived;

        public delegate bool InvitationReceived(CardTypes cards, string hostNick);
        public event InvitationReceived OnInvitationReceived;

        public delegate void FlippedReceived(int row, int column);
        public event FlippedReceived OnFlippedReceived;

        public delegate void TurnReceived();
        public event TurnReceived OnTurnReceived;

        public delegate void OpponentsScoreReceived(int score);
        public event OpponentsScoreReceived OnOpponentScoreReceived;

        public delegate void FinishCurrentGame();
        public event FinishCurrentGame OnGameFinished;
        public event FinishCurrentGame OnForceFinishGame;

        private int _currentGameId;

        public GameClient()
        {

        }

        public GameClient(string nickName)
        {
            //GamePlayer = player;
            NickName = nickName;
            DuplexChannelFactory<IGameService> cf = new DuplexChannelFactory<IGameService>(this, "GameService");
            _gameService = cf.CreateChannel();
            DuplexChannelFactory<IChatService> cf2 = new DuplexChannelFactory<IChatService>(this, "ChatService");
            _chatService = cf2.CreateChannel();

            _gameService.JoinLobby(NickName);
            _chatService.JoinLobby(NickName);
        }

        public void Disconnect()
        {
            _gameService.LeaveSession(NickName);
            _chatService.LeaveSession(NickName);
        }

        public void ReceiveMessage(Message message)
        {
            OnMessageReceived?.Invoke(message.ToString(), message.Sender);
        }

        public bool ReceiveInvitation(int gameId, CardTypes cards, string hostNick)
        {
            return OnInvitationReceived?.Invoke(cards, hostNick) == true;
        }

        public void StartGame(int gameId, char[][] cards, string opponentNick, bool turn)
        {
            _currentGameId = gameId;
            OnCardsReceived?.Invoke(cards, opponentNick, turn);
        }

        public void ReceiveFlipped(int row, int column)
        {
            OnFlippedReceived?.Invoke(row, column);
        }

        public void ReceiveTurn()
        {
            OnTurnReceived?.Invoke();
        }

        public void ReceiveOpponentsScore(int opponentsScore)
        {
            OnOpponentScoreReceived?.Invoke(opponentsScore);
        }

        public void EndGame(int myScore, int opponentsScore, Result myResult, Result opponentResult)
        {
            _gameService.FinishGame(_currentGameId, myScore, opponentsScore, myResult, opponentResult);
        }

        public void FinishGame()
        {
            OnGameFinished?.Invoke();
            _currentGameId = -1;
        }

        public void ForceFinishGame()
        {
            OnForceFinishGame?.Invoke();
            _currentGameId = -1;
        }

        public void SendFlipped(int row, int column)
        {
            _gameService.SendFlipped(row, column, _currentGameId);
        }

        public void SendTurn()
        {
            _gameService.SendTurn(_currentGameId);
        }

        public void SendScore(int score)
        {
            _gameService.SendMyScore(score, _currentGameId);
        }

        public void AddMove()
        {
            _gameService.AddMove(_currentGameId);
        }

        public List<string> GetAllOnlinePlayers()
        {
            return _gameService.RefreshWaitingPlayers(NickName);
        }

        public void SendMessage(string message, string addressee)
        {
            var m = new Message()
            {
                Content = message,
                Sender = NickName,
                Receiver = addressee,
                Date = DateTime.Now
            };
            _chatService.SendMessage(m);
        }

        public void SendInvitation(string hostNickName, string opponentNickName, CardTypes card)
        {
            _gameService.SendInvitation(hostNickName, opponentNickName, card);
        }
    }
}
