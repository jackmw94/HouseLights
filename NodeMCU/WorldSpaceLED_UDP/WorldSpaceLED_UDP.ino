#include <FastLED.h>
#include <ESP8266WiFi.h>
#include <WiFiUdp.h>

// --- LED CONFIGURATION ---
#define NUM_STRIPS 5
#define NUM_LEDS_PER_STRIP 100
#define NUM_LEDS 500

// --- DATA CONFIGURATION ---
#define PACKET_SIZE 2

// --- PROG CONFIGURATION ---
#define VERBOSE true
#define PRINT_IP true
#define USE_UDP true
#define LEDS_DIE false

// --- WIFI CONFIGURATION
#define STASSID "JackHatHouse"
#define STAPSK "Password24"


unsigned int localPort = 8888;
WiFiUDP udpDataReceiver;
char udpPacketBuffer[UDP_TX_PACKET_MAX_SIZE + 1];
byte serialReadBuffer[PACKET_SIZE];

CRGB leds[NUM_LEDS];
bool started = false;
bool received = false;

void setup() {

  Serial.begin(921600);  /// 921600 ?

  FastLED.addLeds<NEOPIXEL, 16>(leds, 0 * NUM_LEDS_PER_STRIP, NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 5>(leds, 1 * NUM_LEDS_PER_STRIP, NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 4>(leds, 2 * NUM_LEDS_PER_STRIP, NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 2>(leds, 3 * NUM_LEDS_PER_STRIP, NUM_LEDS_PER_STRIP);
  FastLED.addLeds<NEOPIXEL, 14>(leds, 4 * NUM_LEDS_PER_STRIP, NUM_LEDS_PER_STRIP);

  if (USE_UDP) SetupUDP();
}

void loop() {

  if (!started) {
    for (int i = 0; i < NUM_LEDS; i++) {
      leds[i] = CHSV(0, 0, 0);
    }

    FastLED.show();
    started = true;
  }

  bool changed = false;
  if (USE_UDP) {
    changed = ReceiveDataUDP();
  } else {
    changed = ReceiveDataSerial();
  }
  received |= changed;

  bool oncePerSecond = millis() % 1000 == 0;
  bool oncePerTenSeconds = millis() % 10000 == 0;

  if (LEDS_DIE) {
    for (int i = 0; i < NUM_LEDS; i++) {
      leds[i].fadeToBlackBy(40);
    }
  }

  // !! todo: decide how often to show
  if (changed || oncePerSecond || true) {
    FastLED.show();
  }

  if (PRINT_IP && oncePerTenSeconds) {
    Serial.printf("Broadcasting: %s \n", WiFi.localIP().toString().c_str());
  }  

  delay(5);
}

bool ReceiveDataUDP() {

  int packetSize = udpDataReceiver.parsePacket();
  if (!packetSize) {
    return false;
  }

  if (VERBOSE) Serial.printf("Received packet of size %d from %s:%d\n    (to %s:%d, free heap = %d B)\n", packetSize, udpDataReceiver.remoteIP().toString().c_str(), udpDataReceiver.remotePort(), udpDataReceiver.destinationIP().toString().c_str(), udpDataReceiver.localPort(), ESP.getFreeHeap());

  // read the packet into packetBufffer
  int n = udpDataReceiver.read(udpPacketBuffer, UDP_TX_PACKET_MAX_SIZE);
  udpPacketBuffer[n] = 0;

  int parts = n / 2;

  if (VERBOSE) {
    Serial.printf("Received %d bytes, making %d data changes\n", n, parts);
    for (int i = 0; i < n; i++) {
      Serial.printf("%d, ", (int)udpPacketBuffer[i]);
    }
    Serial.println("\n");
  }

  for (int i = 0; i < parts; i++) {
    int index = i * 2;
    int b1 = (int)udpPacketBuffer[index];
    int b2 = (int)udpPacketBuffer[index + 1];

    UpdateDataFromMessage(b1, b2);
  }

  return true;
}

bool ReceiveDataSerial() {

  bool changed = false;
  while (Serial.available()) {

    // should we be reading whole buffer at once? will this mean fewer handshakes?
    Serial.readBytes(serialReadBuffer, PACKET_SIZE);
    int b1 = serialReadBuffer[0];
    int b2 = serialReadBuffer[1];

    UpdateDataFromMessage(b1, b2);

    if (VERBOSE) {
      Serial.printf("Received serial part: %d, %d \n", b1, b2);
    }

    changed = true;
  }

  return changed;
}

void UpdateDataFromMessage(int b1, int b2) {
  int index = ((b2 & 0x01) << 8) | (b1 & 0xFF);

  int r = (((b2 & 0xC0) >> 6) / 3.0) * 255;
  int g = (((b2 & 0x30) >> 4) / 3.0) * 255;
  int b = (((b2 & 0x0C) >> 2) / 3.0) * 255;

  leds[index] = CRGB(r, g, b);
}

void SetupUDP() {

  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("Not connected at start.");
    WiFi.mode(WIFI_STA);
    WiFi.begin(STASSID, STAPSK);
  }

  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }

  Serial.print("\nConnected! \nIP address: ");
  Serial.println(WiFi.localIP());

  Serial.print("UDP server on port: ");
  Serial.println(localPort);

  udpDataReceiver.begin(localPort);
}