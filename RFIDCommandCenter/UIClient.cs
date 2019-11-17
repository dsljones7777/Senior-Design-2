using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace RFIDCommandCenter
{
    class UIClient : Client
    {
        public volatile bool exit = false;
        public List<object> messages = new List<object>();


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
        };
        NetworkStream clientStream;
        public UIClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
        {
            clientStream = new NetworkStream(who);
            
        }

        

        public override void serverThreadRoutine(object state)
        {
            while (!exit)
            {
                //I have a command available
                if(clientStream.DataAvailable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    object cmd = formatter.Deserialize(clientStream);
                    int cmdVal = (int)cmd;
                    NetworkCommands actualCmd = (NetworkCommands)cmdVal;
                    executeRPC();
                }
                //Check messages to send to UI
                lock(messages)
                {
                    foreach(object val in messages)
                    {
                        if(val.GetType() == typeof(string))
                        {
                            if (tellClient((string)val))
                            {
                                //tell main thread somehow to continue;
                            }
                            else
                            {
                                //tell main thread to not continue with error;
                            }
                        }
                    }
                }
            }
        }

        public void executeRPC()
        {
            
        }
        
        public bool tellClient(string msg)
        {
            return false;
        }
    }
}
