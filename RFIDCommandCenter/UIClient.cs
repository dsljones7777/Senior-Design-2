using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace RFIDCommandCenter
{
    class UIClient : Client
    {
        enum NetworkCommands
        {
            CONNECT = 1,
            SAVE_TAG,
            WRITE_TAG,
            DELETE_TAG,
            SAVE_SYSTEM_USER,
            DELETE_SYSTEM_USER,
            GET_LOCATION_LIST,
            SAVE_LOCATION,
            DELETE_LOCATION,
            PING_DEVICE,
            SETUP_DEVICE

        NetworkStream clientStream;
        public UIClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
        {
            clientStream = new NetworkStream(who);
            
        }

        

        public override void serverThreadRoutine(object state)
        {
            
        }

        public void executeRPC()
        {
            
        }
        
    }
}
