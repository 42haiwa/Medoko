using GHIElectronics.TinyCLR.Devices.Adc;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace test_lcd
{
    internal class FSR02
    {
        private AdcController adc;
        private AdcChannel channel;
        private double value;
        private bool isTake;

        private CSVManager csvM;

        public FSR02() 
        {
            this.adc = AdcController.FromName(SC20100.Adc.Controller1.Id);
            this.channel = adc.OpenChannel(SC20100.Adc.Controller1.PA4);
            this.value = channel.ReadRatio();
            this.csvM = new CSVManager();
            this.isTake = false;
        }

        public void ReadValue()
        {
            this.value = this.channel.ReadRatio(); // Valeur que retourne le capteur de force

            if (this.value < 0.04) // Condition pour savoir si les médicaments sont pris ou non
            {
                if (!this.isTake)
                {
                    csvM.Write();
                    csvM.Read();
                    this.isTake = true;
                }

                Debug.WriteLine("Médicament pris"); // Afficher dans la console de débug que le médicament a été pris
            }

            else
            {
                if (this.isTake)
                {
                    this.isTake = false;
                }

                Debug.WriteLine("Médicament non pris"); // Afficher dans la console de débug que le médicament n'a pas été pris
            }

            //Thread.Sleep(100); //On ralentit la vitesse de travail du MicroContrôleur
        }
    }
}
