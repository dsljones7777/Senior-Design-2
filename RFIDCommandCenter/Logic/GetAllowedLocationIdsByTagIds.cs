using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class GetAllowedLocationIdsByTagIds
    {
        public List<int> Execute(int tagId, DataContext dataContext)
        {
            return dataContext.AllowedLocations.Where(x => x.TagID == tagId).Select(x => x.LocationID).ToList();
        }
    }
}
