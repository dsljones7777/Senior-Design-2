using System.Net.Sockets;

namespace RFIDCommandCenter
{
    class SecureNetworkCommunication : NetworkCommunication
    {
        internal override int readFrom(Socket who, byte[] buffer, int size = 0)
        {
            return base.readFrom(who, buffer, size);
        }

        internal override int writeTo(Socket who, byte[] buffer, int size)
        {
            return base.writeTo(who, buffer, size);
        }
    }
}
