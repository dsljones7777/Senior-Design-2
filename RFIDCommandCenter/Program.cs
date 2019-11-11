using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new DataContext())
            {
                var location = context.Locations.Where(x => x.ID == 1).ToList();
                var test = location;
            }

        }
    }
}
