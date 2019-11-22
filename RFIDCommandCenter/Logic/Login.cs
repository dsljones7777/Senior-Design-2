using System;
using System.Linq;

namespace RFIDCommandCenter.Logic
{
    public class Login
    {
        public void Execute(string username, byte[] pass, int userRole, DataContext context)
        {            
            var existingSysUser = context.SystemUsers.SingleOrDefault(s => s.Username == username);

            if (existingSysUser.Pass != pass)
                throw new ApplicationException("The username/password entered was incrorrect");
        }
    }
}
