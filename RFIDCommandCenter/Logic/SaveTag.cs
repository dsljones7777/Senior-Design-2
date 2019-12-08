using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class SaveTag
    {
        public void Execute(byte[] tagNumber, string name, bool guest, DataContext context)
        {
            var tag = context.Tags.FirstOrDefault(t => t.TagNumber == tagNumber);

            if (tag != null)
                throw new UIClientException("A tag with that number already exists.");

            var tageEntity = new Tag
            {
                TagNumber = tagNumber,
                Name = name,
                Guest = guest
            };

            context.Tags.Add(tageEntity);
            context.SaveChanges();
        }

    }
}
