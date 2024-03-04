using Hyper_Ship_Battle.LAN_Multiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Hyper_Ship_Battle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Popup stringInputPopup;

        public MainPage()
        {
            this.InitializeComponent();
            backgroundMusic.Source = new Uri("ms-appx:///Assets/Sounds/ukulele-long.mp3");
            backgroundMusic.Play();
            backgroundMusic2.Source = new Uri("ms-appx:///Assets/Sounds/waves-53479.mp3");
            backgroundMusic2.Play();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            /*
            backgroundMusic.Source = new Uri("ms-appx:///Assets/Sounds/ukulele-8488.mp3");
            backgroundMusic.Play();
            */
        }

        private void PlayB_Click(object sender, RoutedEventArgs e)
        {
            App.resetSetup = true;
            backgroundMusic.Stop();
            Frame.Navigate(typeof(Setup));
        }

        private void exit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void LANB_Click(object sender, RoutedEventArgs e)
        {
            var dialogOrientation = new ContentDialog
            {
                Title = "LAN Multiplayer",
                Content = "Create or Join!",
                PrimaryButtonText = "Create room",
                SecondaryButtonText = "Join room"
            };
            dialogOrientation.CloseButtonText = "Cancle";
            ContentDialogResult resultName = await dialogOrientation.ShowAsync();

            if (resultName== ContentDialogResult.Primary)
            {
                string roomName;

                ContentDialog inputDialog = new ContentDialog
                {
                    Title = "Room name",
                    Content = new TextBox(),
                    PrimaryButtonText = "OK",
                    CloseButtonText = "Cancel"
                };

                ContentDialogResult result = await inputDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    TextBox textBox = inputDialog.Content as TextBox;
                    roomName = textBox.Text;
                    TcpServer host = ServerManager.Instance.GetHost();
                    host.StartServer(roomName, App.PORT_NUMBER);
                    Frame.Navigate(typeof(Setup));
                }
            }
            else if (resultName == ContentDialogResult.Secondary)
            {
                backgroundMusic.Stop();
                Frame.Navigate(typeof(RoomSelect));
            }
            else if (resultName == ContentDialogResult.None)
            {
                // cancel
            }
        }
    }
}
