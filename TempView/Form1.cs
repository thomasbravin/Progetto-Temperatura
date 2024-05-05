using LiveCharts.Wpf;
using LiveCharts;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Media;

namespace TempView
{
	public partial class Form1 : Form
	{
		public static int samples = 20;
		public class Dato
		{
			public int Id { get; set; }
			public DateTime DateTime { get; set; }
			public string Temperatura { get; set; }
			public double NumTemp { get; set; }
		}

		public static List<Dato> dati;

		public Form1()
		{
			InitializeComponent();
			timer1.Enabled = true;
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double>()
                }
            };
			for (int i = 0; i < samples; i++)
			{
				cartesianChart1.Series[0].Values.Add(0.00);
            }
        }

		private void timer1_Tick(object sender, EventArgs e)
		{
			readDB();
			List<double> temperature = new List<double>();
			foreach (var dato in dati)
			{
				temperature.Add(Convert.ToDouble(dato.Temperatura.Replace('.', ',')));
			}
						
			for (int i = 0; i < samples; i++)
			{
				cartesianChart1.Series[0].Values[i] = temperature[i];
			}
						

		}


		public static void readDB()
		{
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=C:\\Users\\Thomas Bravin\\Desktop\\2iot-progettostage-main\\2IOTDB.mdf;" +
            "Integrated Security=True;" +
            "Connect Timeout=30;";

            dati = new List<Dato>();
			
			try
			{
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					conn.Open();
					string query = $"SELECT TOP {samples} * FROM Dati ORDER BY Id DESC";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								Dato dato = new Dato();
								dato.Id = reader.GetInt32(0);
								dato.DateTime = reader.GetDateTime(1);
								dato.Temperatura = reader.GetString(2);
								dati.Add(dato);
							}
						}
					}
					dati.Reverse();
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
					
		}
	}
}
