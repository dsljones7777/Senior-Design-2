using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class RemoveAllConnectedDevices
    {
        public void Execute(DataContext context)
        {
            var connectedDevieces = context.ConnectedDevices.ToList();

            context.ConnectedDevices.RemoveRange(connectedDevieces);

            context.SaveChanges();
        }
    }
}
