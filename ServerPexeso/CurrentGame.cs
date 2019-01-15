using System;
using System.Diagnostics;
using System.Timers;
using ServiceLibraryPexeso;
using ServiceLibraryPexeso.Interfaces;

namespace ServerPexeso
{
    class CurrentGame
    {
        public int GameId { get; set; }
        public ConnectedPlayer Host { get; set; }
        public ConnectedPlayer Opponent { get; set; }
        public CardTypes CardTypes { get; set; }
        public TimeSpan GameDuration => _stopWatch.Elapsed;
        private Timer _timer;

        private Stopwatch _stopWatch;

        public delegate void TimerCallback(CurrentGame game);

        public CurrentGame(ConnectedPlayer host, ConnectedPlayer opponent, CardTypes cardTypes, int gameId)
        {
            Host = host;
            Opponent = opponent;
            CardTypes = cardTypes;
            _stopWatch = new Stopwatch();
            host.MovesCount = 0;
            opponent.MovesCount = 0;
            GameId = gameId;
            _timer = new Timer(60000);
            _timer.AutoReset = false;
        }

        public ConnectedPlayer GetOther(IClient client)
        {
            if (Host.Client == client)
                return Opponent;
            return Host;
        }

        public ConnectedPlayer GetMe(IClient client)
        {
            if (Host.Client == client)
                return Host;
            return Opponent;
        }

        public void Start(TimerCallback timerCallback)
        {
            if (_stopWatch.IsRunning)
                _stopWatch.Reset();
            _stopWatch.Start();

            _timer.Elapsed += (sender, args) =>
            {
                timerCallback?.Invoke(this);
            };
            _timer.Start();
        }

        public void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }
        public void Stop()
        {
            _stopWatch.Stop();
            _timer.Stop();
        }
    }
}
