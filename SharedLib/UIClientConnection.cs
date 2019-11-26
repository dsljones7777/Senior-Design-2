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
            public class FunctionException : Exception
            {
                public FunctionException(string msg) : base(msg)
                {

                }
            }
            public event EventHandler Connected;
            public event EventHandler<Exception> FailedConnecting;
            public static event EventHandler<Exception> NetworkError;
            public event EventHandler<ServerMessage> ServerMessageReceived;
            [Serializable]
            public abstract class UINetworkPacket
            {
                public UINetworkPacket execute()
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FunctionCallStatusRPC status = null;
                    UINetworkPacket returnval = null;
                    try
                    {
                        lock (UIClientConnection.streamLock)
                        {
                            formatter.Serialize(serializerStream, this);
                            UINetworkPacket response = (UINetworkPacket)formatter.Deserialize(serializerStream);
                            status = response as FunctionCallStatusRPC;
                            if (status != null && status.error != null)
                                throw new FunctionException(status.error);
                            else if (status == null)
                            {
                                returnval = (UINetworkPacket)response;
                                status = (FunctionCallStatusRPC)formatter.Deserialize(serializerStream);
                                if (status.error != null)
                                    throw new FunctionException(status.error);
                            }
                            return returnval;
                        }
                    }
                    catch (FunctionException e)
                    {
                        throw e;
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
                        
                        object rpc;
                        lock(streamLock)
                        {
                            if (myServerConnection.Available <= 0)
                            {
                                Thread.Sleep(200);
                                continue;
                            }
                            rpc = formatter.Deserialize(serializerStream);
                        }
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
