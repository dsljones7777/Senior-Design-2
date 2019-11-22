using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network;
using static Network.NetworkLib;

namespace UIDemo
{
    class ServerConnection : IDisposable
    {
        public class ServerMessage
        {
            public string deviceSerial;     //Null serials indicate it is a server related issue not particular to any device
            public string message;
            public bool retry = false;
        }
        public event EventHandler Connected;
        public event EventHandler<Exception> FailedConnecting;
        public event EventHandler<Exception> NetworkError;
        public event EventHandler<ServerMessage> ServerMessageReceived;
        static volatile bool exitCallbackThread;
        
        Task serverCallbackThread;
        TcpClient myServerConnection;

        void serverCallbackThreadRoutine()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                while (!exitCallbackThread)
                {
                    try
                    {
                       
                        if (myServerConnection.Available <= 0)
                        {
                            Thread.Sleep(200);
                            continue;
                        }
                        switch ((NetworkCommands)formatter.Deserialize(myServerConnection.GetStream()))
                        {
                            case NetworkLib.NetworkCommands.ERROR_PROMPT:
                                handleErrorPromptFromServer();
                                break;
                        }

                    }
                    catch
                    {

                    }

                }

            }
            catch
            {

            }
        }

        void handleErrorPromptFromServer()
        {
            Stream netStream = myServerConnection.GetStream();
            BinaryFormatter formatter = new BinaryFormatter();
            object arg1 = formatter.Deserialize(netStream);
            string msg = (string)formatter.Deserialize(netStream);
            string devSerial = null;
            if (arg1.GetType() != typeof(NetworkLib.NullSerializer))
                devSerial = (string)arg1;
            ServerMessageReceived?.Invoke(this, new ServerMessage{deviceSerial = devSerial , message = msg});
        }

        public Task<bool> sendMessageReply(ServerMessage msg)
        {
            Func<bool> sendMessageReplyFunc = new Func<bool>(
               () =>
               {
                   BinaryFormatter formatter = new BinaryFormatter();
                   Stream serverStream = myServerConnection.GetStream();
                   try
                   {
                       //Tell server message
                       formatter.Serialize(serverStream, NetworkLib.NetworkCommands.ERROR_PROMPT);
                       if (msg.deviceSerial == null)
                           formatter.Serialize(serverStream, new NetworkLib.NullSerializer());
                       else
                           formatter.Serialize(serverStream, msg.deviceSerial);
                       formatter.Serialize(serverStream, msg.retry);
                   }
                   catch (Exception e)
                   {
                       NetworkError?.Invoke(this, e);
                       return false;
                   }
                   return true;
               });
            return Task.Run(sendMessageReplyFunc);
        }

        public Task<bool> connect(string ipOrHostName, ushort port)
        {
            Func<bool> connectRPC = new Func<bool>
                (() =>
                {
                    if (myServerConnection != null)
                        myServerConnection.Dispose();
                    try
                    {
                        myServerConnection = new TcpClient(ipOrHostName, port);
                        serverCallbackThread = new Task(serverCallbackThreadRoutine);
                        serverCallbackThread.Start();
                        myServerConnection.Client.Send(BitConverter.GetBytes((int)1));
                    }
                    catch (Exception e)
                    {
                        FailedConnecting?.Invoke(this, e);
                        return false;
                    }
                    Connected?.Invoke(this, null);
                    return true;
                });
            return Task.Run(connectRPC);
        }

        public Task<bool> saveTag(byte[] tagNumber, string name)
        {
            Func<bool> saveTagRPC = new Func<bool>(
                () =>
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        //Tell server it is save tag command
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.SAVE_TAG);
                        //Give server save tag arguments
                        formatter.Serialize(myServerConnection.GetStream(), tagNumber);
                        formatter.Serialize(myServerConnection.GetStream(), name);
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });
            return Task.Run(saveTagRPC);
        }

        public Task<bool> deleteTag(byte[] tagNumber)
        {
            Func<bool> deteleTagRPC = new Func<bool>(
                () =>
                {
                    var formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.DELETE_TAG);
                        formatter.Serialize(myServerConnection.GetStream(), tagNumber);
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });

            return Task.Run(deteleTagRPC);
        }

        public Task<bool> writeTag(byte[] targetTag, byte[] newTagBytes)
        {
            Func<bool> writeTagRPC = new Func<bool>(
                () =>
                {
                    var formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.WRITE_TAG);
                        formatter.Serialize(myServerConnection.GetStream(), targetTag);
                        formatter.Serialize(myServerConnection.GetStream(), newTagBytes);
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });

            return Task.Run(writeTagRPC);
        }

        public Task<bool> saveSystemUser(string username, string pass, int role)
        {
            Func<bool> saveSystemUser = new Func<bool>(
                () =>
                {
                    var formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.SAVE_SYSTEM_USER);
                        formatter.Serialize(myServerConnection.GetStream(), username);
                        formatter.Serialize(myServerConnection.GetStream(), pass);
                        formatter.Serialize(myServerConnection.GetStream(), role);
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });

            return Task.Run(saveSystemUser);
        }

        public Task<bool> deleteSystemUser(string username)
        {
            Func<bool> deleteSystemUser = new Func<bool>(
                () =>
                {
                    var formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.DELETE_SYSTEM_USER);
                        formatter.Serialize(myServerConnection.GetStream(), username);
                    }
                    catch(Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });
            return Task.Run(deleteSystemUser);
        }
       
        public Task<bool> saveLocation(string locationName, string readerSerialIn, string readerSerialOut)
        {
            Func<bool> saveLocation = new Func<bool>(
                () =>
                {
                    var formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.SAVE_LOCATION);
                        formatter.Serialize(myServerConnection.GetStream(), locationName);
                        formatter.Serialize(myServerConnection.GetStream(), readerSerialIn);

                        if (readerSerialOut == null)
                            formatter.Serialize(myServerConnection.GetStream(), new NetworkLib.NullSerializer());
                        else
                            formatter.Serialize(myServerConnection.GetStream(), readerSerialOut);
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });
            return Task.Run(saveLocation);
        }

        public Task<bool> deletLocation(string locationName)
        {
            Func<bool> deleteLocation = new Func<bool>(
                () =>
                {
                    var formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(myServerConnection.GetStream(), NetworkLib.NetworkCommands.SAVE_LOCATION);
                        formatter.Serialize(myServerConnection.GetStream(), locationName);
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });
            return Task.Run(deleteLocation);
        }

        public void Dispose()
        {
            if (myServerConnection != null)
                myServerConnection.Dispose();
        }
    }
}
