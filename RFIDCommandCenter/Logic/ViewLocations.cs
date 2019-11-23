using System.Collections.Generic;
using System.Linq;

namespace RFIDCommandCenter.Logic
{
    public class ViewLocations
    {
        public List<SharedLib.SharedModels.LocationModel> Execute(DataContext context)
        {
            return context.Locations.Select(l => 
            new SharedLib.SharedModels.LocationModel
            {
                LocationName = l.LocationName, ReaderSerialIn = l.ReaderSerialIn, ReaderSeralOut = l.ReaderSerialOut
            }).ToList();
        }
    }
}
