using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Drivers.Motor.Servo;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace test_lcd
{
    internal class Servomotor
    {
        private PwmController pwmController;
        private PwmChannel pin;
        private ServoController servo;
        public Servomotor() 
        {
            this.pwmController = PwmController.FromName(SC20100.Timer.Pwm.Software.Id);
            this.pin = pwmController.OpenChannel(SC20100.GpioPin.PA0);
            this.servo = new ServoController(this.pwmController, this.pin);
        }

        public void testServo()
        {
            Debug.WriteLine("Moteur activé");

            servo.Set(0); // 0 degree

            Thread.Sleep(2000);

            servo.Stop();
        }
    }
}
