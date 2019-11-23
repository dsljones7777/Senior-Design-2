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

            return new List<SharedLib.SharedModels.ViewTagModel>();
        }
    }
}
