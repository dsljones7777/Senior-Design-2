using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class TagArrive
    {
        public void Execute(byte[] tag, string deviceSerial, DataContext context)
        {
            var tagInSystem = context.Tags.SingleOrDefault(t => t.TagNumber == tag);

            if (tagInSystem == null)
                throw new ApplicationException("Tag with that tag number does not exist in the system");

            var location = context.Locations.FirstOrDefault(x => x.ReaderSerialIn == deviceSerial || x.ReaderSerialOut == deviceSerial);

            var isTagAllowedInLocation = context.AllowedLocations.Any(x => x.TagID == tagInSystem.ID && x.LocationID == location.ID);

            if (!isTagAllowedInLocation)
                throw new ApplicationException("Tag sent is not allowed in that location");
            
            tagInSystem.InLocation = true;
            tagInSystem.LastLocation = location.ID;

            context.SaveChanges();
        }
    }
}
