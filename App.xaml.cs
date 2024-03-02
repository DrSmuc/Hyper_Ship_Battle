using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using System.Data.Common;
using Windows.Security.Authentication.Identity.Core;
using Windows.Media.Streaming.Adaptive;

namespace Hyper_Ship_Battle
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        public static int[,] p_board = new int[10, 10];
        public static int[,] r_board = new int[10, 10];
        public static bool resetSetup = false;
        public static bool readySetup = false;

        public static Color emptyColor = Colors.DeepSkyBlue;
        public static Color missColor = Colors.LightSkyBlue;
        public static Color shipColor = Colors.DarkGray;
        public static Color strokeColor = Colors.Gray;
        public static Color hitColor = Colors.Red;
        public static Color sinkColor = Colors.DarkRed;

        private static int port_number = 1337;
        public static bool serverActive;
        public static bool clientActive;

        public static bool ready_opponent = false;

        public static bool hostReceivedSetupSet;
        public static bool hostReceivedGameSet;
        public static bool clientReceivedSetupSet;
        public static bool clientReceivedGameSet;

        public static int PORT_NUMBER
        {
            get { return port_number; }
        }

        /*private static string emptyColorHex = "#FF0000";
        public static Color emptyColor = Color.FromArgb(
            byte.Parse(emptyColorHex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(emptyColorHex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(emptyColorHex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(emptyColorHex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));

        private static string shipColorHex = "#FF0000";
        public static Color shipColor = Color.FromArgb(
            byte.Parse(shipColorHex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(shipColorHex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(shipColorHex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(shipColorHex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));

        private static string missColorHex = "#FF0000";
        public static Color missColor = Color.FromArgb(
            byte.Parse(missColorHex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(missColorHex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(missColorHex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(missColorHex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));
        
        private static string strokeColorHex = "#FF0000";
        public static Color strokeColor = Color.FromArgb(
            byte.Parse(strokeColorHex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(strokeColorHex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(strokeColorHex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
            byte.Parse(strokeColorHex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber));
        */

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            board0();
            serverActive = false;
            clientActive = false;
        }

        public static void board0()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    p_board[i, j] = 0;
                    r_board[i, j] = 0;
                }
            }
        }

        public static void r_board0()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    r_board[i, j] = 0;
        }

        public static void p_board0()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    r_board[i, j] = 0;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
