using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System;
using Windows.UI.Xaml;

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

        public Setup()
        {
            this.InitializeComponent();
            InitializeGrid();
        }

        // grid s gumbom
        private void InitializeGrid()
        {
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

            if (horizontal)
            {
                for (int i = column; i < column + requiredLength; i++)
                {
                    gridButtons[row, i].Background = new SolidColorBrush(Colors.Blue);
                    gridButtons[row, i].Tag = "Brod";
                }
            }
            else
            {
                for (int i = row; i < row + requiredLength; i++)
                {
                    gridButtons[i, column].Background = new SolidColorBrush(Colors.Blue);
                    gridButtons[i, column].Tag = "Brod";
                }
            }
        }


        // poruka za usera
        private async void ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}