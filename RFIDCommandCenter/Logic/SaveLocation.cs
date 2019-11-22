using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class SaveLocation
    {
        public void Execute(string locationName, string readerSerialIn, string readerSerialOut, DataContext context)
        {
            context.Locations.Add(new Location
            {
                LocationName = locationName,
                ReaderSerialIn = readerSerialIn,
                ReaderSerialOut = readerSerialOut,
            });


            context.SaveChanges();
        }
    }
}
