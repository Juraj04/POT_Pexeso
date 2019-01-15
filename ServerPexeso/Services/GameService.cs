using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using DatabasePexeso;
using ServiceLibraryPexeso;
using ServiceLibraryPexeso.Interfaces;

namespace ServerPexeso.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class GameService : IGameService
    {
        private static IClient ClientCallback => OperationContext.Current.GetCallbackChannel<IClient>();
        public Dictionary<string, ConnectedPlayer> AllPlayers { get; set; }
        public Dictionary<int, CurrentGame> CurrentGames { get; set; }
        private const string CardsPics = "ABCDEFGHIJKLMNOPQRSTUVWXYZ@#$%&*";//ABECEDA 26, po hviezdicku 32

        private int _gameId;

        public GameService()
        {
            AllPlayers = new Dictionary<string, ConnectedPlayer>();
            CurrentGames = new Dictionary<int, CurrentGame>();
            _gameId = 0;
        }

        public void JoinLobby(string nickName)
        {
            if(!AllPlayers.ContainsKey(nickName))
                AllPlayers.Add(nickName, new ConnectedPlayer(ClientCallback, nickName));
        }

        public void LeaveSession(string nickName)
        {
            AllPlayers.Remove(nickName);
        }

        public List<string> RefreshWaitingPlayers(string nickName)
        {
            return AllPlayers.Where(pair => pair.Key != nickName).Select(pair => pair.Value.Playing ? "#" + pair.Key : pair.Key).ToList();
        }

        public void SendInvitation(string hostNick, string opponentNick, CardTypes cards)
        {
            if (!AllPlayers.ContainsKey(opponentNick) || AllPlayers[opponentNick].Playing)
                return;
            if (AllPlayers[hostNick].Playing) //if he already plays (could happen)
                return;

            _gameId++;
            if (AllPlayers[opponentNick].Client.ReceiveInvitation(_gameId, cards, hostNick))
            {
                if (AllPlayers[hostNick].Playing) //if he already plays (could happen as well)
                    return;
                var game = new CurrentGame(AllPlayers[hostNick], AllPlayers[opponentNick], cards, _gameId);
                CurrentGames.Add(_gameId, game);
                var area = CreateRandomArea(cards);
                game.Host.Playing = true;
                game.Opponent.Playing = true;
                game.Host.Client.StartGame(_gameId, area, opponentNick, true);
                game.Opponent.Client.StartGame(_gameId, area, hostNick, false);
                game.Start(ForceFinishGame);
                return;
            }
            _gameId--;
        }

        public void SendFlipped(int row, int column, int gameId)
        {
            var game = CurrentGames[gameId];
            var other = game.GetOther(ClientCallback);
            other.Client.ReceiveFlipped(row, column);
            game.ResetTimer();
        }

        private void ForceFinishGame(CurrentGame game)
        {
            var other = game.Host;
            var me = game.Opponent;
            game.Stop();
            other.Client.ForceFinishGame();
            me.Client.ForceFinishGame();
            CurrentGames.Remove(game.GameId);
            me.Playing = false;
            other.Playing = false;
        }
        public void SendTurn(int gameId)
        {
            var game = CurrentGames[gameId];
            var other = game.GetOther(ClientCallback);
            other.Client.ReceiveTurn();
        }

        public void SendMyScore(int myScore, int gameId)
        {
            var game = CurrentGames[gameId];
            var other = game.GetOther(ClientCallback);
            other.Client.ReceiveOpponentsScore(myScore);
        }

        public void FinishGame(int gameId, int myScore, int opponentsScore, Result myResult, Result opponentsResult)
        {
            var game = CurrentGames[gameId];
            var other = game.GetOther(ClientCallback);
            var me = game.GetMe(ClientCallback);
            other.Client.FinishGame();
            game.Stop();
            CurrentGames.Remove(gameId);
            me.Playing = false;
            other.Playing = false;

            using (var db = new GameDataContext())
            {
                GamePlayer dbMe = db.GamePlayers.Find(me.NickName);
                GamePlayer dbOther = db.GamePlayers.Find(other.NickName);

                var dbGame = new Game()
                {
                    CardCount = game.CardTypes.Count,
                    GameDuration = game.GameDuration

                };
                db.Games.Add(dbGame);
                db.SaveChanges();

                var dbMyGameRound = new GameRound()
                {
                    GamePlayer = dbMe,
                    Game = dbGame,
                    GameId = dbGame.Id,
                    MovesCount = me.MovesCount,
                    PlayerNickName = me.NickName,
                    Result = myResult
                };

                var dbOpponentRound = new GameRound()
                {
                    GamePlayer = dbOther,
                    Game = dbGame,
                    GameId = dbGame.Id,
                    MovesCount = other.MovesCount,
                    PlayerNickName = other.NickName,
                    Result = opponentsResult
                };
                db.GameRounds.Add(dbMyGameRound); //add rounds
                db.GameRounds.Add(dbOpponentRound);

                dbGame.PlayedRounds.Add(dbMyGameRound); //add games reference
                dbGame.PlayedRounds.Add(dbOpponentRound);

                dbMe?.PlayerRounds.Add(dbMyGameRound); //add players reference
                dbOther?.PlayerRounds.Add(dbOpponentRound);

                db.SaveChanges(); //save

            }

        }

        public void AddMove(int gameId)
        {
            var game = CurrentGames[gameId];
            var me = game.GetMe(ClientCallback);
            me.MovesCount++;
        }

        private char[][] CreateRandomArea(CardTypes cards)
        {
            var chars = new List<char>();
            var counter = 0;
            for (int i = 0; i < 2; i++)
            {
                foreach (var cardsPic in CardsPics)
                {
                    if (counter == cards.Count / 2)
                        break;
                    chars.Add(cardsPic);
                    counter++;
                }

                counter = 0;
            }
            var rand = new Random();
            chars = chars.OrderBy(c => rand.Next()).ToList();

            char[][] result = new char[cards.Row][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new char[cards.Column];
            }

            int a = 0, b = 0;

            foreach (var car in chars)
            {
                result[a][b] = car;
                b++;
                if (b == cards.Column)
                {
                    b = 0;
                    a++;
                }
            }

            return result;
        }
    }
}
