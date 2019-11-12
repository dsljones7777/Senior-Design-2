using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class GetTagByTagNumber
    {
        public Tag Execute(byte[] tagNumber, DataContext context)
        {
            return context.Tags.FirstOrDefault(t => t.TagNumber == tagNumber);
        }
    }
}
