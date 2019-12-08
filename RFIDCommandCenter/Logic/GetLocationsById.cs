using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class GetLocationsById
    {
        public Location Execute(string locationName, string readerSerialIn, string readerSerialOut, DataContext context)
        {
            if (string.Equals(readerSerialIn, readerSerialIn, StringComparison.InvariantCultureIgnoreCase))
                throw new UIClientException("The Reader Serial In cannot be the same as the Reader Serial Out");

             return context.Locations.SingleOrDefault(l => l.LocationName == locationName);
        }
    }
}
