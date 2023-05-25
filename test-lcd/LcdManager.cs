using GHIElectronics.TinyCLR.Display.HD44780;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace test_lcd
{
    enum EChangeMode 
    {
        HOUR,
        MINUTE
    }

    internal class ChangeMode
    {
        public static int hour = 0;
        public static int min = 0;
    }

    internal class LcdManager
    {
        public static EChangeMode changeMode = EChangeMode.HOUR;
        public static void DrawPotentiometer(DisplayHD44780 Display)
        {
            Display.Print(new Potentiometer().GetValue().ToString());
        }

        public static String FormatHour(DateTime time)
        {
            StringBuilder sb = new StringBuilder();

            if (time.Hour.ToString().Length < 2)
            {
                sb.Append("0" + time.Hour.ToString());
            } 
            else
            {
                sb.Append(time.Hour.ToString());
            }

            sb.Append(':');

            if (time.Minute.ToString().Length < 2) { 
                sb.Append("0" + time.Minute.ToString());
            }
            else
            {
                sb.Append(time.Minute.ToString());
            }

            return sb.ToString();
        }
        
        public static void DrawScreen(int n, DisplayHD44780 Display)
        {
            switch (n)
            {
                case 0:
                    Display.SetCursor(2, 3);
                    Display.Print(FormatHour(DateTime.UtcNow));
                    Display.SetCursor(3, 3);

                    DateTime t = TakingTimeManager.GetNextTakingTime(DateTime.UtcNow);
                    
                    Display.Print("Next Take: " + FormatHour(t));
                    break;
                case 1:
                    Display.SetCursor(2, 3);
                    Display.Print("Menu");
                    Display.SetCursor(3, 3);
                    Display.Print("Change Next Take");
                    break;
                case 2:
                    Display.SetCursor(2, 3);
                    Display.Print("Change Next Take");
                    Display.SetCursor(3, 3);
                    DrawNextTake(Display);
                    // Display.Print("XX:XX");
                    break;
                case 3:
                    Display.SetCursor(2, 3);
                    Display.Print("Modify Next Take");
                    Display.SetCursor(3, 3);
                    Display.Print("Ok ? Y / N");
                    break;
                case 4:
                    Display.SetCursor(2, 3);
                    Display.Print("Next Take");
                    Display.SetCursor(3, 3);
                    Display.Print("Modified");
                    break;
                default:
                    break;
            }
        }

        public static void DrawNextTake(DisplayHD44780 Display)
        {
            Display.Clear();
            Thread.Sleep(1000);
            Potentiometer potentiomerter = Program.potentiometer;

            /*int hour = TakingTimeManager.nextHour.Hour;
            int min = TakingTimeManager.nextHour.Minute;*/

            if (changeMode == EChangeMode.HOUR)
            {
                float value = potentiomerter.GetValueF(24);
                ChangeMode.hour = (int)value;

                if (ChangeMode.hour >= 24) ChangeMode.hour = 23;

                TakingTimeManager.lNextHour = new DateTime(2020, 1, 1, ChangeMode.hour, 0, 0);
                //TakingTimeManager.nextHour = new DateTime(2020, 1, 1, hour, 0, 0);
            }

            else
            {
                int value = (int)potentiomerter.GetValueF(3);
                ChangeMode.min = (int)value * 15;
                
                TakingTimeManager.lNextHour = new DateTime(2020, 1, 1, ChangeMode.hour, ChangeMode.min, 0);
                //TakingTimeManager.nextHour = new DateTime(2020, 1, 1, hour, min, 0);
            }

            Display.Print(FormatHour(new DateTime(2020, 1, 1, ChangeMode.hour, ChangeMode.min, 0)));
            //Display.Print(hour + ":" + min);
        }
    }
}
