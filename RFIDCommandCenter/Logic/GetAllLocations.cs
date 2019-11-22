using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;

namespace RFIDCommandCenter.Logic
{
    public class GetAllLocations
    {
        public List<NetworkLib.LocationObjList> Execute(DataContext context)
        {
            return context.Locations.Select(n => new NetworkLib.LocationObjList{LocationName = n.LocationName, ReaderSerialIn = n.ReaderSerialIn }).ToList();

            
        }
    }
}
