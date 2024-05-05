#include <Arduino.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const char* ssid = "";
const char* password = "";
const char* mqtt_server = "broker.hivemq.com";

WiFiClient espClient;
PubSubClient client(espClient);
unsigned long lastMsg = 0;
#define MSG_BUFFER_SIZE	(50)
char msg[MSG_BUFFER_SIZE];
int value = 0;

void setup_wifi()
{
  delay(10);
  Serial.println();
  Serial.print("collegamento a ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("wifi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void callback(char *topic, byte *payload, unsigned int lenght)
{
  Serial.print("è arrivato un messsaggio sul topic: ");
  Serial.print(topic);
  Serial.print(": ");
  
}

void setup()
{
  Serial.begin(115200);
  randomSeed(analogRead(0));

  setup_wifi();

  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);
}

void reconnect()
{
  while (!client.connected())
  {
    Serial.print("mi sto riconnettendo al server MQTT..");

    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);

    if (client.connect(clientId.c_str()))
    {
      Serial.println("connesso");
      client.publish("ProgettoStage", "Sono ESP8266, sto pubblicando un messaggio");
      client.subscribe("ProgettoStage");
    }
    else
    {
      Serial.print("connessione fallita, rc=");
      Serial.print(client.state());
      Serial.println(" riprovo tra 5 secondi");

      delay(5000);
    }
  }
}

void loop() {

  if (!client.connected()) {
    reconnect();
  }
  client.loop();

  unsigned long now = millis();
  if (now - lastMsg > 5000) {
    lastMsg = now;
    value = random(-1000, 4000);
    float temp = value / 100.00;
    snprintf (msg, MSG_BUFFER_SIZE, "Temp: %0.2f [°C]", temp);
    Serial.print("Publish message: ");
    Serial.println(msg);
    client.publish("ProgettoStage", msg);
  }
}