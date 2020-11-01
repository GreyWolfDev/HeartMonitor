# ESP8266 Heart Monitor with ECG style graphic

## Materials needed
- ESP8266 board
- AD8232 - the one I used can be found [here](https://www.dfrobot.com/product-1510.html)
- ECG electrode pads - example [here](https://www.amazon.com/gp/product/B0757JNVGK)

## Configuration
Connect the ECG module to ground, 5v, and analog pin on ESP8266.  Be sure to open the sketch file (heart_monitor.ino) and input your WiFi settings.

Run the client WPF application.  The ESP8266 will detect there is no client connected, and will broadcast a packet over UDP.  The desktop client will see this UDP broadcast, and automatically connect.  Takes up to 3 seconds.  Once connection has been made, you should begin seeing the graph running.

## ECG Placement
- Important note: wearing a shirt will cause massive interference from the static electricity
- I have found the Fontaine placement method works best.

![Fontaine Lead Placement](https://litfl.com/wp-content/uploads/2019/03/Fontaine-bipolar-precordial-lead-electrocardiography-F-ECG-768x607.png "Fontaine Lead Placement")

If you purchased the DFRobot module I did, then here is the color guide:
- RA: Red
- LA: Green
- LL: Yellow
