using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.IO;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Pins;
using System.IO;
using System.Diagnostics;

namespace test_lcd
{
    internal class CSVManager
    {
        private StorageController sd;
        private IDriveProvider drive;
        private FileStream fs;

        private const bool ERASE_LOG = true;

        public CSVManager()
        {
            this.sd = StorageController.FromName(SC20100.StorageController.SdCard);
            this.drive = FileSystem.Mount(this.sd.Hdc);

            if (ERASE_LOG)
            {
                File.Delete($@"{this.drive.Name}take_log.csv");
                this.fs = new FileStream($@"{this.drive.Name}take_log.csv", FileMode.OpenOrCreate);
            }
            else
            {
                this.fs = new FileStream($@"{this.drive.Name}take_log.csv", FileMode.OpenOrCreate);
            }
        }

        // DATA: heure_de_prise_effective, heure_de_prise_original

        public void Write()
        {
            byte[] data = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString() + Environment.NewLine);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            FileSystem.Flush(sd.Hdc);
            Debug.WriteLine("Writed");
        }

        public void Read()
        {
            Debug.WriteLine("Read SD");
            byte[] data = File.ReadAllBytes($@"{this.drive.Name}take_log.csv");
            string s = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);

            for (int i = 0; i < data.Length; ++i)
            {
                Debug.WriteLine(s);
            }
        }
    }
}
