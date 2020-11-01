#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <FS.h>
char* ssid = "WIFI SSID HERE";
char* password = "WIFI PASSWORD HERE";
bool ledState = 0;
int sensorValue = 0;
const int ledPin = 2;
const int analogInPin = A0;
bool clientConnected = false;
unsigned int udpPort = 2311; // local port to send UDP packets
int port = 80; //local port for tcp communications
const char* filename = "/wifisettings.txt";

WiFiUDP udp;
WiFiServer server(port);
WiFiClient client;
void setup(){
  // Serial port for debugging purposes
  Serial.begin(115200);

  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);

  //SPIFFS.begin();
  //loadWifiSettings();
  startWifi();
  // Connect to Wi-Fi
  //WiFi.mode(WIFI_STA);
  
  delay(1000);
}

void startWifi()
{
  WiFi.begin(ssid, password);
  int seconds = 0;
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi..");
    seconds = seconds + 1;
    if (seconds > 30)
    {
      //unable to connect, start the process over
      
    }
  }

  // Print ESP Local IP Address
  Serial.println(WiFi.localIP());
  server.begin();
}

void loadWifiSettings()
{
    File f = SPIFFS.open(filename, "r");
    if (!f){
      //wifi settings not saved, we need to configure ourselves as a server now.
      getWifiSettings();
    }
}

void getWifiSettings()
{
  
}

void loop() {
  
  client = server.available();
  
  Serial.println("Checking client");
  if (client)
  {  
    Serial.println("Found client");
    if(client.connected())
    {
      Serial.println("Client Connected");
    }
    while (!client.connected()){
    
      udp.begin(udpPort);
      Serial.println("Client not connected, sending UDP discovery packet");
      udp.beginPacket("255.255.255.255", udpPort);
      udp.print("HeartMonitorActive:" + udpPort);
      udp.endPacket();
      
      delay(2000);
    
      return;
    
    }
    while(client.connected()){
      sensorValue = analogRead(analogInPin);
      long curtime = millis();
      Serial.print(sensorValue);
      Serial.print(":");
      Serial.println(curtime);
      client.print(sensorValue);
      client.print(":");
      client.println(curtime);
      delay(10);
    }
    client.stop();
    Serial.println("Client Disconnected");
  }
  else
  {
      udp.begin(udpPort);
      Serial.println("Client not connected, sending UDP discovery packet");
      udp.beginPacket("255.255.255.255", udpPort);
      udp.print("HeartMonitorActive:" + udpPort);
      udp.endPacket();
      
      delay(2000);
  }
  
}
