#include <FastLED.h>

#define NUM_REAL_LEDS 120
#define NUM_LEDS 91
#define LED_PIN 13

CRGB leds[NUM_LEDS];

byte buffer[NUM_LEDS * 3 + 1];
int i;

bool startRead = false;
bool readComplete = false;

void setup() {
  Serial.begin(500000);
  
  FastLED.addLeds<WS2812B, LED_PIN, GRB>(leds, NUM_REAL_LEDS);
  FastLED.setBrightness(60);
}

void loop() {
  if (readComplete) {
    for (int o = 0; o < NUM_LEDS; o++) {
      byte r = buffer[o * 3];
      byte g = buffer[(o * 3) + 1];
      byte b = buffer[(o * 3) + 2];

      leds[o + NUM_REAL_LEDS - NUM_LEDS] = CRGB(r, g, b);
    }
    
    FastLED.show();
    readComplete = false;
    Serial.write((byte)100);
  }
}

void serialEvent() {
  while (Serial.available()) {
    byte serialIn = (byte)Serial.read();

    if (startRead == true) {
      buffer[i] = serialIn;
      
      if (buffer[i] == 255) {
        readComplete = true;
        startRead = false;
        i = 0;
      } else {
        i++;
      }
    }   

    if (serialIn == 254) {
      startRead = true;
    }
  }
}
