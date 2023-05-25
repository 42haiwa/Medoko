using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace test_lcd
{
    internal class TakingTimeManager
    {
        public static DateTime[] takingTimes = {
             new DateTime(2020, 1, 1, 8, 0, 0),
             new DateTime(2020, 1, 1, 12, 0, 0),
             new DateTime(2020, 1, 1, 19, 0, 0),
        };

        public static DateTime nextHour = takingTimes[0];
        public static DateTime lNextHour;

        public static bool notTake = false;

        public static bool takeNow = false;

        public static DateTime[] getTakingTimes()
        {
            return takingTimes;
        }

        public static DateTime GetNextTakingTime(DateTime current)
        {
            for (int i = 0; i < takingTimes.Length; i++) 
            {
                if (current.Hour >= takingTimes[i].Hour && current.Minute >= takingTimes[i].Minute) 
                {
                    continue;
                }

                return takingTimes[i];
            }
            return takingTimes[0];
        }

        public static bool TakingAlert(DateTime current)
        {
            if (current.Ticks >= nextHour.Ticks)
                return true;

            return false;
        }

        public static int GetRangeNextTakingTime(DateTime current)
        {
            for (int i = 0; i < takingTimes.Length; i++)
            {
                if (current.Hour >= takingTimes[i].Hour && current.Minute >= takingTimes[i].Minute)
                {
                    continue;
                }
                return i;
            }
            return 0;
        }

        public static bool IsTake(DateTime current)
        {
            return !notTake;
        }
        public static void ModifyNextHour(DateTime current)
        {
            int n = GetRangeNextTakingTime(current);

            if (n + 1 < takingTimes.Length)
            {
                if (!(nextHour.Ticks > takingTimes[n + 1].Ticks) && !(nextHour.Ticks < current.Ticks))
                {
                    takingTimes[n] = nextHour;
                    Debug.WriteLine("Hour modified !");
                    return;
                }
            }

            Debug.WriteLine("Hour not modified !");
            // Vérifier que les heures ne se chevauchent pas
        }
    }
}
