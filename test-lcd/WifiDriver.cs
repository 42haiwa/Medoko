using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Networking.Net;
using GHIElectronics.TinyCLR.Devices.Rtc;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace test_lcd
{
    internal class WifiDriver
    {
        private int enablePinNumber;
        private int chipSelectPinNumber;
        private int irqPinNumber;
        private int resetPinNumber;
        private string spiControllerName;
        private string gpioControllerName;
        private GpioPin enablePin;

        static bool isReady = false;

        const string _SSID = "OnePlus Nord";
        const string _PASSWORD = "jordan12";

        public WifiDriver() 
        {
            this.enablePinNumber = FEZDuino.GpioPin.WiFiEnable;
            this.chipSelectPinNumber = FEZDuino.GpioPin.WiFiChipselect;
            this.irqPinNumber = FEZDuino.GpioPin.WiFiInterrupt;
            this.resetPinNumber = FEZDuino.GpioPin.WiFiReset;
            this.spiControllerName = FEZDuino.SpiBus.WiFi;
            this.gpioControllerName = SC20100.GpioPin.Id;

            this.enablePin = GpioController.GetDefault().OpenPin(enablePinNumber);
            enablePin.SetDriveMode(GpioPinDriveMode.Output);
            enablePin.Write(GpioPinValue.High);

            SpiNetworkCommunicationInterfaceSettings netInterfaceSettings =
                new SpiNetworkCommunicationInterfaceSettings();

            var chipselect = GpioController.GetDefault().OpenPin(chipSelectPinNumber);

            var settings = new SpiConnectionSettings()
            {
                ChipSelectLine = chipselect,
                ClockFrequency = 4000000,
                Mode = SpiMode.Mode0,
                ChipSelectType = SpiChipSelectType.Gpio,
                ChipSelectHoldTime = TimeSpan.FromTicks(10),
                ChipSelectSetupTime = TimeSpan.FromTicks(10)
            };

            // netInterfaceSettings
            netInterfaceSettings.SpiApiName = spiControllerName;
            netInterfaceSettings.SpiSettings = settings;

            netInterfaceSettings.GpioApiName = gpioControllerName;

            netInterfaceSettings.InterruptPin = GpioController.GetDefault().OpenPin(irqPinNumber);
            netInterfaceSettings.InterruptEdge = GpioPinEdge.FallingEdge;
            netInterfaceSettings.InterruptDriveMode = GpioPinDriveMode.InputPullUp;

            netInterfaceSettings.ResetPin = GpioController.GetDefault().OpenPin(resetPinNumber);
            netInterfaceSettings.ResetActiveState = GpioPinValue.Low;

            // Wifi setting
            var wifiSettings = new WiFiNetworkInterfaceSettings()
            {
                Ssid = _SSID,
                Password = _PASSWORD,
            };

            wifiSettings.DhcpEnable = true;
            wifiSettings.DynamicDnsEnable = true;

            var networkController = NetworkController.FromName(SC20260.NetworkController.ATWinc15x0);

            networkController.SetInterfaceSettings(wifiSettings);
            networkController.SetCommunicationInterfaceSettings(netInterfaceSettings);
            networkController.SetAsDefaultController();

            networkController.NetworkAddressChanged += NetworkController_NetworkAddressChanged;

            networkController.NetworkLinkConnectedChanged += NetworkController_NetworkLinkConnectedChanged;

            networkController.Enable();

            // Network is ready to use
            var t = new Thread(this.NetworkThread);
            t.Start();
        }

        private void NetworkThread()
        {
            // Network is ready to use
            Debug.Write("[" + DateTime.UtcNow.ToString() + "] Network Thread Launched !");
            while (!isReady)
                Thread.Sleep(100);
            Debug.Write("test");

            //Create a listener.
            HttpListener listener = new HttpListener("http", 80);

            listener.Start();
            System.Diagnostics.Debug.WriteLine("Listening...");

            var clientRequestCount = 0;

            while (true)
            {
                //Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();

                //Obtain a response object.
                HttpListenerResponse response = context.Response;

                //Construct a response.                
                var responseString = string.Format("<HTML><BODY> I am TinyCLR OS Server. " +
                    "Client request count: {0}</BODY></HTML>", ++clientRequestCount);

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                //Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;

                output.Write(buffer, 0, buffer.Length);

                //You must close the output stream.
                output.Close();
            }

            listener.Stop();
        }

        private void NetworkController_NetworkLinkConnectedChanged
                (NetworkController sender, NetworkLinkConnectedChangedEventArgs e)
        {
            Debug.Write("connected");
        }

        private void NetworkController_NetworkAddressChanged
            (NetworkController sender, NetworkAddressChangedEventArgs e)
        {
            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();
            Debug.WriteLine("IP: " + address[0] + "." + address[1] + "." + address[2] +
                "." + address[3]);

            if (address[0] != 0)
                isReady = true;
        }
    }
}
