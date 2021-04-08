
# DreamScreenNet

A .NET Standard 2.0 library for LIFX.
Supports .NET, UWP, Xamarin iOS, Xamarin Android, and any other .NET Platform that has implemented .NET Standard 2.0+.

## Sponsoring

If you like this library and use it a lot, consider sponsoring me. Anything helps and encourages me to keep going.

See here for details: https://github.com/sponsors/d8ahazard


### NuGet

Get the [Nuget package here](http://www.nuget.org/packages/DreamScreenNet/):
```
PM> Install-Package DreamScreenNet 
```

Tested with LIFX 2.0 Firmware.

Based on the unofficial [DreamScreen protocol docs](https://github.com/d8ahazard/DreamscreenDocs/blob/master/docs/DreamScreen%20V2%20WiFi%20UDP%20Protocol%20Rev%205.pdf)

####Usage

```csharp
	client = new DreamScreenClient();
	client.Discovered += Client_DeviceDiscovered;
	client.Lost += Client_DeviceLost;
	client.StartDeviceDiscovery();

...

	private async void Client_DeviceDiscovered(object sender, DreamScreenClient.DeviceDiscoveryEventArgs e)
	{
		var dsDev = e.Device;
		await client.SetMode(dsDev, DeviceMode.Video); //Set device to video mode
		await client.SetColorAsync(bulb, Colors.Red, 2700); //Set color to Red and 2700K Temperature			
	}

```
See the sample apps for more examples.

Note: Be careful with sending too many messages to your bulbs - LIFX recommends a max of 20 messages pr second pr bulb.
This is especially important when using sliders to change properties of the bulb - make sure you use a throttling
mechanism to avoid issues with your bulbs. See the sample app for one way to handle this.
