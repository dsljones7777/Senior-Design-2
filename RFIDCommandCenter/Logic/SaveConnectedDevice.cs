using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class SaveConnectedDevice
    {
        public void Execute(string serialNumber, DataContext context)
        {
            if (context.ConnectedDevices.Any(x => x.SerialNumber == serialNumber))
                throw new Exception("There is already a connected device with that serial number");

            context.ConnectedDevices.Add(new Models.ConnectedDevice { SerialNumber = serialNumber });
            context.SaveChanges();
        }
    }
}
