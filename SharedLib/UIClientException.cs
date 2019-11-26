using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    [Serializable]
    public class UIClientException : Exception
    {
        public UIClientException(string message) : base(message)
        {

        }
    }
}
