#include <NeoPixelBus.h>
#include <WiFi.h>
#include <WiFiUdp.h>

#define NUM_REAL_LEDS 297
#define NUM_LEDS 297
#define LED_PIN 2
#define BRIGHTNESS 60

#define WIFI_SSID "YOUR_WIFI_SSID_HERE"
#define WIFI_PASSWORD "YOUR_WIFI_PASSWORD_HERE"

#define PORT 12241

NeoPixelBus<NeoGrbwFeature, NeoEsp32I2s1800KbpsMethod> strip(NUM_REAL_LEDS, LED_PIN);

uint8_t buffer[NUM_LEDS * 3];

WiFiUDP udp;

void setup() {  
  strip.Begin();
  strip.Show();

  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
  }

  udp.begin(PORT);
}

void loop() {
  int packetSize = udp.parsePacket();
  if (packetSize)
  {
    udp.read(buffer, NUM_LEDS * 3);

    for (int i = 0; i < NUM_LEDS; i++) {
      strip.SetPixelColor(i + NUM_REAL_LEDS - NUM_LEDS, rgbToRgbw(buffer[i * 3], buffer[i * 3 + 1], buffer[i * 3 + 2]));
    }

    strip.Show();

    // udp.beginPacket(udp.remoteIP(), udp.remotePort());
    // udp.write(100);
    // udp.endPacket();
  }
}

RgbwColor rgbToRgbw(uint8_t r, uint8_t g, uint8_t b) {
  //Get the maximum between R, G, and B
  float tM = max(r, max(g, b));
  
  //If the maximum value is 0, immediately return pure black.
  if(tM == 0) {
    return RgbwColor(0, 0, 0, 0);
  }
  
  //This section serves to figure out what the color with 100% hue is
  float multiplier = 255.0 / tM;
  float hR = r * multiplier;
  float hG = g * multiplier;
  float hB = b * multiplier;  
  
  //This calculates the Whiteness (not strictly speaking Luminance) of the color
  float M = max(hR, max(hG, hB));
  float m = min(hR, min(hG, hB));
  float Luminance = ((M + m) / 2.0 - 127.5) * (255.0/127.5) / multiplier;
  
  //Calculate the output values
  uint32_t Wo = (uint32_t)(Luminance);
  uint32_t Bo = (uint32_t)(b - Luminance);
  uint32_t Ro = (uint32_t)(r - Luminance);
  uint32_t Go = (uint32_t)(g - Luminance);
  
  //Trim them so that they are all between 0 and 255
  if (Wo < 0) Wo = 0;
  if (Bo < 0) Bo = 0;
  if (Ro < 0) Ro = 0;
  if (Go < 0) Go = 0;
  if (Wo > 255) Wo = 255;
  if (Bo > 255) Bo = 255;
  if (Ro > 255) Ro = 255;
  if (Go > 255) Go = 255;
  return RgbwColor((uint8_t)Ro, (uint8_t)Go, (uint8_t)Bo, (uint8_t)Wo);
}
