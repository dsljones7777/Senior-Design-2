﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class SaveTag
    {
        public void Execute(byte[] tagNumber, string name, int lastLocation, bool? inLocation, DataContext context)
        {
            var tag = context.Tags.FirstOrDefault(t => t.TagNumber == tagNumber);

            if (tag != null)
                throw new ApplicationException("A tag with that number already exists.");

            var tageEntity = new Tag
            {
                TagNumber = tagNumber,
                Name = name,
                LastLocation = lastLocation,
                InLocation = inLocation
            };

            context.Tags.Add(tageEntity);
            context.SaveChanges();
        }
    }
}
