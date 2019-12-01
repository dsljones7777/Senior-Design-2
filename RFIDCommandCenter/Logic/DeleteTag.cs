using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SharedLib.Network.UIClientConnection;

namespace RFIDCommandCenter.Logic
{
    public class DeleteTag
    {
        public void Execute(string tagName, DataContext context)
        {
            var tagToDelete = context.Tags.SingleOrDefault(t => t.Name == tagName);

            if (tagToDelete == null)
                throw new UIClientException("A Tag that ID does not exist");

            var allowedLocationsWithTagId = context.AllowedLocations.Where(l => l.TagID == tagToDelete.ID).ToList();

            context.AllowedLocations.RemoveRange(allowedLocationsWithTagId);
            context.SaveChanges();
            
            context.Tags.Remove(tagToDelete); 

            context.SaveChanges();
        }
    }
}
