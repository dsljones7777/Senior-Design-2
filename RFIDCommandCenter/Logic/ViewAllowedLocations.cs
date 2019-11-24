using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedLib.SharedModels;

namespace RFIDCommandCenter.Logic
{
    public class ViewAllowedLocations
    {
        public List<ViewAllowedLocationsModel> Execute(string tagName, DataContext context)
        {
            var tagId = context.Tags.Where(t => t.Name == tagName).First().ID;
            var allowedTagLocationIDs = context.AllowedLocations.Where(l => l.TagID == tagId).Select(x => x.LocationID).ToList();

            var allLocations = context.Locations.ToList();

            return allLocations.ConvertAll(l => new ViewAllowedLocationsModel
            {
                ID = l.ID,
                LocationName = l.LocationName,
                TagAllowedInLoc = allowedTagLocationIDs.Contains(l.ID)
            }).ToList();
        }
    }
}
