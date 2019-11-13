using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter.Logic
{
    public class DeleteSystemUser
    {
        public void Execute(string username, DataContext context)
        {
            var userToDelete = context.SystemUsers.SingleOrDefault(s => string.Equals(username, s.Username, StringComparison.InvariantCultureIgnoreCase));

            if (userToDelete == null)
                throw new ApplicationException("There is no system user with that username");

            userToDelete.Active = false;

            context.SaveChanges();
        }
    }
}
