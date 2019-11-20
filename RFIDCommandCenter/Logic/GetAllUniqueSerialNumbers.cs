using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class GetAllUniqueSerialNumbers
    {
        public List<string> Execute(DataContext dataContext)
        {             
            //var serialNumbers = dataContext.Locations.SqlQuery<string>
            //    (@"DECLARE Temp Table(SerialNum NVARCHAR(50)) 
            //       SET TEMP = SELECT DISTINCT ReaderSerialIn FROM Locations UNION SELECT DISTINCT ReaderSerialOut FROM Locations
            //       SELECT DISTINCT SerialNum FROM Temp").ToList();

            return new List<string>();
        }
    }
}
