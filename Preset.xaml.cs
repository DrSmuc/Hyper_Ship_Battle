using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Npgsql;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel.Calls;
using System.Collections.Specialized;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hyper_Ship_Battle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Preset : Page
    {

        private int[,,] boards = new int[9, 10, 10];

        public Preset()
        {
            this.InitializeComponent();
            openned();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            openned();
        }

        private void openned()
        {
            for (int m = 0; m < 9; m++)
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        boards[m, i, j] = -1;

            loadPresets();
            CreateGrid();
        }

        public void loadPresets()
        {

            //var connectionString = "postgresql://dr:dFvz2ADZeOpME1TfRcAI1A@narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud:26257/presets?sslmode=verify-full";

            //dFvz2ADZeOpME1TfRcAI1A
            //postgresql://dr:dFvz2ADZeOpME1TfRcAI1A@narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud:26257/presets?sslmode=verify-full


            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            // Set connection parameters
            builder.Host = "narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud";
            builder.Port = 26257;
            builder.Username = "dr";
            builder.Password = "dFvz2ADZeOpME1TfRcAI1A";
            builder.Database = "presets";
            builder.SslMode = SslMode.Require;

            string connectionString = builder.ConnectionString;


            var sql = "SELECT board FROM preset";

            try
            {
                // Connect to the database
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                // Create command object with the SQL query and connection
                var cmd = new NpgsqlCommand(sql, connection);

                // Execute the query and retrieve data
                var reader = cmd.ExecuteReader();
                int m = 0;
                // Process the results
                while (reader.Read())
                {
                    if (m > 8)
                        break;
                    string board_str = reader.GetString(0);
                    int count = 0;
                    for (int i=0; i<10; i++)
                    {
                        for (int j=0;j<10;j++)
                        {
                            count++;
                            boards[m, i, j] = board_str[count] - '0';
                        }
                    }
                    m++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void CreateGrid()
        {
            int count = -1;
            for (int m=0; m<3; m++)
            {
                for (int n=0; n<3; n++)
                {
                    count++;
                    Grid inner = new Grid();
                    for (int i = 0; i < 10; i++)
                    {
                        inner.RowDefinitions.Add(new RowDefinition());
                        inner.ColumnDefinitions.Add(new ColumnDefinition());
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            Rectangle rect = new Rectangle();

                            if (boards[count, i, j]>0)
                                rect.Fill=new SolidColorBrush(App.shipColor);
                            else
                                rect.Fill = new SolidColorBrush(App.emptyColor);

                            rect.Stroke = new SolidColorBrush(App.strokeColor);
                            rect.StrokeThickness = 1;
                            rect.Height = 24;
                            rect.Width = 24;
                            rect.PointerPressed += Rect_PointerPressed;
                            rect.Name = "r" + count;
                            rect.Tag = m;
                            Grid.SetRow(rect, i);
                            Grid.SetColumn(rect, j);
                            inner.Children.Add(rect);
                        }
                    }
                    Grid.SetRow(inner, m);
                    Grid.SetColumn(inner, n+1);
                    MainGrid.Children.Add(inner);
                }
            }
        }

        private void Rect_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;

            switch(rect.Name)
            {
                case "r0":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[0, i, j]==-1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[0, i, j];
                        }
                    }
                    break;
                case "r1":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[1, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[1, i, j];
                        }
                    }
                    break;
                case "r2":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[2, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[2, i, j];
                        }
                    }
                    break;
                case "r3":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[3, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[3, i, j];
                        }
                    }
                    break;
                case "r4":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[4, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[4, i, j];
                        }
                    }
                    break;
                case "r5":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[5, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[5, i, j];
                        }
                    }
                    break;
                case "r6":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[6, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[6, i, j];
                        }
                    }
                    break;
                case "r7":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[7, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[7, i, j];
                        }
                    }
                    break;
                case "r8":
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (boards[8, i, j] == -1)
                            {
                                App.p_board0();
                                return;
                            }
                            App.p_board[i, j] = boards[8, i, j];
                        }
                    }
                    break;
            }
            App.resetSetup = false;
            App.readySetup = true;
            Frame.Navigate(typeof(Setup));
        }



        private void back_Click(object sender, RoutedEventArgs e)
        {
            App.resetSetup = true;
            Frame.Navigate(typeof(Setup));
        }
    }
}
