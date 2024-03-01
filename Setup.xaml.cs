using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Npgsql;
using Hyper_Ship_Battle.LAN_Multiplayer;
using System.Collections.Specialized;

namespace Hyper_Ship_Battle
{
    public sealed partial class Setup : Page
    {
        private Button[,] gridButtons;
        private const int Rows = 10;
        private const int Columns = 10;
        private bool placingShip = false;
        private int shipLength;
        private int numberOfShipsPlaced = 0;
        private int shipsOfLength2Placed = 0;
        private int shipsOfLength3Placed = 0;
        private int shipsOfLength4Placed = 0;

        private bool b6 = false;
        private bool b7 = false;

        private bool ready = false;
        private bool ready_opponent = false;

        private TcpServer host;

        public Setup()
        {
            this.InitializeComponent();
            openned();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.resetSetup)
            {
                openned();
                App.resetSetup = false;
            }
            else if (App.readySetup)
            {
                ready_f();
            }
            if (App.serverActive)
            {
                host = ServerManager.Instance.GetHost();
                host.MessageReceived += opponent_ready;
            }
            else if (App.clientActive)
            {
                host.MessageReceived += clientStartGame;
            }
        }

        private void openned()
        {
            InitializeGrid();
            ready = false;
            continue_b.Opacity = 60;
            savepreset_b.Opacity = 60;
            numberOfShipsPlaced = 0;
            shipsOfLength2Placed = 0;
            shipsOfLength3Placed = 0;
            shipsOfLength4Placed = 0;
            b6 = false;
            b7 = false;
            ready = false;
            placingShip = false;
        }

        private void DeleteAllChildren(Panel parentPanel)
        {
            parentPanel.Children.Clear();
        }

        // grid s gumbom
        private void InitializeGrid()
        {
            if (ready)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        DeleteAllChildren(GameGrid);
                    }
                }
            }

            gridButtons = new Button[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Button button = new Button();
                    button.Name = "Button_" + i + "_" + j;
                    button.Click += Cell_Click;
                    button.Width = 50;
                    button.Height = 50;
                    button.Margin = new Thickness(1);

                    if (App.p_board[i, j]==0)
                        button.Background = new SolidColorBrush(App.emptyColor);
                    else
                        button.Background = new SolidColorBrush(App.shipColor);

                        Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    GameGrid.Children.Add(button);
                    gridButtons[i, j] = button;
                }
            }
        }

        //click
        private async void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            if (!placingShip)
            {
                if (numberOfShipsPlaced < 5)
                {
                    placingShip = true;

                    var dialog = new ContentDialog
                    {
                        Title = "Odaberi dužinu broda",
                        Content = "Koliko blokova želiš da brod ima?",
                        PrimaryButtonText = "2",
                        SecondaryButtonText = "3"
                    };

                    // Za button brod 4
                    dialog.CloseButtonText = "4";

                    ContentDialogResult result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        shipLength = 2;
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        shipLength = 3;
                    }
                    else if (result == ContentDialogResult.None)
                    {
                        shipLength = 4;
                    }

                    numberOfShipsPlaced++;

                    if (shipLength == 3)
                    {
                        if (shipsOfLength3Placed >= 2)
                        {
                            ShowMessage("Već si postavio maksimalan broj brodova duljine 3 bloka!");
                            placingShip = false;
                            numberOfShipsPlaced--;
                            return;
                        }
                        shipsOfLength3Placed++;
                    }
                    else if (shipLength == 4)
                    {
                        if (shipsOfLength4Placed >= 1)
                        {
                            ShowMessage("Već si postavio maksimalan broj brodova duljine 4 bloka!");
                            placingShip = false;
                            numberOfShipsPlaced--;
                            return;
                        }
                        shipsOfLength4Placed++;
                    }
                    else // shipLength == 2
                    {
                        if (shipsOfLength2Placed >= 2)
                        {
                            ShowMessage("Već si postavio maksimalan broj brodova duljine 2 bloka!");
                            placingShip = false;
                            numberOfShipsPlaced--;
                            return;
                        }
                        shipsOfLength2Placed++;
                    }

                    var dialogOrientation = new ContentDialog
                    {
                        Title = "Odaberi smjer broda",
                        Content = "Kako želiš postaviti brod?",
                        PrimaryButtonText = "Horizontalno",
                        SecondaryButtonText = "Vertikalno"
                    };
                    ContentDialogResult resultOrientation = await dialogOrientation.ShowAsync();
                    bool isHorizontal = (resultOrientation == ContentDialogResult.Primary);

                    if (CheckIfShipFits(row, column, isHorizontal))
                    {
                        PlaceShip(row, column, isHorizontal);
                        placingShip = false;
                    }
                    else
                    {
                        ShowMessage("Nemoguće postaviti brod! Izvan granica ili mjesto je već zauzeto.");
                        placingShip = false;
                        numberOfShipsPlaced--;

                        if (shipLength == 3)
                        {
                            shipsOfLength3Placed--;
                        }
                        else if (shipLength == 4)
                        {
                            shipsOfLength4Placed--;
                        }
                        else
                        {
                            shipsOfLength2Placed--;
                        }
                    }
                }
                else
                {
                    ShowMessage("Nemoguće postaviti više brodova!");
                }
            }

            if (numberOfShipsPlaced==5)
            {
                ready_f();
            }
        }

        private void ready_f()
        {
            continue_b.Opacity = 100;
            savepreset_b.Opacity = 100;
            numberOfShipsPlaced = 5;
            ready = true;
            App.readySetup = false;
        }


        //provjera
        private bool CheckIfShipFits(int row, int column, bool horizontal)
        {
            int requiredLength = shipLength;

            if (horizontal)
            {
                if (column + requiredLength > Columns)
                    return false;

                for (int i = column; i < column + requiredLength; i++)
                {
                    if (gridButtons[row, i].Tag != null)
                        return false;
                }
                return true;
            }
            else
            {
                if (row + requiredLength > Rows)
                    return false;

                for (int i = row; i < row + requiredLength; i++)
                {
                    if (gridButtons[i, column].Tag != null)
                        return false;
                }
                return true;
            }
        }


        //postvaljnje brodova
        private void PlaceShip(int row, int column, bool horizontal)
        {
            int requiredLength = shipLength;
            int putlenght = shipLength;
            if (shipLength == 2 && !b6)
            {
                b6 = true;
            }
            else if (shipLength == 2 && b6)
            {
                putlenght = 6;
                b6 = false;
            }
            else if (shipLength == 3 && !b7)
            {
                b7 = true;
            }
            else if (shipLength == 3 && b7)
            {
                putlenght = 7;
                b7 = false;
            }

            if (horizontal)
            {
                for (int i = column; i < column + requiredLength; i++)
                {
                    App.p_board[row, i] = putlenght;
                    
                    gridButtons[row, i].Background = new SolidColorBrush(App.shipColor);
                    gridButtons[row, i].Tag = "Brod";
                }
            }
            else
            {
                for (int i = row; i < row + requiredLength; i++)
                {
                    App.p_board[i, column] = putlenght;
                    gridButtons[i, column].Background = new SolidColorBrush(App.shipColor);
                    gridButtons[i, column].Tag = "Brod";
                }
            }
        }

        public void savePreset()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            builder.Host = "narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud";
            builder.Port = 26257;
            builder.Username = "dr";
            builder.Password = "dFvz2ADZeOpME1TfRcAI1A";
            builder.Database = "presets";
            builder.SslMode = SslMode.Require;

            string connectionString = builder.ConnectionString;

            try
            {
                string board_str = "9";
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            board_str += App.p_board[i, j];
                        }
                    }
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                string sql = "INSERT INTO preset(board) VALUES('" + board_str + "');";

                var cmd = new NpgsqlCommand(sql, connection);

                // Execute the query and retrieve data
                var reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        // poruka za usera
        private async void ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void savepreset_b_Click(object sender, RoutedEventArgs e)
        {

            if (ready)
            {
                savePreset();
            }
        }

        private void loadpreset_b_Click(object sender, RoutedEventArgs e)
        {
            ready = false;
            if (App.clientActive)
            {
                // send not ready (0)
            }
            continue_b.Opacity = 60;
            savepreset_b.Opacity = 60;
            App.p_board0();
            Frame.Navigate(typeof(Preset));
        }

        private void continue_Click(object sender, RoutedEventArgs e)
        {
            if (ready && ((App.serverActive && ready_opponent) || App.clientActive))
            {
                continue_b.Opacity = 60;
                ready = false;
                savepreset_b.Opacity = 60;
                if (App.serverActive)
                {
                    // send data to start game
                    host.MessageReceived -= opponent_ready;
                    Frame.Navigate(typeof(LANHost));
                }
                else if (App.clientActive)
                {
                    continue_b.Background = new SolidColorBrush(Colors.Lime);
                    
                    // send ready (1) + board

                }
                else
                {
                    Frame.Navigate(typeof(Game));
                }
            }
        }

        private void opponent_ready(object sender, string message)
        {
            if (message[0] == '1')
            {
                ready_opponent = true;
                continue_b.Background = new SolidColorBrush(Colors.Lime);
                int count = 1;
                for (int i = 0;i<10;i++)
                {
                    for (int j=0;j<10;j++)
                    {
                        App.r_board[i, j] = message[count];
                        count++;
                    }
                }
            }
            else if (message[0] == '0')
            {
                ready_opponent = false;
                continue_b.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void clientStartGame(object sender, string message)
        {
            // get app.board_r
            // navigate to LANClient
        }

        private void sendClientReady(string msg)
        {
            // client.send(msg + App.board_p);

            // forst number is 0 - not ready, or 1 - ready, then board
        }

        private void clear_b_Click(object sender, RoutedEventArgs e)
        {
            App.board0();
            openned();
        }
        private void exit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}