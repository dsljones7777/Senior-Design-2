using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class GetSystemUserByUsername
    {
        public SystemUser Execute(string userName, DataContext context)
        {
            return context.SystemUsers.SingleOrDefault(x => x.Username == userName);
        }
    }
}
