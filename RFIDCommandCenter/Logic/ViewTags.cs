using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class ViewTags
    {
        public List<SharedLib.SharedModels.ViewTagModel> Execute(DataContext context)
        {
            var tagList = context.Tags.ToList();

            var locationList = context.Locations.ToList();

            return tagList.ConvertAll(x => new SharedLib.SharedModels.ViewTagModel
            {
                TagName = x.Name,
                TagNumber =x.TagNumber,
                LastLocation = locationList.FirstOrDefault(l => l.ID == x.LastLocation)?.LocationName,
                LostTag = x.LostTag,
                GuestTag = x.Guest
            });            
        }
    }
}
