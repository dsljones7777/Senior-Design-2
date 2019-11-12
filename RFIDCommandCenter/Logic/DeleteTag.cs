using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class DeleteTag
    {
        public void Execute(byte[] tagNumber, DataContext context)
        {
            var tagToDelete = context.Tags.SingleOrDefault(t => t.TagNumber == tagNumber);

            if (tagToDelete == null)
                throw new ApplicationException("A Tag that ID does not exist");

            tagToDelete.Active = false;

            context.SaveChanges();
        }
    }
}
