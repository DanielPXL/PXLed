#include <NeoPixelBus.h>

#define NUM_REAL_LEDS 120
#define NUM_LEDS 91
#define LED_PIN 13
#define SERIAL_BAUD_RATE 500000
#define BRIGHTNESS 60

NeoPixelBus<NeoGrbFeature, NeoWs2812xMethod> strip(NUM_REAL_LEDS, LED_PIN);

uint8_t buffer[3];
int i;
int c;

bool startRead = false;
bool readComplete = false;

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  
  strip.Begin();
  strip.Show();
}

void loop() {
  
}

void serialEvent() {
  while (Serial.available()) {
    byte serialIn = (byte)Serial.read();

    if (startRead == true) {
      if (serialIn == 255) {
        startRead = false;

        strip.Show();
        Serial.write((uint8_t)100);

        i = 0;
      } else {
        buffer[c] = serialIn;

        c++;
        if (c == 3) {
          strip.SetPixelColor(i + NUM_REAL_LEDS - NUM_LEDS, RgbColor(buffer[0], buffer[1], buffer[2]));
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
