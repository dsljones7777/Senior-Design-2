using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class SaveSystemUser
    {
        public void Execute(string userName, byte[] pass, int userRole, DataContext context)
        {
            var systemUser = context.SystemUsers.FirstOrDefault(s => s.Username == userName);

            if (systemUser != null)
                throw new UIClientException("A system user with that Username already exists.");

            context.SystemUsers.Add(new SystemUser { Username = userName, Pass = pass, UserRole = userRole });
            context.SaveChanges();
        }
    }
}
