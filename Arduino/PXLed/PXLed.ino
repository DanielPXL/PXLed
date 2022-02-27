#include <FastLED.h>

#define NUM_REAL_LEDS 120
#define NUM_LEDS 91
#define LED_PIN 13
#define SERIAL_BAUD_RATE 500000
#define BRIGHTNESS 60

CRGB leds[NUM_REAL_LEDS];

byte buffer[3];
int i;
int c;

bool startRead = false;
bool readComplete = false;

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);

  FastLED.addLeds<WS2812B, LED_PIN, GRB>(leds, NUM_REAL_LEDS);
  FastLED.setBrightness(BRIGHTNESS);
}

void loop() {

}

void serialEvent() {
  while (Serial.available()) {
    byte serialIn = (byte)Serial.read();

    if (startRead == true) {
      if (serialIn == 255) {
        startRead = false;

        FastLED.show();
        Serial.write((byte)100);

        i = 0;
      } else {
        buffer[c] = serialIn;

        c++;
        if (c == 3) {
          leds[i + NUM_REAL_LEDS - NUM_LEDS] = CRGB(buffer[0], buffer[1], buffer[2]);
          c = 0;
          i++;
        }        
      }
    }

    if (serialIn == 254) {
      startRead = true;
    }
  }
}
