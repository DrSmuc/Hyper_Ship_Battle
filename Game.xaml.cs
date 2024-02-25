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
        private int pastHit = 0;
        private int direction = 0;
        private int firstHitX;
        private int firstHitY;
        private int lastHitX;
        private int lastHitY;
        //counters
        private int p_br2 = 0;
        private int p_br3 = 0;
        private int p_br4 = 0;
        private int p_br5 = 0;
        private int p_br6 = 0;
        private int p_br7 = 0;

        private int r_br2 = 0;
        private int r_br3 = 0;
        private int r_br4 = 0;
        private int r_br5 = 0;
        private int r_br6 = 0;
        private int r_br7 = 0;


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
        private bool passturn = false;
        private int endgame = 0;

        public Game()
        {
            this.InitializeComponent();
            InitializeGrid_p();
            InitializeGrid_r();
            allowed = true;
            turn = true;
            hideEnd();
        }

        private void InitializeGrid_p()
        {
            //testing
            App.p_board[4, 6] = 5;
            App.p_board[5, 6] = 5;
            App.p_board[6, 6] = 5;
            App.p_board[7, 6] = 5;
            App.p_board[8, 6] = 5;

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

                    //for testing
                    if (col < 5 || col > 7 || row < 3)
                    {
                        rect.Fill = new SolidColorBrush(Colors.LightGray);
                    }

                    // Add the rectangle to the grid
                    GameGrid.Children.Add(rect);
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);

                    // Store the rectangle in the array for future reference
                    rectangles_p[row, col] = rect;
                    rect.Visibility = Visibility.Collapsed;
                }
            }
            //testing
            rectangles_p[3, 6].Fill = new SolidColorBrush(Colors.LightGray);
            rectangles_p[9, 6].Fill = new SolidColorBrush(Colors.LightGray);
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
            if (shipsRemaining_r == 0)
            {
                //win
                endgame = 1;
            }
            if (shipsRemaining_p == 0)
            {
                //loss
                endgame = -1;
            }
        }

        private void BotTurn()
        {
        //Robot hit
        again:
            if (pastHit == 0)
            {
                Random rx = new Random();
                Random ry = new Random();
                x = rx.Next(10);
                y = ry.Next(10);
            }
            else
            {
            new_direction:
                if (direction == 0)
                {
                    Random rd = new Random();
                    direction = rd.Next(1, 5);
                }
                if (pastHit == 2)
                {
                    switch (direction)
                    {
                        case 1: //left
                            if (y == 0 || !((rectangles_p[x, y - 1].Fill as SolidColorBrush)?.Color == Colors.White || (rectangles_p[x, y - 1].Fill as SolidColorBrush)?.Color == Colors.DarkGray))
                            {
                                direction = 3;
                                pastHit = 1;
                                lastHitX = firstHitX;
                                lastHitY = firstHitY;
                            }
                            break;
                        case 2: //up
                            if (x == 0 || !((rectangles_p[x - 1, y].Fill as SolidColorBrush)?.Color == Colors.White || (rectangles_p[x - 1, y].Fill as SolidColorBrush)?.Color == Colors.DarkGray))
                            {
                                direction = 4;
                                pastHit = 1;
                                lastHitX = firstHitX;
                                lastHitY = firstHitY;
                            }
                            break;
                        case 3: //right
                            if (y == 9 || !((rectangles_p[x, y + 1].Fill as SolidColorBrush)?.Color == Colors.White || (rectangles_p[x, y + 1].Fill as SolidColorBrush)?.Color == Colors.DarkGray))
                            {
                                direction = 1;
                                pastHit = 1;
                                lastHitX = firstHitX;
                                lastHitY = firstHitY;
                            }
                            break;
                        case 4: //down !!!not going through when needed
                            if (x == 9 || !((rectangles_p[x + 1, y].Fill as SolidColorBrush)?.Color == Colors.White || (rectangles_p[x + 1, y].Fill as SolidColorBrush)?.Color == Colors.DarkGray))
                            {
                                direction = 2;
                                pastHit = 1;
                                lastHitX = firstHitX;
                                lastHitY = firstHitY;
                            }
                            break;
                    }
                }
                switch (direction)
                {
                    case 1: //left
                        if (lastHitY > 0)
                        {
                            x = lastHitX;
                            y = lastHitY - 1;
                        }
                        else
                        {
                            direction = 0;
                            goto new_direction;
                        }
                        break;
                    case 2: //up
                        if (lastHitX > 0)
                        {
                            x = lastHitX - 1;
                            y = lastHitY;
                        }
                        else
                        {
                            direction = 0;
                            goto new_direction;
                        }
                        break;
                    case 3: //right
                        if (lastHitY < 9)
                        {
                            x = lastHitX;
                            y = lastHitY + 1;
                        }
                        else
                        {
                            direction = 0;
                            goto new_direction;
                        }
                        break;
                    case 4: //down
                        if (lastHitX < 9)
                        {
                            x = lastHitX + 1;
                            y = lastHitY;
                        }
                        else
                        {
                            direction = 0;
                            goto new_direction;
                        }
                        break;
                }
            }

            Rectangle rect = rectangles_p[x, y];
            if ((rect.Fill as SolidColorBrush)?.Color == Colors.White || (rect.Fill as SolidColorBrush)?.Color == Colors.DarkGray)
            {
                if (p_board[x, y] != 0)
                {
                    lastHitX = x;
                    lastHitY = y;
                    if (pastHit == 0)
                    {
                        firstHitX = x;
                        firstHitY = y;
                    }
                    rect.Fill = new SolidColorBrush(Colors.Red);
                    switch (p_board[x, y])
                    {
                        case 2:
                            p_br2++;
                            if (p_br2 < 2)
                            {
                                pastHit = 1;
                            }
                            else
                            {
                                pastHit = 0;
                                direction = 0;
                                shipsRemaining_p--;
                                sink(p_board[x, y], rectangles_p, App.p_board);
                            }
                            break;
                        case 3:
                            p_br3++;
                            if (p_br3 < 3)
                            {
                                if (p_br3 == 2)
                                    pastHit = 2;
                                else
                                    pastHit = 1;
                            }
                            else
                            {
                                pastHit = 0;
                                direction = 0;
                                shipsRemaining_p--;
                                sink(p_board[x, y], rectangles_p, App.p_board);
                            }
                            break;
                        case 4:
                            p_br4++;
                            if (p_br4 < 4)
                            {
                                if (p_br4 > 1)
                                    pastHit = 2;
                                else
                                    pastHit = 1;
                            }
                            else
                            {
                                pastHit = 0;
                                direction = 0;
                                shipsRemaining_p--;
                                sink(p_board[x, y], rectangles_p, App.p_board);
                            }
                            break;
                        case 5:
                            p_br5++;
                            if (p_br5 < 5)
                            {
                                if (p_br5 > 1)
                                    pastHit = 2;
                                else
                                    pastHit = 1;
                            }
                            else
                            {
                                pastHit = 0;
                                direction = 0;
                                shipsRemaining_p--;
                                sink(p_board[x, y], rectangles_p, App.p_board);
                            }
                            break;
                        case 6:
                            p_br6++;
                            if (p_br6 < 2)
                            {
                                pastHit = 1;
                            }
                            else
                            {
                                pastHit = 0;
                                direction = 0;
                                shipsRemaining_p--;
                                sink(p_board[x, y], rectangles_p, App.p_board);
                            }
                            break;
                        case 7:
                            p_br7++;
                            if (p_br7 < 3)
                            {
                                if (p_br7 == 2)
                                    pastHit = 2;
                                else
                                    pastHit = 1;
                            }
                            else
                            {
                                pastHit = 0;
                                direction = 0;
                                shipsRemaining_p--;
                                sink(p_board[x, y], rectangles_p, App.p_board);
                            }
                            break;
                    }
                    passturn = false;
                }
                else
                {
                    rect.Fill = new SolidColorBrush(Colors.LightGray);
                    if (pastHit == 2)
                    {
                        if (direction == 1)
                            direction = 3;
                        else if (direction == 2)
                            direction = 4;
                        else if (direction == 3)
                            direction = 1;
                        else if (direction == 4)
                            direction = 2;
                        lastHitX = firstHitX;
                        lastHitY = firstHitY;
                        pastHit = 1;
                    }
                    else
                        direction = 0;
                    passturn = true;
                }
            }
            else
            {
                direction = 0;
                goto again;
            }
            continue_f();
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
                await Task.Delay(1000);
                turn_sw();
                await Task.Delay(1000);
                BotTurn();
            }
            else if (!turn && passturn && endgame == 0)
            {
                await Task.Delay(1000);
                turn_sw();
                allowed = true;
                passturn = false;
            }
            else if (!turn && !passturn && endgame == 0)
            {
                await Task.Delay(1000);
                BotTurn();
            }
            else if (endgame == 1)
            {
                await Task.Delay(1000);
                win();
            }
            else if (endgame == -1)
            {
                await Task.Delay(1000);
                loss();
            }
        }

        private void hideEnd()
        {
            endstatus.Visibility = Visibility.Collapsed;
            rematch_b.Visibility = Visibility.Collapsed;
            home_b.Visibility = Visibility.Collapsed;
            myCanvas.Visibility = Visibility.Collapsed;
            myCanvas.Visibility = Visibility.Collapsed;
            myCanvas.Opacity = 0;
        }
        private void win()
        {
            endstatus.Text = "Mission passed";
            endstatus.Visibility = Visibility.Visible;
            rematch_b.Visibility = Visibility.Visible;
            home_b.Visibility = Visibility.Visible;
            myCanvas.Visibility = Visibility.Visible;
            myCanvas.Background = new SolidColorBrush(Colors.DarkGray);
            myCanvas.Background.Opacity = 90;
            myCanvas.Opacity = 5;
        }
        private void loss()
        {
            endstatus.Text = "Mission failled";
            endstatus.Visibility = Visibility.Visible;
            rematch_b.Visibility = Visibility.Visible;
            home_b.Visibility = Visibility.Visible;
            myCanvas.Visibility = Visibility.Visible;
            myCanvas.Background = new SolidColorBrush(Colors.DarkGray);
            myCanvas.Background.Opacity = 90;
            myCanvas.Opacity = 5;
        }

        private void rematch_b_Click(object sender, RoutedEventArgs e)
        {
            hideEnd();
            Frame.Navigate(typeof(MainPage));   //treba na setup
        }
        private void home_b_Click(object sender, RoutedEventArgs e)
        {
            hideEnd();
            Frame.Navigate(typeof(MainPage));
        }
    }
}
