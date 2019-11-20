using Network;
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
        public volatile bool clientAnswered = false;
        public volatile bool continueExecution = false;
        public List<object> messages = new List<object>();
        public event EventHandler<Exception> IncomingRPCError;
        public event EventHandler<Exception> OutgoingRPCError;


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
                    NetworkLib.NetworkCommands actualCmd = (NetworkLib.NetworkCommands)cmdVal;
                    executeRPC(actualCmd);
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

        public void executeRPC(NetworkLib.NetworkCommands command)
        {
            using (var context = new DataContext())
            {
                var formatter = new BinaryFormatter();
                switch (command)
                {
                    case NetworkLib.NetworkCommands.SAVE_TAG:
                        var tagNumberSave = (byte[])formatter.Deserialize(clientStream);
                        if (tagNumberSave == null || tagNumberSave.Length != 12)
                            throw new ApplicationException("Invalid RPC");
                        var tagName = (string)formatter.Deserialize(clientStream);
                        var saveTag = new Logic.SaveTag();
                        saveTag.Execute(tagNumberSave, tagName, context);
                        break;
                    case NetworkLib.NetworkCommands.DELETE_TAG:
                        var tagNumberDel = (byte[])formatter.Deserialize(clientStream);
                        if (tagNumberDel == null || tagNumberDel.Length != 12)
                            throw new ApplicationException("Invalid RPC");
                        var delTag = new Logic.DeleteTag();
                        delTag.Execute(tagNumberDel, context);
                        break;
                    case NetworkLib.NetworkCommands.SAVE_SYSTEM_USER:
                        var userName = (string)formatter.Deserialize(clientStream);
                        var pass = (string)formatter.Deserialize(clientStream);
                        var userRole = (int)formatter.Deserialize(clientStream);
                        if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pass) || userRole == 0)
                            throw new ApplicationException("Invalid RPC");
                        var saveSysUser = new Logic.SaveSystemUser();
                        //need to encrypt pass
                        //saveSysUser.Execute(userName, pass, userRole, context);
                        break;
                    case NetworkLib.NetworkCommands.DELETE_SYSTEM_USER:
                        var sysUsername = (string)formatter.Deserialize(clientStream);
                        if (string.IsNullOrEmpty(sysUsername))
                            throw new ApplicationException("Invalid RPC");
                        var delSysUser = new Logic.DeleteSystemUser();
                        delSysUser.Execute(sysUsername, context);
                        break;
                    case NetworkLib.NetworkCommands.GET_LOCATION_LIST:
                        //
                        break;
                    case NetworkLib.NetworkCommands.SAVE_LOCATION:
                        //
                        break;
                    case NetworkLib.NetworkCommands.DELETE_LOCATION:
                        //
                        break;

                }
            }
        }
        
        public bool tellClient(string msg)
        {
            return false;
        }
        
        public void addMessage(string message)
        {

        }

        private void removeMessage(string message)
        {

        }
    }
}
