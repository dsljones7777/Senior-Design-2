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

            var readerSerialIns = dataContext.Locations.Select(x => x.ReaderSerialIn).Distinct().ToList();
            var readerSerialOuts = dataContext.Locations.Select(x => x.ReaderSerialOut).Distinct().ToList();

            var serialNumberList = new List<string>();
            readerSerialIns.ForEach(x => serialNumberList.Add(x));
            readerSerialOuts.ForEach(x => serialNumberList.Add(x));

            return serialNumberList;
        }
    }
}
