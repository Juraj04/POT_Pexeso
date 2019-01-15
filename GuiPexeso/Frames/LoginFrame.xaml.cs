using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ServiceLibraryPexeso.Interfaces;

namespace GuiPexeso.Frames
{
    /// <summary>
    /// Interaction logic for LoginFrame.xaml
    /// </summary>
    public partial class LoginFrame
    {
        public string NickName { get; set; }
        private ChannelFactory<ILoginService> ChannelFactory { get; }

        public ILoginService Login { get; set; }

        public LoginFrame()
        {
            InitializeComponent();
            DataContext = this;
            ChannelFactory = new ChannelFactory<ILoginService>("LoginService");
            Login = ChannelFactory.CreateChannel();

        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckNotEmpty())
            {
                if (Login.LoginPlayer(TbNick.Text.Trim()))
                {
                    DialogResult = true;
                    Close();
                    return;
                }
                else
                {
                    MessageBox.Show($"Nick {TbNick.Text.Trim()} does not exist or is already in game", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
            }

            MessageBox.Show("Nick must be entered", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private bool CheckNotEmpty()
        {
            return TbNick.Text.Length > 0;
        }

        private void BtnRegister_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckNotEmpty())
            {
                if (Login.RegisterPlayer(TbNick.Text.Trim()))
                {
                    DialogResult = true;
                    Close();
                    return;
                }
                else
                {
                    MessageBox.Show($"Nick {TbNick.Text.Trim()} already exist", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            MessageBox.Show("Nick must be entered", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[a-zA-Z0-9-]+");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
