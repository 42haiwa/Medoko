using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace test_lcd
{
    internal class Buzzer
    {
        const int NOTE_C = 261;
        const int NOTE_D = 294;
        const int NOTE_E = 330;
        const int NOTE_F = 349;
        const int NOTE_G = 392;

        const int WHOLE_DURATION = 2000;
        const int EIGHTH = WHOLE_DURATION / 8;
        const int QUARTER = WHOLE_DURATION / 4;
        const int QUARTERDOT = WHOLE_DURATION / 3;
        const int HALF = WHOLE_DURATION / 2;
        const int WHOLE = WHOLE_DURATION;

        public static DateTime buzzerTimer = DateTime.MinValue;

        private static int[] note = { NOTE_E, NOTE_E, NOTE_F, NOTE_G, NOTE_G, NOTE_F, NOTE_E,
                          NOTE_D, NOTE_C, NOTE_C, NOTE_D, NOTE_E, NOTE_E, NOTE_D,
                          NOTE_D, NOTE_E, NOTE_E, NOTE_F, NOTE_G, NOTE_G, NOTE_F,
                          NOTE_E, NOTE_D, NOTE_C, NOTE_C, NOTE_D, NOTE_E, NOTE_D,
                          NOTE_C, NOTE_C};

        private static int[] duration = { QUARTER, QUARTER, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTER, QUARTER, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTERDOT, EIGHTH, HALF, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTER, QUARTER, QUARTER, QUARTER, QUARTER, QUARTER,
                              QUARTER, QUARTER, QUARTERDOT, EIGHTH, WHOLE};

        private PwmController controller;
        private PwmChannel buzzer;
        public Buzzer() 
        {
            // PWM
            this.controller = PwmController.FromName(SC20100.Timer.Pwm.Controller3.Id);
            this.buzzer = controller.OpenChannel(SC20100.Timer.Pwm.Controller3.PB1);
            this.buzzer.SetActiveDutyCyclePercentage(0.5);
        }

        public void playMusic()
        {
            buzzer.Start();

            for (int i = 0; i < note.Length; i++)
            {
                controller.SetDesiredFrequency(note[i]);
                Thread.Sleep(duration[i]);
            }

            buzzer.Stop();

            Thread.Sleep(1000);
        }

        public void alert(int f, int s)
        {
            this.buzzer.Start();
            this.controller.SetDesiredFrequency(f);
            Thread.Sleep(s);
            this.buzzer.Stop();
            Thread.Sleep(s);
        }
    }
}
