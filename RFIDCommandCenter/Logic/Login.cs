using SharedLib;
using System;
using System.Linq;

namespace RFIDCommandCenter.Logic
{
    public class Login
    {
        public void Execute(string username, byte[] pass,out int userRole, DataContext context)
        {            
            var existingSysUser = context.SystemUsers.SingleOrDefault(s => s.Username == username);
            if(existingSysUser == null)
                throw new UIClientException("The username/password entered was incrorrect");
            userRole = existingSysUser.UserRole;
           
            if (!existingSysUser.Pass.SequenceEqual(pass))
                throw new UIClientException("The username/password entered was incrorrect");
        }
    }
}
