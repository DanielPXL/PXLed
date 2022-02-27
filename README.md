# PXLed

## About
PXLed is a real-time LED strip (WS2812B) interface made in WPF that is designed to run on a computer without GPIO pins. It uses a secondary system like an Arduino to connect to the LED strip and communicates with it through USB.

## Features
- Real-time WS2812B LED strip connection
    - Arduino is the only supported device right now, support for Raspberry Pi is planned in the future
- Useful LED strip preview
- Easy to use plugin system
- Basic effects included by default

## Installation

### Requirements
- Windows 7 or newer
- .NET 6.0 or newer
- An Arduino that you can connect to via USB
- Arduino IDE
- WS2812B LED strip that is already set up with the Arduino

### Guide
1. Download the latest release of PXLed
2. Connect the Arduino to your computer
3. Open PXLed.ino in the Arduino IDE (found in Arduino/PXLed.ino)
3. Edit the five defines at the top of the file
    - NUM_REAL_LEDS - the actual number of LEDs in your LED strip
    - NUM_LEDS - the number of LEDs that will be controlled by the host computer (if less than NUM_REAL_LEDS, PXLed will not use the first few LEDs in your LED strip)
    - LED_PIN - GPIO pin on your Arduino connected to the WS2812B data
    - SERIAL_BAUD_RATE - baud rate with which to communicate with the host computer, default is usually fine
    - BRIGHTNESS - maximum brightness of the LED strip (between 0 and 255)
5. Upload the code to the Arduino and close the IDE
6. Start PXLed on your computer
7. In the settings, edit the values and click "Apply"
    - Number of LEDs - equal to NUM_LEDS in the Arduino code
    - Arduino Port Name - Name of the Arduino port in Windows (viewable in Device Manager)
    - Arduino Baud Rate - equal to SERIAL_BAUD_RATE in the Arduino code
8. Click the reset button on the Arduino and the LED strip should turn on!