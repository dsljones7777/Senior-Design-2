using System.Net.Sockets;

namespace RFIDCommandCenter
{
    class RFIDUIClient : Client
    {
        public RFIDUIClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
        {

        }
    }
}
