using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class AltSaveAllowedLocations
    {
        public void Execute(string tagName, List<string> locationNames, DataContext context)
        {
            var locationIDs = context.Locations.Where(l => locationNames.Contains(l.LocationName)).Select(l => l.ID).ToList();

            if(locationIDs == null)
                throw new UIClientException("There is no location with any of the specified location names");

            var tag = context.Tags.SingleOrDefault(t => t.Name == tagName);

            if (tag == null)
                throw new UIClientException("There is no tag with the specified tag name");

            var allowedLocations = context.AllowedLocations.Where(a => locationIDs.Contains(a.LocationID)).ToList();
            var allForTag = context.AllowedLocations.Where(a => a.TagID == tag.ID).ToList();
            if(allForTag != null)
            {
                context.AllowedLocations.RemoveRange(allForTag);
                context.SaveChanges();
            }
            //if(allowedLocations != null)
            //{
            //    context.AllowedLocations.RemoveRange(allowedLocations);
            //    context.SaveChanges();
            //}

            var allowedLocationsToSave = locationIDs.ConvertAll(l => new Models.TagLocationBridge { TagID = tag.ID, LocationID = l });

            context.AllowedLocations.AddRange(allowedLocationsToSave);
            context.SaveChanges();
                
        }
    }
}
