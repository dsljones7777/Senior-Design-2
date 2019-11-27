using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class SaveAllowedTagLocation
    {
        public void Execute(string tagName, List<string> locationName, DataContext context)
        {
            var locationIds = context.Locations.Where(l => locationName.Contains(l.LocationName)).Select(l => l.ID).ToList();

            if (locationIds == null)
                throw new UIClientException("There is no location with any of the specified location names");

            var tag = context.Tags.SingleOrDefault(t => t.Name == tagName);

            if (tag == null)
                throw new UIClientException("There is no tag with the specified tag name");

            var allowedLocations = locationIds.ConvertAll(x => new Models.TagLocationBridge { LocationID = x, TagID = tag.ID });

            if (allowedLocations.Any(x => x.TagID == tag.ID && locationIds.Contains(x.LocationID))) 
                throw new UIClientException("An allowed location with that ID and Location already exists");

            context.AllowedLocations.AddRange(allowedLocations);

            context.SaveChanges();
        }
    }
}
