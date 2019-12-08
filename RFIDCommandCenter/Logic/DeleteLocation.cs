using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class DeleteLocation
    {
        public List<string> Execute(string locationName, DataContext context)
        {
            var existingLocation = context.Locations.SingleOrDefault(l => l.LocationName == locationName);

            if (existingLocation == null)
                throw new UIClientException("There is no location with that location name/reader serial combo.");

            var allowedLocations = context.AllowedLocations.Where(l => l.LocationID == existingLocation.ID).ToList();

            var tagsInLocation = context.Tags.Where(t => t.LastLocation == existingLocation.ID).ToList();

            if(tagsInLocation != null)
            {
                tagsInLocation.ForEach(x => { x.LastLocation = null; x.InLocation = false; } );
                context.SaveChanges();
            }
            
            if (allowedLocations != null)
            {
                context.AllowedLocations.RemoveRange(allowedLocations);
                context.SaveChanges();
            }
            List<string> returnval = new List<string>();
            returnval.Add(existingLocation.ReaderSerialIn);
            if (!string.IsNullOrWhiteSpace(existingLocation.ReaderSerialOut))
                returnval.Add(existingLocation.ReaderSerialOut);
            context.Locations.Remove(existingLocation);
            context.SaveChanges();
            return returnval;
        }
    }
}
