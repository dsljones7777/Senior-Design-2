using SharedLib;
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
            var userToDelete = context.SystemUsers.SingleOrDefault(s => username.Equals(s.Username, StringComparison.InvariantCultureIgnoreCase));

            if (userToDelete == null)
                throw new UIClientException("There is no system user with that username");

            context.SystemUsers.Remove(userToDelete);

            context.SaveChanges();
        }
    }
}
