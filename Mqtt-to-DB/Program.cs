using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using MQTTnet;
using MQTTnet.Client;

using System.Text;
using MQTTnet.Server;

public class Program
{
	public class Dato
	{
		public int Id { get; set; }
		public DateTime DateTime { get; set; }
		public string Temperatura { get; set; }
	}

	static async Task Main(string[] args)
	{
		string broker = "broker.hivemq.com";
		int port = 1883;
		string topic = "ProgettoStage";

		// Create a MQTT client factory
		var factory = new MqttFactory();

		// Create a MQTT client instance
		var mqttClient = factory.CreateMqttClient();

		// Create MQTT client options
		var options = new MqttClientOptionsBuilder()
			.WithTcpServer(broker, port) // MQTT broker address and port
			.WithCleanSession()
			.Build();

		// Connect to MQTT broker
		var connectResult = await mqttClient.ConnectAsync(options);

		if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
		{
			Console.WriteLine("Connected to MQTT broker successfully.");

			// Subscribe to a topic
			await mqttClient.SubscribeAsync(topic);

			// Callback function when a message is received
			mqttClient.ApplicationMessageReceivedAsync += e =>
			{
				Dato dato = new Dato();
				string message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				string[] temp = message.Split(' ');
				dato.Temperatura = temp[1];


				Console.WriteLine($"Ho ricevuto il messaggio: {dato.Temperatura}");
				scriviDato(dato);


				return Task.CompletedTask;
			};

			// Publish a message 10 times
			while (Console.ReadKey().Key != ConsoleKey.Escape) ;

			// Unsubscribe and disconnect
			await mqttClient.UnsubscribeAsync(topic);
			await mqttClient.DisconnectAsync();
		}
		else
		{
			Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
		}
	}


	public static void scriviDato(Dato x)
	{
		string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=C:\\Users\\Thomas Bravin\\Desktop\\2iot-progettostage-main\\2IOTDB.mdf;" +
			"Integrated Security=True;" +
			"Connect Timeout=30;";

		try
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				string queryInsert = $"INSERT INTO Dati (Message) VALUES ('{x.Temperatura}')";

				using (SqlCommand cmd = new SqlCommand(queryInsert, conn))
				{
					cmd.ExecuteNonQuery();
				}

				conn.Close();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	public static void readDB()
	{
        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
    "AttachDbFilename=C:\\Users\\Thomas Bravin\\Desktop\\2iot-progettostage-main\\2IOTDB.mdf;" +
    "Integrated Security=True;" +
    "Connect Timeout=30;";

        List<Dato> dati = new List<Dato>();

		try
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				string query = "SELECT * FROM Dati";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							Dato dato = new Dato();
							dato.Id = reader.GetInt32(0);
							dato.DateTime = reader.GetDateTime(1);
							//dato.Temperatura = reader.GetDecimal(2);
							dati.Add(dato);
						}
					}
				}

				conn.Close();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}


		foreach (Dato dato in dati)
		{
			Console.WriteLine($"{dato.Id} - {dato.DateTime} - {dato.Temperatura}°C");
		}
	}
}