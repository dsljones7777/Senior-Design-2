using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class TagLeave
    {
        public void Execute(byte[] tagNum, DataContext context)
        {
            var tagLeaving = context.Tags.SingleOrDefault(t => t.TagNumber == tagNum);

            if (tagLeaving == null)
                throw new ApplicationException("The Tag marked for leaving does not exist in the system");
            
            tagLeaving.InLocation = false;

            context.SaveChanges();
        }
    }
}
