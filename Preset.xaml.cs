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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hyper_Ship_Battle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Preset : Page
    {
        public Preset()
        {
            this.InitializeComponent();
        }

        public void loadPresets()
        {
            var connectionString = "postgresql://dr:0TEbISYQ1wJPqS_qFIoPmw @narrow-onager-13839.8nj.gcp-europe-west1.cockroachlabs.cloud:26257/presets?sslmode=verify-full";
            
            
            var sql = "SELECT * FROM my_table";

            try
            {
                // Connect to the database
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                // Create command object with the SQL query and connection
                var cmd = new NpgsqlCommand(sql, connection);

                // Execute the query and retrieve data
                var reader = cmd.ExecuteReader();

                // Process the results
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string board_str = reader.GetString(1);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
