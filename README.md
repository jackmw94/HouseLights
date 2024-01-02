
# House Lights
Control LED strips through their world space locations. Uses Unity to setup position information and then control LED colours. Uses a NodeMCU board to receive colour information and set appropriate LEDs.

[![Watch the video](https://i.ibb.co/TmsrN7W/Screenshot-2024-01-02-093518.png)](https://vimeo.com/899168891?share=copy)

### Hardware
Hardware setup was to have D0-D3 pins control 4 transistors which connect voltage to data line on LED strips. A 60A 5V power supply powers both NodeMCU and leds. Uses a common ground for all components. 

In the image you can see 4 data lines coming out of the D0-D3 pins to the transistors' middle pins, with their Vin connected to the voltage line and Vout to strip data. NodeMCU connects to voltage line and ground with orange and green wires. USB can still be connected to detect IP address which is sent through serial connection every 10 seconds. https://ibb.co/drh37X7

### Setup
Use the LEDSetup scene to setup LED locations. This uses the webcam feed to determine normalised position. This is currently quite rudimentary as it's done by detecting the brightest pixel, setup should be done in dark surroundings. This setup might take some tuning and manual iteration - see the setup component for these parameters.

Ensure that the transmitter components have the correct IP addresses and the dispatcher has a transmitter for each microcontroller you're connecting to.

When running you'll see various gizmos on the screen, these relate to the stored position, frame position and average position. It's the average position that's stored when the setup advances to next LED. Use arrow keys to go forward and back, space to update cached position and S to save this to the persistent scriptable object. Once this detects reliably then you can uncheck "wait for user input" to have this run automatically, it then advances when there is a low error threshold across averaged positions.

### Running
Once setup, use the LEDDisplay scene to send screen colours to the LEDs. Again ensure that the transmitter components have the correct IP addresses and the dispatcher has a transmitter for each microcontroller you're connecting to. These should match those used in setup - this should probably form part of the setup data.

Then click play and the screen colours will be sent to the NodeMCUs. This works via the RenderTexture on RTCamera, this RenderTexture is square by default but I'd expect other sizes to also work as LED positions are stored as normalised.

Anything you want to create in Unity can then be sent to display on the strips. In future I'd like to make some sort of screen capture to display other monitors as this would make showing anything on the PC much easier.

### Known Issues
Red and green channels are swapped, should be an easy fix as FastLED library supports automatic RGB channel swizzling.

Currently you have to connect via USB to see what IP address it's receiving on. This could/should be broadcast when available for connection.
