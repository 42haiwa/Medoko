using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Adc;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace test_lcd
{
    internal class Potentiometer
    {
        private AdcController adc;
        private AdcChannel analog;
        public Potentiometer() 
        {
            this.adc = AdcController.FromName(SC20100.Adc.Controller1.Id);
            this.analog = adc.OpenChannel(SC20100.Adc.Controller1.PA7);
        }

        public int GetValue(int n = 360)
        {
            return (int)Math.Floor(this.analog.ReadRatio() * n);
        }

        public float GetValueF(int n)
        {
            return (float) this.analog.ReadRatio() * n;
        }
    }
}
