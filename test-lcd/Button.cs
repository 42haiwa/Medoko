using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace test_lcd
{
    internal class Button
    {
        private GpioPin pin;
        public Button(GpioController gpio)
        {
            gpio = GpioController.GetDefault();
            this.pin = gpio.OpenPin(SC20100.GpioPin.PB10);
            this.pin = gpio.OpenPin(FEZDuino.GpioPin.ButtonApp);
            this.pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
        }

        public bool isPressed()
        {
            if (pin.Read() == GpioPinValue.Low)
            {
                return true;
            }
            return false;
        }
    }
}
