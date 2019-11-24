using Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Network.NetworkLib;

namespace SharedLib
{
    namespace Network
    {
        

        public class UIClientConnection
        {
            public event EventHandler Connected;
            public event EventHandler<Exception> FailedConnecting;
            public static event EventHandler<Exception> NetworkError;
            public event EventHandler<ServerMessage> ServerMessageReceived;
            [Serializable]
            public abstract class UINetworkPacket
            {
                protected bool bothWay = false;
                public UINetworkPacket execute()
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    UINetworkPacket returnval = null;
                    try
                    {
                        lock (UIClientConnection.streamLock)
                        {
                            formatter.Serialize(serializerStream, this);
                            if (bothWay)
                                returnval = (UINetworkPacket)formatter.Deserialize(serializerStream);
                            return returnval;
                        }
                    }
                    catch (Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        throw;
                    }
                }

                public Task<UINetworkPacket> executeAsync()
                {
                    return Task.Run((Func<UINetworkPacket>)this.execute);
                }
            }

            public Task<bool> connect(string ipOrHostName, ushort port)
            {
                Func<bool> connectRPC = new Func<bool>
                    (() =>
                    {
                        if (myServerConnection != null)
                            myServerConnection.Dispose();
                        if(serverCallbackThread != null)
                        {
                            exitCallbackThread = true;
                            if(!serverCallbackThread.IsCompleted)
                                serverCallbackThread.Wait();
                            serverCallbackThread.Dispose();
                            serverCallbackThread = null;
                        }
                        try
                        {
                            lock(streamLock)
                            {
                                myServerConnection = new TcpClient(ipOrHostName, port);
                                serializerStream = myServerConnection.GetStream();
                                myServerConnection.Client.Send(BitConverter.GetBytes((int)1));
                                serverCallbackThread = new Task(serverCallbackThreadRoutine);
                                exitCallbackThread = false;
                                serverCallbackThread.Start();
                            }
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
            

            public void Dispose()
            {
                if (myServerConnection != null)
                {
                    exitCallbackThread = true;
                    myServerConnection.Dispose();
                    myServerConnection = null;
                    if (serverCallbackThread != null)
                    {
                        if (!serverCallbackThread.IsCompleted)
                            serverCallbackThread.Wait();
                        serverCallbackThread.Dispose();
                        serverCallbackThread = null;
                    }
                }
            }

            Task serverCallbackThread;
            TcpClient myServerConnection;
            static volatile bool exitCallbackThread = false;
            static object streamLock = new object();
            static Stream serializerStream;

            void serverCallbackThreadRoutine()
            {
                BinaryFormatter formatter = new BinaryFormatter();
                while (!exitCallbackThread)
                {
                    try
                    {
                        if (myServerConnection.Available <= 0)
                        {
                            Thread.Sleep(200);
                            continue;
                        }
                        object rpc = formatter.Deserialize(serializerStream);
                        if (rpc.GetType() == typeof(ErrorReplyRPC))
                            handleErrorPromptFromServer((ErrorReplyRPC)rpc);
                        else
                            throw new Exception("Could not determine the RPC from the server");
                    }
                    catch(Exception e)
                    {

                    }
                }
            }

            void handleErrorPromptFromServer(ErrorReplyRPC serverMsg)
            {
                ServerMessageReceived?.Invoke(this,
                    new ServerMessage {deviceSerial = serverMsg.serialNumber, message = serverMsg.msg});
            }
        }
    }
    
}
