using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class SaveAllowedTagLocation
    {
        public void Execute(int tagId, int locationId, DataContext context)
        {
            var allowedLocation = context.AllowedLocations.SingleOrDefault(a => a.TagID == tagId && a.LocationID == locationId);

            if (allowedLocation != null)
                throw new UIClientException("An allowed location with that ID and Location already exists");

            context.AllowedLocations.Add(new Models.TagLocationBridge { TagID = tagId, LocationID = locationId });

            context.SaveChanges();
        }
    }
}
