using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GuiPexeso.Annotations;
using GuiPexeso.Frames;
using ServiceLibraryPexeso;
using Button = System.Windows.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;

namespace GuiPexeso
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private GameClient _gameClient;
        private Dictionary<string, TextBox> _chatRooms;

        private Button[,] _playBoard;
        private bool _turn;

        public bool Turn
        {
            get => _turn;
            set
            {
                _turn = value;
                DisableEnablePlayBoard();
            }
        }

        private int _step;
        private BoardItem _firstFliped;
        private Grid _grid;
        private int _myScore;
        private int _opponentScore;
        private string _myNick;
        private string _opponentNick;
        private Result _myResult;
        private Result _opponentsResult;
        public int MyScore
        {
            get => _myScore;
            set
            {
                _myScore = value;
                OnPropertyChanged(nameof(MyScore));
            }
        }

        private int OpponentScore
        {
            get => _opponentScore;
            set
            {
                _opponentScore = value;
                OnPropertyChanged(nameof(OpponentScore));
            }
        }

        private string MyNickName
        {
            get => _myNick;
            set
            {
                _myNick = value;
                OnPropertyChanged(nameof(MyNickName));
            }
        }

        private string OpponentNickName
        {
            get => _opponentNick;
            set
            {
                _opponentNick = value;
                OnPropertyChanged(nameof(OpponentNickName));
            }
        }

        private const string Local = "Local \r\n";
        private const string Opponent = "Opponent \r\n";
        private const string Score = "Score: ";

        private LoginFrame _loginFrame;
        //METHODS FROM HERE---------------------------------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();
            _step = 0;
            _loginFrame = new LoginFrame();

            if (_loginFrame.ShowDialog() != true)
            {
                Close();
                return;
            }

            DataContext = this;
            _gameClient = new GameClient(_loginFrame.NickName);
            _gameClient.OnMessageReceived += ShowMessage;
            _gameClient.OnInvitationReceived += ProcessInvitation;
            _gameClient.OnCardsReceived += CreatePlayArea;
            _gameClient.OnFlippedReceived += ProcessOpponentsFlip;
            _gameClient.OnTurnReceived += ProcessOpponentsTurn;
            _gameClient.OnOpponentScoreReceived += ProcessOpponentsScore;
            _gameClient.OnGameFinished += ProcessGameFinished;
            _gameClient.OnForceFinishGame += ProcessForceGameFinished;


            MyNickName = _loginFrame.NickName;
            MyScore = 0;
            OpponentScore = 0;
            _chatRooms = new Dictionary<string, TextBox>();
            RefreshAvailablePlayers(_gameClient.GetAllOnlinePlayers());
            InitializeCombo();


            Closing += (sender, args) => FinishWork();

        }

        private void DisableEnablePlayBoard()
        {
            foreach (var btn in _playBoard)
            {
                btn.IsEnabled = Turn;
            }
        }

        private void FlipBoard()
        {
            foreach (var button in _playBoard)
            {
                if (button.DataContext is BoardItem item && !item.Found)
                    button.Content = item.DefaultChar;
            }
        }


        private void InitializeCombo()
        {
            var list = new[]
            {
                new CardTypes(3, 2), new CardTypes(4, 3), new CardTypes(4, 4), new CardTypes(5, 4), new CardTypes(6, 5),
                new CardTypes(6, 6), new CardTypes(7, 6), new CardTypes(8, 7), new CardTypes(8, 8)
            };
            foreach (var c in list)
            {
                ComboCards.Items.Add(c);
            }

            ComboCards.SelectedIndex = 1;
        }

        private void RefreshAvailablePlayers(List<string> nickNames)
        {
            LbPlayers.Items.Clear();
            foreach (var nickName in nickNames)
            {
                LbPlayers.Items.Add(nickName);
            }
        }

        private void FinishWork()
        {
            foreach (TabItem tabChatItem in TabChat.Items)
            {
                _gameClient.SendMessage("Disconnected", (string)tabChatItem.Header);
            }
            _gameClient.Disconnect();
            _loginFrame.Login.LogOut(MyNickName);
        }

        private void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshAvailablePlayers(_gameClient.GetAllOnlinePlayers());
        }

        private void BtnRandom_OnClick(object sender, RoutedEventArgs e)
        {
            BtnRefresh_OnClick(null, e); //refresh first

            var available = new List<string>();
            foreach (var item in LbPlayers.Items)
            {
                var text = (string)item;
                if (!text.StartsWith("#"))
                    available.Add(text);
            }

            if (available.Count == 0) return;
            var r = new Random();
            var n = r.Next(0, available.Count);
            var card = ComboCards.SelectedItem as CardTypes;
            _gameClient.SendInvitation(MyNickName, available[n], card);
        }

        private void BtnInvite_OnClick(object sender, RoutedEventArgs e)
        {
            var name = (LbPlayers.SelectedItem as string);
            if (name == null) return;
            var card = ComboCards.SelectedItem as CardTypes;
            _gameClient.SendInvitation(MyNickName, name, card);
        }

        private void BtnMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var name = (LbPlayers.SelectedItem as string);
            if (name == null) return;
            if (name.StartsWith("#"))
            {
                name = name.Remove(0, 1);  //lebo mozem...
            }
            if (_chatRooms.ContainsKey(name)) return;
            CreateNewChatTab(name);

        }

        private void CreateNewChatTab(string chatBuddy)
        {
            TabItem item = new TabItem() { Header = chatBuddy };
            TextBox tb = new TextBox();
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            tb.IsReadOnly = true;
            var grid = new Grid();
            grid.Children.Add(tb);
            item.Content = grid;
            _chatRooms.Add(chatBuddy, tb);
            TabChat.Items.Add(item);
            TabChat.SelectedItem = item;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnChatSend_OnClick(object sender, RoutedEventArgs e)
        {
            var item = TabChat.SelectedItem as TabItem;
            if (item == null) return;
            var box = _chatRooms[item.Header as string ?? throw new InvalidOperationException()];
            var addressee = (string)item.Header;
            var message = TbChatInput.Text.Trim();
            _gameClient.SendMessage(message, addressee);
            box.AppendText("My message: \r\n" + message + "\r\n");
            TbChatInput.Clear();
        }

        public void ShowMessage(string message, string sender)
        {
            if (!_chatRooms.ContainsKey(sender))
            {
                CreateNewChatTab(sender);
            }
            var box = _chatRooms[sender];
            box.AppendText(message);

            foreach (TabItem tabChatItem in TabChat.Items)
            {
                if ((tabChatItem.Header as string) == sender)
                {
                    TabChat.SelectedItem = tabChatItem;
                    break;
                }

            }
        }

        public void CreatePlayArea(char[][] cards, string opponentNick, bool turn)
        {
            OpponentNickName = opponentNick;
            _playBoard = new Button[cards.Length, cards[0].Length];

            BtnInvite.IsEnabled = false;
            BtnRandom.IsEnabled = false;

            _grid = new Grid();
            Thickness margin = _grid.Margin;
            margin.Left = 5;
            margin.Right = 5;
            margin.Top = 5;
            margin.Bottom = 5;
            _grid.Margin = margin;

            for (int i = 0; i < cards.Length; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < cards[0].Length; j++)
            {
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < cards.Length; i++)
            {

                for (int j = 0; j < cards[i].Length; j++)
                {
                    var btn = new Button();
                    var item = new BoardItem(i, j, btn, cards[i][j]);
                    btn.DataContext = item;
                    btn.Content = item.DefaultChar;
                    btn.Click += BtnBoardOnClick;
                    btn.Background = Brushes.Azure;
                    _playBoard[i, j] = btn;
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                    _grid.Children.Add(btn);
                }
            }
            Grid.SetRow(_grid, 1);
            Grid.SetColumn(_grid, 0);
            GridPlay.Children.Add(_grid);
            Turn = turn;
            UpdateScoreBoard();
        }

        private void BtnBoardOnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.DataContext as BoardItem;
            if (item == null) return;

            if ((btn.Content is char c) && c == item.CardChar) //if user clicked on flipped card
            {
                return;
            }

            btn.Content = item.CardChar;
            _step++;
            _gameClient.SendFlipped(item.RowIndex, item.ColIndex);
            if (_step % 2 == 1)
            {
                _firstFliped = item;
            }
            else
            {
                _gameClient.AddMove();
                if (_firstFliped.CardChar == item.CardChar)
                {
                    _firstFliped.Found = true;
                    item.Found = true;
                    _grid.Children.Remove(_firstFliped.Button);
                    _grid.Children.Remove(item.Button);
                    MyScore++;
                    _gameClient.SendScore(MyScore);
                    UpdateScoreBoard();
                    if (CheckIfGameFinished())
                    {
                        _gameClient.EndGame(MyScore, OpponentScore, _myResult, _opponentsResult);
                        ProcessGameFinished();
                    }
                    return;
                }
                Turn = false;
                _gameClient.SendTurn();
                WaitAndFlip(3000);

            }

        }

        private async void WaitAndFlip(int time)
        {
            await Task.Run(() => Thread.Sleep(time));
            FlipBoard();
        }

        public bool ProcessInvitation(CardTypes cards, string hostNick)
        {
            var invDialog = new InvitationFrame(hostNick, cards);

            return invDialog.ShowDialog() == true;
        }

        public void ProcessOpponentsFlip(int row, int column)
        {
            var btn = _playBoard[row, column];
            var item = btn.DataContext as BoardItem;
            if (item == null) return;
            btn.Content = item.CardChar;
            _step++;
            if (_step % 2 == 1)
            {
                _firstFliped = item;
            }
            else
            {
                if (_firstFliped.CardChar == item.CardChar)
                {
                    _firstFliped.Found = true;
                    item.Found = true;
                    _grid.Children.Remove(_firstFliped.Button);
                    _grid.Children.Remove(item.Button);
                }
            }
        }

        public void ProcessOpponentsTurn()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => Thread.Sleep(3000);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                FlipBoard();
                Turn = true;
            };
            worker.RunWorkerAsync();
        }

        public void ProcessOpponentsScore(int score)
        {
            OpponentScore = score;
            UpdateScoreBoard();
            CheckIfGameFinished();
        }

        private void UpdateScoreBoard()
        {
            var me = Local + MyNickName + "\r\n" + Score + MyScore + "\r\n";
            var him = Opponent + OpponentNickName + "\r\n" + Score + OpponentScore + "\r\n";

            TbMyScore.Text = me;
            TbOpponentsScore.Text = him;
        }

        public void ProcessGameFinished()
        {
            MyScore = 0;
            OpponentScore = 0;
            OpponentNickName = "";
            GridPlay.Children.Remove(_grid);
            _grid.Children.Clear();
            BtnInvite.IsEnabled = true;
            BtnRandom.IsEnabled = true;

        }

        public void ProcessForceGameFinished()
        {
            TbMyScore.AppendText("Time ran out, game finished");
            TbOpponentsScore.AppendText("Time ran out, game finished");
            Turn = false;
            ProcessGameFinished();
        }

        private bool CheckIfGameFinished()
        {
            var maxPoints = _playBoard.Length / 2;
            if (maxPoints == MyScore + OpponentScore)
            {
                if (MyScore == OpponentScore)
                {
                    TbMyScore.AppendText("Draw");
                    TbOpponentsScore.AppendText("Draw");
                    _myResult = Result.Draw;
                    _opponentsResult = Result.Draw;
                }
                else if (MyScore > OpponentScore)
                {
                    TbMyScore.AppendText("Win");
                    TbOpponentsScore.AppendText("Lose");
                    _myResult = Result.Win;
                    _opponentsResult = Result.Lose;
                }
                else
                {
                    TbMyScore.AppendText("Lose");
                    TbOpponentsScore.AppendText("Win");
                    _myResult = Result.Lose;
                    _opponentsResult = Result.Win;
                }

                return true;
            }

            return false;
        }
    }
}
