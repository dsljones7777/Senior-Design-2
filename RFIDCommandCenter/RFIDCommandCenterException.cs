using System;

namespace RFIDCommandCenter
{
    class RFIDCommandCenterException : Exception
    {
        public readonly Exception realException;
        public RFIDCommandCenterException(string message,Exception inner) : base(message)
        {
            realException = inner;
        }
    }
}
