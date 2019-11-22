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

        //First obj in tuple is the device serial number, the second object is a string for now but it is what to send to the client
        List<Tuple<string, object>> messagesToSend = new List<Tuple<string, object>>();

        //First obj in tuple is the device serial number, the second object is a bool for now but it is what client sends back to us
        List<Tuple<string, object>> messagesRcvd = new List<Tuple<string, object>>();
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
                    try
                    {
                        executeRPC(actualCmd);
                    }
                    catch(Exception e)
                    {
                        addMessage(null, e.Message);
                    }
                }

                //Check to see messages needed to be sent to the client
                lock(messagesToSend)
                {
                    
                        try
                        {
                            foreach (var serialMsgTup in messagesToSend)
                                tellClient(serialMsgTup.Item1, (string)serialMsgTup.Item2);
                            messagesToSend.Clear();
                        }
                        catch
                        {
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
                    case NetworkLib.NetworkCommands.ERROR_PROMPT:
                        string serial = (string)formatter.Deserialize(clientStream);
                        bool userResponse = (bool)formatter.Deserialize(clientStream);
                        msgRecevied(serial, userResponse);
                        break;
                }
            }
        }
        
       

        public void tellClient(string serial, string msg)
        {
            BinaryFormatter formatSerializer = new BinaryFormatter();
            formatSerializer.Serialize(clientStream, NetworkLib.NetworkCommands.ERROR_PROMPT);
            if (serial == null)
                formatSerializer.Serialize(clientStream, new NetworkLib.NullSerializer());
            else 
                formatSerializer.Serialize(clientStream, serial);
            formatSerializer.Serialize(clientStream, msg);
        }
        
        public void addMessage(string serial, string message)
        {
            lock (messagesToSend)
            {
                messagesToSend.Add(new Tuple<string, object>(serial, message));
            }  
        }

        public void msgRecevied(string serial, bool didUsrSayYes)
        {
            lock(messagesRcvd)
            {
                messagesRcvd.Add(new Tuple<string, object>(serial, didUsrSayYes));
            }
        }
        
    }
}
