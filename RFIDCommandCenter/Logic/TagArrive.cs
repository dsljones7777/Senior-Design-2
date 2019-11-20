using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class TagArrive
    {
        public void Execute(byte[] tag, DataContext context)
        {
            var tagInSystem = context.Tags.SingleOrDefault(t => t.TagNumber == tag);

            if (tagInSystem == null)
                throw new ApplicationException("Tag does not exist in the system");

            tagInSystem.InLocation = true;

            context.SaveChanges();
        }
    }
}
