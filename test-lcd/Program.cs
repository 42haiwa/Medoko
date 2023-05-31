using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

using GHIElectronics.TinyCLR.Display.HD44780;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Devices.Rtc;
using GHIElectronics.TinyCLR.Native;

namespace test_lcd
{
    internal class Program
    {
        static DisplayHD44780 Display;
        static GpioController gpio;

        public static Potentiometer potentiometer;

        // numéro de menu
        static int n_menu = 0;

        static void Main()
        {
            // Initialiser les broches GPIO
            initDisplay();

            LcdManager.DrawScreen(n_menu, Display);

            gpio = GpioController.GetDefault();

            potentiometer = new Potentiometer();

            var button = gpio.OpenPin(SC20100.GpioPin.PB10);
            button.SetDriveMode(GpioPinDriveMode.InputPullUp);
            button.ValueChangedEdge = GpioPinEdge.FallingEdge;// | GpioPinEdge.RisingEdge;
            button.ValueChanged += Button_ValueChanged;

            var button2 = gpio.OpenPin(SC20100.GpioPin.PC0);
            button2.SetDriveMode(GpioPinDriveMode.InputPullUp);
            button2.ValueChangedEdge = GpioPinEdge.FallingEdge;
            button2.ValueChanged += Button2_ValueChanged;

            Buzzer buzzer = new Buzzer();
            Servomotor servomotor = new Servomotor();    

            Thread.Sleep(1000);

            //bool notTake = false;

            var rtc = RtcController.GetDefault();
            var ndt = new DateTime(2020, 1, 1, 7, 59, 50);
            rtc.Now = ndt;
            SystemTime.SetTime(rtc.Now);

            bool turning = true;

            FSR02 fsr = new FSR02(); // Capteur de pesée
            WifiDriver wifiDriver = new WifiDriver();

            while (true) 
            {
                LcdManager.DrawScreen(n_menu, Display);
                Thread.Sleep(1000);
                
                fsr.ReadValue();

                if (/*notTake || */TakingTimeManager.TakingAlert(DateTime.UtcNow))
                {
                    buzzer.alert(440, 500);

                    if (turning)
                    {
                        servomotor.rotate();
                    }

                    turning = false;

                    if (DateTime.UtcNow.Second - Buzzer.buzzerTimer.Second >= -50)
                    {
                        TakingTimeManager.nextHour = TakingTimeManager.takingTimes[TakingTimeManager.GetRangeNextTakingTime(DateTime.UtcNow)];
                        TakingTimeManager.ModifyNextHour(DateTime.UtcNow);
                    }

                } 
                else
                {
                    turning = true;
                    Buzzer.buzzerTimer = DateTime.UtcNow;
                }
                
                //buzzer.playMusic();
            }

            void initDisplay()
            {
                gpio = GpioController.GetDefault();

                /* HD44780 pinnig with gadgeteer adapter
                 * pin1 : 3.3V
                 * pin2 : 5V
                 * pin3 : E
                 * pin4 : RS
                 * pin5 : D4
                 * pin6 : D7
                 * pin7 : D5
                 * pin8 : Backlight
                 * pin9 : D6
                 * pin10 : GND
                 */

                
                var D4 = new GpioPin[4]{
                gpio.OpenPin(SC20100.GpioPin.PA1), //D4-P5
                gpio.OpenPin(SC20100.GpioPin.PA9),  //D5-P7
                gpio.OpenPin(SC20100.GpioPin.PA10),  //D6-P9
                gpio.OpenPin(SC20100.GpioPin.PB0) };//D7-P6
                var E = gpio.OpenPin(SC20100.GpioPin.PA2);
                var RS = gpio.OpenPin(SC20100.GpioPin.PC7);
                
                 
                

                Display = new DisplayHD44780(D4, E, RS);

                Display.Clear();
            }
        }

        private static void Button2_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            switch (n_menu) 
            {
                case 1:
                    n_menu = 2;
                    break;
                default:
                    break;
            }
        }

        private static void Button_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (n_menu >= 1)
            { 
                if (n_menu == 2 && LcdManager.changeMode == EChangeMode.HOUR) 
                {
                    LcdManager.changeMode = EChangeMode.MINUTE;
                    return;
                }  

                else if (n_menu == 2 && LcdManager.changeMode == EChangeMode.MINUTE)
                {
                    LcdManager.changeMode = EChangeMode.HOUR;
                    TakingTimeManager.nextHour = TakingTimeManager.lNextHour;
                    TakingTimeManager.ModifyNextHour(DateTime.UtcNow);
                    n_menu = 0;
                    Display.Clear();
                    Thread.Sleep(1000);
                    return;
                }

                n_menu++;
                n_menu = 0;
                Display.Clear();
                Thread.Sleep(1000);
                return;
            }
            n_menu++;
            Display.Clear();
            Thread.Sleep(1000);
        }
    }
}


