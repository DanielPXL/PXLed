# PXLed

## About
PXLed is a real-time LED strip (WS2812B/SK6812) interface made in WPF that is designed to run on a computer without GPIO pins. It uses a microcontroller to connect to the LED strip and communicates with it through USB or WiFi.

## Features
- Real-time WS2812B LED strip connection
    - Supports Arduino and ESP32
- Different connection methods
    - USB for low latency
    - WiFi for bigger distances
- Useful LED strip preview
- Easy to use plugin system
- Basic effects included by default

## Installation

### Requirements
- Windows 7 or newer
- .NET 6.0 or newer
- An Arduino or ESP32 that you can connect to via USB (for setup)
- Arduino IDE
- WS2812B or SK6812 LED strip that is already set up with the microcontroller

### Guide
1. Download the [latest release](https://github.com/DanielPXL/PXLed/releases/latest) of PXLed
2. Connect the microcontroller to your computer
3. Follow the instructions below depending on your connection method

#### USB
1. Open PXLed_NeoPixelBus.ino in the Arduino IDE (found in Microcontroller/PXLed_NeoPixelBus/PXLed_NeoPixelBus.ino)
2. Edit the five defines at the top of the file
    - NUM_REAL_LEDS - the actual number of LEDs in your LED strip
    - NUM_LEDS - the number of LEDs that will be controlled by the host computer (if less than NUM_REAL_LEDS, PXLed will not use the first few LEDs in your LED strip)
    - LED_PIN - GPIO pin on your microcontroller connected to the LED strip data pin
    - SERIAL_BAUD_RATE - baud rate with which to communicate with the host computer, default is usually fine
    - BRIGHTNESS - maximum brightness of the LED strip (between 0 and 255)
3. Upload the code to the microcontroller and close the IDE
4. Start PXLed on your computer
5. In the settings, make sure WiFi is disabled and edit the following values:
    - Number of LEDs - equal to NUM_LEDS in the microcontroller code
    - Device Port Name - Name of the microcontroller port in Windows (viewable in Device Manager)
    - Device Baud Rate - equal to SERIAL_BAUD_RATE in the microcontroller code
6. Click "Apply"
7. Hit the reset button on the microcontroller and the LED strip should turn on!

#### WiFi
1. Open PXLed_WiFi.ino in the Arduino IDE (found in Microcontroller/PXLed_WiFi/PXLed_WiFi.ino)
2. Edit the seven defines at the top of the file
    - NUM_REAL_LEDS - the actual number of LEDs in your LED strip
    - NUM_LEDS - the number of LEDs that will be controlled by the host computer (if less than NUM_REAL_LEDS, PXLed will not use the first few LEDs in your LED strip)
    - LED_PIN - GPIO pin on your microcontroller connected to the LED strip data pin
    - BRIGHTNESS - maximum brightness of the LED strip (between 0 and 255)
    - WIFI_SSID - the SSID of the WiFi network you want to connect to
    - WIFI_PASSWORD - the password of the WiFi network you want to connect to
    - PORT - the port number to use (default is 12241)
3. Upload the code to the microcontroller and close the IDE
4. Start PXLed on your computer
5. In the settings, make sure WiFi is enabled and edit the following values:
    - Number of LEDs - equal to NUM_LEDS in the microcontroller code
    - Device IP - IP address of the microcontroller (can be found through your router for example)
    - Device Port - equal to PORT in the microcontroller code (default is 12241)
6. Click "Apply"
7. Hit the reset button on the microcontroller and the LED strip should turn on!
8. Disconnect the microcontroller from your computer and connect it to a different power source. It should now communicate over WiFi