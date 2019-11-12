using RFIDCommandCenter.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDCommandCenter
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var context = new DataContext())
            {
                var component = new Logic.GetSystemUserByUsername();
                var test = component.Execute("cosimo", context);
            }

        }
    }
}
