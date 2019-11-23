using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Network.NetworkLib;

namespace RFIDCommandCenter.Logic
{
    public class ViewUsers
    {
        public List<SharedModels.SharedUsers> Execute(DataContext context)
        {
            return context.SystemUsers.Select(s => new SharedModels.SharedUsers { Username = s.Username, UserRole = (Role)s.UserRole }).ToList();
        }
    }
}
