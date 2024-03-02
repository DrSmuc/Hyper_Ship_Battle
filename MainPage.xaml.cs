using Hyper_Ship_Battle.LAN_Multiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            stringInputPopup = null;
        }

        private void PlayB_Click(object sender, RoutedEventArgs e)
        {
            App.resetSetup = true;
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
            ContentDialogResult resultName = await dialogOrientation.ShowAsync();
            bool createRoom = (resultName == ContentDialogResult.Primary);
            if (createRoom)
            {
                stringInputPopup = new Popup();
                stringInputPopup.HorizontalOffset = (Window.Current.Bounds.Width - 300) / 2;
                stringInputPopup.VerticalOffset = (Window.Current.Bounds.Height - 200) / 2;
                stringInputPopup.Child = CreatePopupInput();
                stringInputPopup.IsOpen = true;
            }
            else
            {
                Frame.Navigate(typeof(RoomSelect));
            }
        }

        private StackPanel CreatePopupInput()
        {

            StackPanel panel = new StackPanel();
            TextBox inputText = new TextBox();
            Button continueButton = new Button();
            string roomName;

        inputText.PlaceholderText = "Enter name...";
            continueButton.Content = "Continue";
            continueButton.Click += (s, e) =>
            {
                roomName=inputText.Text;
                stringInputPopup.IsOpen = false;
                TcpServer host = ServerManager.Instance.GetHost();
                host.StartServer(roomName, App.PORT_NUMBER);
                Frame.Navigate(typeof(Setup));
            };
            panel.Children.Add(inputText);
            panel.Children.Add(continueButton);

            return panel;
        }
    }
}
