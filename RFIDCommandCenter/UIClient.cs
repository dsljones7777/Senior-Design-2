using Network;
using SharedLib.Network;
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
        public volatile bool continueExecution = false;
        public Exception lastException;
        public string clientUsername = null;
        public int role;

        //First obj in tuple is the device serial number, the second object is a string for now but it is what to send to the client
        List<Tuple<string, object>> messagesToSend = new List<Tuple<string, object>>();

        //First obj in tuple is the device serial number, the second object is a bool for now but it is what client sends back to us
        List<Tuple<string, object>> messagesRcvd = new List<Tuple<string, object>>();

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
                    try
                    {
                        executeRPC(cmd);
                    }
                    catch(Exception e)
                    {
                        lastException = e;
                        return;
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
                    catch(Exception e)
                    {
                        lastException = e;
                        return;
                    }
                }
            }
        }

        void executeRPC(object cmd)
        {
            using (var context = new DataContext())
            {
                if (cmd.GetType() == typeof(LoginUserRPC))
                {
                    //TODO: Execute login 
                    return;
                }
                else if (clientUsername == null)
                    throw new Exception("You must be logged in to do that");
                if(cmd.GetType() == typeof(SaveTagRPC))
                {
                    SaveTagRPC op = (SaveTagRPC)cmd;
                    var saveTag = new Logic.SaveTag();
                    saveTag.Execute(op.tagNumber,op.name, context);
                }
                else if(cmd.GetType() == typeof(DeleteTagRPC))
                {
                    DeleteTagRPC op = (DeleteTagRPC)cmd;
                    var deleteTag = new Logic.DeleteTag();
                    deleteTag.Execute(op.tagNumber,context);
                }
                else if (cmd.GetType() == typeof(SaveSystemUserRPC))
                {
                    SaveSystemUserRPC op = (SaveSystemUserRPC)cmd;
                    var saveSysUser = new Logic.SaveSystemUser();
                    //TODO: Implement hashing
                    byte[] passBytes = null;
                    saveSysUser.Execute(op.username, passBytes,op.role, context);
                }
                else if (cmd.GetType() == typeof(DeleteSystemUserRPC))
                {
                    DeleteSystemUserRPC op = (DeleteSystemUserRPC)cmd;
                    var delSysUser = new Logic.DeleteSystemUser();
                    delSysUser.Execute(op.username, context);
                }
                else if(cmd.GetType() == typeof(SaveLocationRPC))
                {
                    SaveLocationRPC op = (SaveLocationRPC)cmd;
                    var saveLoc = new Logic.SaveLocation();
                    saveLoc.Execute(op.locationName, op.readerSerialIn,op.readerSerialOut,context);
                }
                else if(cmd.GetType() == typeof(DeleteLocationRPC))
                {
                    DeleteLocationRPC op = (DeleteLocationRPC)cmd;
                    var delLoc = new Logic.DeleteLocation();
                    //TODO: Delete location should not include reader serial in
                    delLoc.Execute(op.locationName, context);
                }
                else if(cmd.GetType() == typeof(ErrorReplyRPC))
                {
                    ErrorReplyRPC op = (ErrorReplyRPC)cmd;
                    if (op.serialNumber != null)
                        msgRecevied(op.serialNumber, op.retry);
                }
                else
                {
                    throw new Exception("Client sent an invalid RPC");
                }
            }
        }

        void tellClient(string serialNum, string clientMsg)
        {
            ErrorReplyRPC errorRpc = new ErrorReplyRPC()
            {
                msg = clientMsg,
                serialNumber = serialNum
            };
            BinaryFormatter formatSerializer = new BinaryFormatter();
            formatSerializer.Serialize(clientStream, errorRpc);
        }
        
        public void addErrorMessage(string serial, string message)
        {
            lock (messagesToSend)
            {
                messagesToSend.Add(new Tuple<string, object>(serial, message));
            }  
        }

        void msgRecevied(string serial, bool didUsrSayYes)
        {
            lock(messagesRcvd)
            {
                messagesRcvd.Add(new Tuple<string, object>(serial, didUsrSayYes));
            }
        }
        
    }
}
