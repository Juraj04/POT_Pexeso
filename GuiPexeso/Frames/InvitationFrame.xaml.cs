using System.Windows;
using ServiceLibraryPexeso;

namespace GuiPexeso.Frames
{
    /// <summary>
    /// Interaction logic for InvitationFrame.xaml
    /// </summary>
    public partial class InvitationFrame
    {
        public string HostNick { get; set; }
        public CardTypes CardTypes { get; set; }


        private const string InviteFrom = "Invite from: ";
        private const string GameType = "Game type: ";

        public InvitationFrame()
        {
            InitializeComponent();

            LabelName.Content = InviteFrom + HostNick;
            LabelGame.Content = GameType + CardTypes;
        }

        public InvitationFrame(string hostNick, CardTypes cards)
        {
            InitializeComponent();
            LabelName.Content = InviteFrom + hostNick;
            LabelGame.Content = GameType + cards;
        }

        private void BtnAccept_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnDecline_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
