using Hyper_Ship_Battle.LAN_Multiplayer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hyper_Ship_Battle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LANClient : Page
    {
        private const int GridSize = 10;

        //counters
        private int p_br2 = 2;
        private int p_br3 = 3;
        private int p_br4 = 4;
        private int p_br5 = 5;
        private int p_br6 = 2;
        private int p_br7 = 3;

        private int r_br2 = 2;
        private int r_br3 = 3;
        private int r_br4 = 4;
        private int r_br5 = 5;
        private int r_br6 = 2;
        private int r_br7 = 3;

        private const int NumberOfShips_p = 5;
        private const int NumberOfShips_r = 5;

        private Rectangle[,] rectangles_p = new Rectangle[GridSize, GridSize];
        private Rectangle[,] rectangles_r = new Rectangle[GridSize, GridSize];

        // Track the number of ships remaining
        private int shipsRemaining_p = NumberOfShips_p;
        private int shipsRemaining_r = NumberOfShips_r;
        private int[,] p_board = App.p_board;
        private int[,] r_board = App.r_board;

        private bool allowed;
        private bool turn;   // true - player fire / false - bot fire
        private int endgame = 0;

        private TcpClient client;

        public LANClient()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            openned();
        }

        private void openned()
        {
            Start();
            InitializeGrid_p();
            InitializeGrid_r();
            hideEnd();
            client = ClientManager.Instance.GetClient();
            client.MessageReceived += MessageReceived;
            missSound.Source = new Uri("ms-appx:///Assets/Sounds/water-splash-46402.mp3");
            hitSound.Source = new Uri("ms-appx:///Assets/Sounds/short-explosion.mp3");
            backgroundMusic.Source = new Uri("ms-appx:///Assets/Sounds/waves-53479.mp3");
            winSound.Source = new Uri("ms-appx:///Assets/Sounds/success-fanfare-trumpets-6185.mp3");
            losSound.Source = new Uri("ms-appx:///Assets/Sounds/videogame-death-sound-43894.mp3");
            backgroundMusic.Play();
        }

        private void Start()
        {
            allowed = false;
            turn = false;
            endgame = 0;

            p_br2 = 0;
            p_br3 = 0;
            p_br4 = 0;
            p_br5 = 0;
            p_br6 = 0;
            p_br7 = 0;

            r_br2 = 2;
            r_br3 = 3;
            r_br4 = 4;
            r_br5 = 5;
            r_br6 = 2;
            r_br7 = 3;

            shipsRemaining_p = NumberOfShips_p;
            shipsRemaining_r = NumberOfShips_r;

            p_board = App.p_board;
            r_board = App.r_board;
        }

        private void InitializeGrid_p()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    if (p_board[row, col] == 0)
                    {
                        rect.Fill = new SolidColorBrush(App.emptyColor);
                    }
                    else
                    {
                        rect.Fill = new SolidColorBrush(App.shipColor);
                    }
                    rect.Stroke = new SolidColorBrush(App.strokeColor);

                    GameGrid.Children.Add(rect);
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);

                    rectangles_p[row, col] = rect;
                    rect.Visibility = Visibility.Visible;
                }
            }
        }
        private void InitializeGrid_r()
        {
            // Loop through each cell in the grid
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(App.emptyColor);
                    switch (r_board[row, col])
                    {
                        case 0:
                            rect.PointerPressed += Rect_OnPointerPressed_Miss;
                            //rect.Fill = new SolidColorBrush(Colors.Yellow);
                            break;
                        case 2:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_2;
                            //rect.Fill = new SolidColorBrush(Colors.Orange);
                            break;
                        case 3:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_3;
                            //rect.Fill = new SolidColorBrush(App.hitColor);
                            break;
                        case 4:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_4;
                            //rect.Fill = new SolidColorBrush(Colors.Purple);
                            break;
                        case 5:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_5;
                            //rect.Fill = new SolidColorBrush(Colors.Blue);
                            break;
                        case 6:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_6;
                            //rect.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 7:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_7;
                            //rect.Fill = new SolidColorBrush(Colors.Black);
                            break;
                    }
                    rect.Stroke = new SolidColorBrush(App.strokeColor);

                    GameGrid.Children.Add(rect);
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    rect.Visibility = Visibility.Collapsed;

                    rectangles_r[row, col] = rect;
                }
            }
        }


        // on each send data for shots
        private void Rect_OnPointerPressed_Miss(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                missSound.Play();
                rect.Fill = new SolidColorBrush(App.missColor);
                allowed = false;

                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());

                continue_f();
            }
        }
        private void Rect_OnPointerPressed_Hit_2(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                r_br2--;
                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());
                if (r_br2 == 0)
                {
                    shipsRemaining_r--;
                    sink(2, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_3(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                r_br3--;
                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());
                if (r_br3 == 0)
                {
                    shipsRemaining_r--;
                    sink(3, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_4(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                r_br4--;
                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());
                if (r_br4 == 0)
                {
                    shipsRemaining_r--;
                    sink(4, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_5(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                r_br5--;
                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());
                if (r_br5 == 0)
                {
                    shipsRemaining_r--;
                    sink(5, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_6(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                r_br6--;
                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());
                if (r_br6 == 0)
                {
                    shipsRemaining_r--;
                    sink(6, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_7(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == App.emptyColor && allowed)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                r_br7--;
                int x = Grid.GetRow(rect);
                int y = Grid.GetColumn(rect);
                client.Send(x.ToString() + y.ToString());
                if (r_br7 == 0)
                {
                    shipsRemaining_r--;
                    sink(7, rectangles_r, App.r_board);
                }
            }
        }
        private void sink(int num, Rectangle[,] r, int[,] p)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (p[i, j] == num)
                    {
                        r[i, j].Fill = new SolidColorBrush(App.sinkColor);
                    }
                }
            }
        }

        private void opponentShot(int x, int y)
        {
            Rectangle rect = rectangles_p[x, y];
            if (p_board[x, y] != 0)
            {
                hitSound.Play();
                rect.Fill = new SolidColorBrush(App.hitColor);
                switch (p_board[x, y])
                {
                    case 2:
                        p_br2++;
                        if (p_br2 == 2)
                        {
                            shipsRemaining_p--;
                            sink(p_board[x, y], rectangles_p, App.p_board);
                        }
                        break;
                    case 3:
                        p_br3++;
                        if (p_br3 == 3)
                        {
                            shipsRemaining_p--;
                            sink(p_board[x, y], rectangles_p, App.p_board);
                        }
                        break;
                    case 4:
                        p_br4++;
                        if (p_br4 == 4)
                        {
                            shipsRemaining_p--;
                            sink(p_board[x, y], rectangles_p, App.p_board);
                        }
                        break;
                    case 5:
                        p_br5++;
                        if (p_br5 == 5)
                        {
                            shipsRemaining_p--;
                            sink(p_board[x, y], rectangles_p, App.p_board);
                        }
                        break;
                    case 6:
                        p_br6++;
                        if (p_br6 == 2)
                        {
                            shipsRemaining_p--;
                            sink(p_board[x, y], rectangles_p, App.p_board);
                        }
                        break;
                    case 7:
                        p_br7++;
                        if (p_br7 == 3)
                        {
                            shipsRemaining_p--;
                            sink(p_board[x, y], rectangles_p, App.p_board);
                        }
                        break;
                }
            }
            else
            {
                missSound.Play();
                rect.Fill = new SolidColorBrush(App.missColor);
            }
        }

        private void turn_sw()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (turn)
                    {
                        rectangles_r[row, col].Visibility = Visibility.Collapsed;
                        rectangles_p[row, col].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        rectangles_p[row, col].Visibility = Visibility.Collapsed;
                        rectangles_r[row, col].Visibility = Visibility.Visible;
                    }
                }
            }
            turn = !turn;
        }

        private async void continue_f()
        {
            if (!allowed && turn && endgame == 0)
            {
                client.Send("sw");
                await Task.Delay(1000);
                turn_sw();
                await Task.Delay(1000);
            }
        }

        // get message / decode
        private async void MessageReceived(object sender, string message)
        {
            if (message == "sw")
            {
                await Task.Delay(1000);
                turn_sw();
                allowed = true;
            }
            else if (message == "win")
            {
                allowed = false;
                win();
            }
            else if (message == "loss")
            {
                allowed = false;
                loss();
            }
            else if (message[0]=='y')
            {
                // turn, host board hits, client board hits

                bool syncTurn = (message[1] == '1') ? true : false;
                if (turn!=syncTurn)
                {
                    turn_sw();
                    turn = syncTurn;
                }

                r_br2 = message[2] - '0';
                r_br2 = message[3] - '0';
                r_br3 = message[4] - '0';
                r_br4 = message[5] - '0';
                r_br6 = message[6] - '0';
                r_br7 = message[7] - '0';

                p_br2 = message[8] - '0';
                p_br2 = message[9] - '0';
                p_br3 = message[10] - '0';
                p_br4 = message[11] - '0';
                p_br6 = message[12] - '0';
                p_br7 = message[13] - '0';

                int count = 14;
                for (int i = 0;i<10;i++)
                {
                    for (int j = 0;j<10;j++)
                    {
                        if (message[count] == '0')
                        {
                            rectangles_r[i, j].Fill = new SolidColorBrush(App.emptyColor);
                        }
                        else if (message[count] == '1')
                        {
                            rectangles_r[i, j].Fill = new SolidColorBrush(App.missColor);
                        }
                        else if(message[count] == '2')
                        {
                            rectangles_r[i, j].Fill = new SolidColorBrush(App.hitColor);
                        }
                        else if(message[count] == '3')
                        {
                            rectangles_r[i, j].Fill = new SolidColorBrush(App.sinkColor);
                        }
                        count++;

                        if (message[count] == '0')
                        {
                            rectangles_p[i, j].Fill = new SolidColorBrush(App.emptyColor);
                        }
                        else if (message[count] == '1')
                        {
                            rectangles_p[i, j].Fill = new SolidColorBrush(App.missColor);
                        }
                        else if (message[count] == '2')
                        {
                            rectangles_p[i, j].Fill = new SolidColorBrush(App.hitColor);
                        }
                        else if (message[count] == '3')
                        {
                            rectangles_p[i, j].Fill = new SolidColorBrush(App.sinkColor);
                        }
                        else if ((message[count] == '4'))
                        {
                            rectangles_p[i, j].Fill = new SolidColorBrush(App.shipColor);
                        }
                        count++;
                    }
                }
            }
            else
            {
                int x = 0, y = 0;

                x = message[0] - '0';
                y = message[1] - '0';

                opponentShot(x, y);
            }
        }

        private void hideEnd()
        {
            endstatus.Visibility = Visibility.Collapsed;
            home_b.Visibility = Visibility.Collapsed;
            myCanvas.Visibility = Visibility.Collapsed;
            myCanvas.Visibility = Visibility.Collapsed;
            myCanvas.Opacity = 0;
        }
        private void win()
        {
            endstatus.Text = "Mission passed";
            endstatus.Visibility = Visibility.Visible;
            home_b.Visibility = Visibility.Visible;
            myCanvas.Visibility = Visibility.Visible;
            myCanvas.Background = new SolidColorBrush(App.shipColor);
            myCanvas.Background.Opacity = 90;
            myCanvas.Opacity = 5;
            client.MessageReceived -= MessageReceived;
            winSound.Play();
        }
        private void loss()
        {
            endstatus.Text = "Mission failled";
            endstatus.Visibility = Visibility.Visible;
            home_b.Visibility = Visibility.Visible;
            myCanvas.Visibility = Visibility.Visible;
            myCanvas.Background = new SolidColorBrush(App.shipColor);
            myCanvas.Background.Opacity = 90;
            myCanvas.Opacity = 5;
            client.MessageReceived -= MessageReceived;
            losSound.Play();
        }

        private void home_b_Click(object sender, RoutedEventArgs e)
        {
            hideEnd();
            App.board0();
            App.resetSetup = true;
            client.Disconnect();
            ClientManager.Instance.ResetClient();
            Frame.Navigate(typeof(MainPage));
        }
        private void exit_click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
            Application.Current.Exit();
        }

        private void sync_b_Click(object sender, RoutedEventArgs e)
        {
            client.Send("y");
        }
    }
}
