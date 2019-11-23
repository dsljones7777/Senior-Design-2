using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDCommandCenter.Logic
{
    public class EditUser
    {
        public void Execute(string username, byte[] pass,  Network.NetworkLib.Role? userRole, DataContext context)
        {
            var existingUser = context.SystemUsers.SingleOrDefault(x => x.Username == username);

            if (existingUser == null)
                throw new Exception("There is no user with that username in the database");

            if (pass != null)
                existingUser.Pass = pass;

            if (userRole != null)
                existingUser.UserRole = (int)userRole;

            context.SaveChanges();
        }
    }
}
