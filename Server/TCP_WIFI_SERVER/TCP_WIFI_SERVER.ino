#include <ESP8266WiFi.h>
#include <WiFiClient.h>
 
const char* ssid = "Назва Wi-Fi";
const char* password = "Пароль";
WiFiServer server(2323);
String prevMessage = "";

void setup() {
  Serial.begin(115200);
  delay(10);
 
  // Підключення до Wi-Fi
  Serial.println();
  Serial.println();
  Serial.print("Підключення до Wi-Fi: ");
  Serial.println(ssid);
 
  WiFi.begin(ssid, password);
 
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
 
  Serial.println("");
  Serial.println("Підключено до Wi-Fi");
 
  // Запуск TCP-сервера
  server.begin();
  Serial.print("TCP-сервер запущено з наступними даними: ");
   IPAddress ip = WiFi.localIP();
  Serial.print(ip);
  Serial.println(" 2323");
}
 
void loop() {
  WiFiClient client = server.available();
  if (client) {
    Serial.println("Новий клієнт");
    while (client.connected()) {
      if (client.available()) {
        String message = client.readStringUntil('\r');
        Serial.println("Отримано повідомлення: " + message);
        if (message != prevMessage) {
          Serial.println("Виявлено нове повідомлення");
          prevMessage = message;
          client.println("Повідомлення отримано: " + message);
        } else {
          Serial.println("Виявлено дублікат повідомлення");
          client.println("Дублікат повідомлення отримано: " + message);
        }
      }
    }
    Serial.println("Клієнт відключився");
  }
}
