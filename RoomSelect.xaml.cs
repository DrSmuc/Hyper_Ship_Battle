using Hyper_Ship_Battle.LAN_Multiplayer;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class RoomSelect : Page
    {
        TcpClient client;
        private string[] names = new string[9];
        private string[] ips = new string[9];
        private int roomcount;

        public RoomSelect()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            roomcount = 0;
            loadRooms();
            CreateGrid();
        }

        private void loadRooms()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            builder.Host = "narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud";
            builder.Port = 26257;
            builder.Username = "dr";
            builder.Password = "dFvz2ADZeOpME1TfRcAI1A";
            builder.Database = "presets";
            builder.SslMode = SslMode.Require;

            string connectionString = builder.ConnectionString;


            var sql = "SELECT name, ip FROM room_list";

            try
            {
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var cmd = new NpgsqlCommand(sql, connection);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (roomcount> 8)
                        break;
                    names[roomcount] = reader.GetString(0);
                    ips[roomcount] = reader.GetString(1);
                    roomcount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void CreateGrid()
        {
            int count = 0;
            for (int m = 0; m < 3; m++)
            {
                for (int n = 0; n < 3; n++)
                {
                    Grid inner = new Grid();
                    for (int i = 0; i < 2; i++)
                    {
                        inner.RowDefinitions.Add(new RowDefinition());
                    }
                    inner.ColumnDefinitions.Add(new ColumnDefinition());
                    inner.HorizontalAlignment = HorizontalAlignment.Center;
                    inner.VerticalAlignment = VerticalAlignment.Center;

                    TextBlock nameText = new TextBlock();
                    nameText.Text = names[count];
                    Grid.SetRow(nameText, 0);
                    Grid.SetColumn(nameText, 0);
                    inner.Children.Add(nameText);

                    Button button = new Button();
                    button.Content = "Join";
                    button.Height = 58;
                    button.Width = 180;
                    button.Click += Rect_PointerPressed;
                    button.Name = "r" + count;
                    Grid.SetRow(button, 1);
                    Grid.SetColumn(button, 0);
                    inner.Children.Add(button);



                    Grid.SetRow(inner, m);
                    Grid.SetColumn(inner, n + 1);
                    MainGrid.Children.Add(inner);

                    count++;
                    if (count >= roomcount)
                    {
                        return;
                    }
                }
            }
        }

        private void Rect_PointerPressed(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            int i = 0;
            switch (button.Name)
            {
                case "r0":
                        i = 0;
                    break;
                case "r1":
                        i = 1;
                    break;
                case "r2":
                        i = 2;
                    break;
                case "r3":
                        i = 3;
                    break;
                case "r4":
                        i = 4;
                    break;
                case "r5":
                        i = 5;
                    break;
                case "r6":
                        i = 6;
                    break;
                case "r7":
                        i = 7;
                    break;
                case "r8":
                        i = 8;
                    break;
            }

            client = ClientManager.Instance.GetClient();
            client.Connect(ips[i], App.PORT_NUMBER);

            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            builder.Host = "narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud";
            builder.Port = 26257;
            builder.Username = "dr";
            builder.Password = "dFvz2ADZeOpME1TfRcAI1A";
            builder.Database = "presets";
            builder.SslMode = SslMode.Require;

            string connectionString = builder.ConnectionString;


            var sql = "DELETE FROM room_list WHERE name = '" + names[i] + "' AND ip = '" + ips[i] + "';";

            try
            {
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var cmd = new NpgsqlCommand(sql, connection);

                var reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Frame.Navigate(typeof(Setup));
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(MainPage));
        }
    }
}
