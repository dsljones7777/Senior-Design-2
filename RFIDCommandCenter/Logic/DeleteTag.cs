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

            var allowedLocationsWithTagId = context.AllowedLocations.Where(l => l.TagID == tagToDelete.ID).ToList();

            context.AllowedLocations.RemoveRange(allowedLocationsWithTagId);
            
            context.Tags.Remove(tagToDelete); 

            context.SaveChanges();
        }
    }
}
