using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace angelica_puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player player;

        public MainWindow()
        {
            InitializeComponent();

            ImportSettings();

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;
            myDoubleAnimation.To = 0.0;
        }

        private void ImportSettings()
        {
            this.player = Settings.GetPlayer();
        }

        private void OnStartOptionClicked(object sender, RoutedEventArgs e)
        {
            if(this.player == null)
            {
                MessageBoxResult res = MessageBox.Show("It's seems that you are not regitsered with any player\nDo you want to play as guest?",
                    "Player Settings",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Asterisk,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.ServiceNotification);

                if(res.Equals(MessageBoxResult.Yes))
                {
                    GameWindow gameWindow = new GameWindow(this);
                    App.Current.MainWindow = gameWindow;
                    this.Hide();
                    gameWindow.Show();
                }
                else
                {

                }
            } else
            {
                try
                {
                    GameWindow gameWindow = new GameWindow(this);
                    App.Current.MainWindow = gameWindow;
                    this.Hide();
                    gameWindow.Show();
                }catch (Exception ex)
                {
                    throw new Exception("tryCreateGameWindow", ex);
                }
            }
        }

        private void OnQuitOptionClicked(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void OnSettingsOptionClicked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
