using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class ViewLostTags
    {
        public List<SharedLib.SharedModels.ViewTagModel> Execute(DataContext context)
        {
            var tagList = context.Tags.Where(t => t.LostTag).ToList();

            var locationList = context.Locations.ToList();

            return tagList.ConvertAll(x => new SharedLib.SharedModels.ViewTagModel
            {
                TagName = x.Name,
                TagNumber = Convert.ToBase64String(x.TagNumber),
                LastLocation = locationList.FirstOrDefault(l => l.ID == x.LastLocation).LocationName
            });
        }
    }
}
