using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input.Custom;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading;
using System.Threading.Tasks;
using Hyper_Ship_Battle;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hyper_Ship_Battle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private const int GridSize = 10;

        //robot stuff begin
        private int x; //row
        private int y; //col
        private bool pastHit = false;
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


        //TODO: make better ai


        private const int NumberOfShips_p = 1;
        private const int NumberOfShips_r = 1;

        private Rectangle[,] rectangles_p = new Rectangle[GridSize, GridSize];
        private Rectangle[,] rectangles_r = new Rectangle[GridSize, GridSize];

        // Track the number of ships remaining
        private int shipsRemaining_p = NumberOfShips_p;
        private int shipsRemaining_r = NumberOfShips_r;
        private int[,] p_board = App.p_board;
        private int[,] r_board = App.r_board;

        private bool allowed;
        private bool turn;   // true - player fire / false - bot fire

        public Game()
        {
            this.InitializeComponent();
            InitializeGrid_p();
            InitializeGrid_r();
            allowed = true;
            turn = true;
        }

        private void InitializeGrid_p()
        {
            // Loop through each cell in the grid
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    if (p_board[row, col] == 0)
                    {
                        rect.Fill = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        rect.Fill = new SolidColorBrush(Colors.DarkGray);
                    }
                    rect.Stroke = new SolidColorBrush(Colors.Gray);

                    // Add the rectangle to the grid
                    GameGrid.Children.Add(rect);
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);

                    // Store the rectangle in the array for future reference
                    rectangles_p[row, col] = rect;
                    rect.Visibility = Visibility.Collapsed;
                }
            };
        }

        private void InitializeGrid_r()
        {
            // Loop through each cell in the grid
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.White);
                    switch (r_board[row, col])
                    {
                        case 0:
                            rect.PointerPressed += Rect_OnPointerPressed_Miss;
                            break;
                        case 2:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_2;
                            break;
                        case 3:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_3;
                            break;
                        case 4:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_4;
                            break;
                        case 5:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_5;
                            break;
                        case 6:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_6;
                            break;
                        case 7:
                            rect.PointerPressed += Rect_OnPointerPressed_Hit_7;
                            break;
                    }
                    rect.Stroke = new SolidColorBrush(Colors.Gray);

                    GameGrid.Children.Add(rect);
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    rect.Visibility = Visibility.Visible;

                    rectangles_r[row, col] = rect;
                }
            }
        }

        private void Rect_OnPointerPressed_Miss(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.LightGray);
                allowed = false;
                continue_f();
            }
        }
        private void Rect_OnPointerPressed_Hit_2(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.Red);
                r_br2--;
                if (r_br2 == 0)
                {
                    sink(2, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_3(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.Red);
                r_br3--;
                if (r_br3 == 0)
                {
                    sink(3, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_4(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.Red);
                r_br4--;
                if (r_br4 == 0)
                {
                    sink(4, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_5(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.Red);
                r_br5--;
                if (r_br5 == 0)
                {
                    sink(5, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_6(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.Red);
                r_br6--;
                if (r_br6 == 0)
                {
                    sink(6, rectangles_r, App.r_board);
                }
            }
        }
        private void Rect_OnPointerPressed_Hit_7(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White && allowed)
            {
                rect.Fill = new SolidColorBrush(Colors.Red);
                r_br7--;
                if (r_br7 == 0)
                {
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
                        r[i, j].Fill = new SolidColorBrush(Colors.DarkRed);
                    }
                }
            }
            if (shipsRemaining_p == 0)
            {
                //loss
            }
            if (shipsRemaining_r == 0)
            {
                //win
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


        private void BotTurn()
        {
            while (true)
            {
                //Robot hit
                Random rx = new Random();
                Random ry = new Random();
                x = rx.Next(10);
                y = ry.Next(10);

                Rectangle rect = rectangles_p[x, y];

                if (BotShot())
                    break;

            }
            continue_f();
            return;
        }

        private bool BotShot()
        {
            Rectangle rect = rectangles_p[x, y];
            if ((rect.Fill as SolidColorBrush)?.Color != Colors.White)
                return false;
            switch (p_board[x, y])
            {
                case 0:
                    pastHit = false;
                    rect.Fill = new SolidColorBrush(Colors.LightGray);
                    break;

                case 2:
                    rect.Fill = new SolidColorBrush(Colors.Red);
                    p_br2--;
                    if (p_br2 == 0)
                    {
                        sink(2, rectangles_p, App.p_board);
                        break;
                    }
                    pastHit = true;
                    break;
                case 3:

                    rect.Fill = new SolidColorBrush(Colors.Red);
                    p_br3--;
                    if (p_br3 == 0)
                    {
                        sink(3, rectangles_p, App.p_board);
                        break;
                    }
                    pastHit = true;
                    break;
                case 4:

                    rect.Fill = new SolidColorBrush(Colors.Red);
                    p_br4--;
                    if (p_br4 == 0)
                    {
                        sink(4, rectangles_p, App.p_board);
                        break;
                    }
                    pastHit = true;
                    break;
                case 5:

                    rect.Fill = new SolidColorBrush(Colors.Red);
                    p_br5--;
                    if (p_br5 == 0)
                    {
                        sink(5, rectangles_p, App.p_board);
                        pastHit = false;
                        break;
                    }
                    pastHit = true;
                    break;
                case 6:
                    rect.Fill = new SolidColorBrush(Colors.Red);
                    p_br6--;
                    if (p_br6 == 0)
                    {
                        sink(6, rectangles_p, App.p_board);
                        pastHit = false;
                        break;
                    }
                    pastHit = true;
                    break;
                case 7:
                    rect.Fill = new SolidColorBrush(Colors.Red);
                    p_br7--;
                    if (p_br7 == 0)
                    {
                        sink(7, rectangles_p, App.p_board);
                        pastHit = false;
                        break;
                    }
                    pastHit = true;
                    break;
            }
            return true;
        }

        private async void continue_f()
        {
            if (!allowed && turn)
            {
                await Task.Delay(1000);
                turn_sw();
                await Task.Delay(1000);
                BotTurn();
            }
            else if (!turn && pastHit)
            {
                await Task.Delay(1000);
                BotTurn();
            }
            else if (!turn && !pastHit)
            {
                await Task.Delay(1000);
                turn_sw();
                allowed = true;
            }
        }
    }
}
