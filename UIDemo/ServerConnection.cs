using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace UIDemo
{
    class ServerConnection : IDisposable
    {
        public event EventHandler Connected;
        public event EventHandler<Exception> FailedConnecting;
        public event EventHandler<Exception> NetworkError;
        TcpClient myServerConnection;

        [Serializable]
        enum NetworkCommands
        {
            CONNECT= 1,
            SAVE_TAG,
            DELETE_TAG,
            SAVE_SYSTEM_USER,
            DELETE_SYSTEM_USER,
            GET_LOCATION_LIST,
            SAVE_LOCATION,
            DELETE_LOCATION,
            PING_DEVICE,
            SETUP_DEVICE
        }
        
        [Serializable]
        protected class StartupPacket
        { 
            int cmd = 1;
            int size = 0;
            long padding; 
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
                        myServerConnection.Client.Send(BitConverter.GetBytes((int)1));
                    }
                    catch(Exception e)
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
                        formatter.Serialize(myServerConnection.GetStream(), NetworkCommands.SAVE_TAG);
                        //Give server save tag arguments
                        formatter.Serialize(myServerConnection.GetStream(), tagNumber);
                        formatter.Serialize(myServerConnection.GetStream(), name);
                    }
                    catch(Exception e)
                    {
                        NetworkError?.Invoke(this, e);
                        return false;
                    }
                    return true;
                });
            return Task.Run(saveTagRPC);
        }
        
        public void Dispose()
        {
            if (myServerConnection != null)
                myServerConnection.Dispose();
        }
    }
}
