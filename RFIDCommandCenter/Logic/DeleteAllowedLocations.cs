using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class DeleteAllowedLocations
    {
        public void Execute(string tagName, string locationName, DataContext context)
        {
            var tagIDs = context.Tags.Where(t => t.Name == tagName).Select(t => t.ID).ToList();
            var locationIDs = context.Locations.Where(l => l.LocationName == locationName).Select(l => l.ID).ToList();

            var allowedLocationsToDelete = context.AllowedLocations.Where(a => tagIDs.Contains(a.TagID) || locationIDs.Contains(a.LocationID)).ToList();

            if (allowedLocationsToDelete == null)
                throw new UIClientException("No allowed location with that Tag/Location combo exists");

            context.AllowedLocations.RemoveRange(allowedLocationsToDelete);

            context.SaveChanges();

        }
    }
}
