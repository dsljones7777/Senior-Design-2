using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class TagArrive
    {
        public bool Execute(byte[] tag, string deviceSerial, DataContext context)
        {
            var tagInSystem = context.Tags.SingleOrDefault(t => t.TagNumber == tag);

            if (tagInSystem == null)
                return false;

            var location = context.Locations.FirstOrDefault(x => x.ReaderSerialIn == deviceSerial || x.ReaderSerialOut == deviceSerial);

            var isTagAllowedInLocation = context.AllowedLocations.Any(x => x.TagID == tagInSystem.ID && x.LocationID == location.ID);

            if (!isTagAllowedInLocation)
                return false;

            if (tagInSystem.InLocation == null || !tagInSystem.InLocation.Value)
                tagInSystem.InLocation = true;
            else
                tagInSystem.InLocation = false;

            tagInSystem.LastLocation = location.ID;

            context.SaveChanges();

            return true;
        }
    }
}
