using System;

namespace RFIDCommandCenter
{
    class CommandCenterException : Exception
    {
        public readonly Exception realException;
        public CommandCenterException(string message,Exception inner) : base(message)
        {
            realException = inner;
        }
    }
}
