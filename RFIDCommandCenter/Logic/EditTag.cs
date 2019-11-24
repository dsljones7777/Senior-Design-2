using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class EditTag
    {
        public void Execute(byte[] tagNumber, string tagName, bool? lost,bool? guest, DataContext context)
        {
            var existingTag = context.Tags.SingleOrDefault(x => x.TagNumber == tagNumber || x.Name == tagName);
            if (existingTag == null)
                throw new Exception("No tag with the specified TagNumber/Name exists in the system.");
            if (tagNumber != null)
                existingTag.TagNumber = tagNumber;
            if (tagName != null)
                existingTag.Name = tagName;
            if(lost.HasValue)
                existingTag.LostTag = lost.Value;
            if (guest.HasValue)
                existingTag.Guest = guest.Value;
            context.SaveChanges();
        }
    }
}
