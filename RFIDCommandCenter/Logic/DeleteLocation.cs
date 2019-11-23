using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class DeleteLocation
    {
        public void Execute(string locationName, DataContext context)
        {
            var existingLocation = context.Locations.SingleOrDefault(l => l.LocationName == locationName);

            if (existingLocation == null)
                throw new Exception("There is no location with that location name/reader serial combo.");

            var allowedLocations = context.AllowedLocations.Where(l => l.LocationID == existingLocation.ID).ToList();

            if(allowedLocations != null)
            {
                context.AllowedLocations.RemoveRange(allowedLocations);
            }

            var tagsInLocation = context.Tags.Where(t => t.LastLocation == existingLocation.ID).ToList();

            if(tagsInLocation != null)
            {
                tagsInLocation.ForEach(x => x.LastLocation = null);
            }

            context.Locations.Remove(existingLocation);
            context.SaveChanges();
        }
    }
}
