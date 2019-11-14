using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class GetTagIdsByLocationId
    {
        public List<int> Execute(int locationId, DataContext context)
        {
            return context.Tags.Where(t => t.LastLocation == locationId).Select(x => x.ID).ToList();
        }
    }
}
