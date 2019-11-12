using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RFIDCommandCenter
{

    enum ClientType
    {
        CLIENT_UI_APP = 1,
        CLIENT_RFID_DEVICE = 2
    };

    class Client
    {
        protected Socket clientSocket;

        protected Client(Socket who)
        {

        }
    }

    class RFIDDeviceClient : Client
    {
        public RFIDDeviceClient(Socket who) : base (who)
        {

        }
    }

    class RFIDUIClient : Client
    {
        public RFIDUIClient(Socket who) : base (who)
        {

        }
    }
    class RFIDCommandCenterException : Exception
    {
        public readonly Exception realException;
        public RFIDCommandCenterException(string message,Exception inner) : base(message)
        {
            realException = inner;
        }
    }
    class SecureNetworkCommunication : NetworkCommunication
    {
        protected override int readFrom(Socket who, byte[] buffer, int size, int offset = 0)
        {

            return base.readFrom(who, buffer, size, offset);
        }

        protected override int writeTo(Socket who, byte[] buffer, int size, int offset = 0)
        {
            return base.writeTo(who, buffer, size, offset);
        }
    }
    class NetworkCommunication
    {
        const int BACKLOG_CONNECTION_AMOUNT = 5;
        bool hasStarted = false;
        Socket serverSocket;

        static NetworkCommunication createNetworkCommunicationObject()
        {
            return new SecureNetworkCommunication();
        }

        protected NetworkCommunication()
        {
            
        }

        public void start()
        {
            if (hasStarted)
                return;
            IPHostEntry hostIPAddressEntries = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress hostIPAddress = hostIPAddressEntries.AddressList[0];
            IPEndPoint hostEndpoint = new IPEndPoint(hostIPAddress, 52437);
            serverSocket = new Socket(hostIPAddress.AddressFamily, SocketType.Stream, ProtocolType.IPv4);
            serverSocket.Bind(hostEndpoint);
            serverSocket.Listen(BACKLOG_CONNECTION_AMOUNT);
            hasStarted = true;
        }

        public Client acceptPendingClient()
        {
            Socket acceptedSocket;
            byte[] buffer = new byte[4];
            try
            {
                acceptedSocket = serverSocket.Accept();
            }
            catch(Exception e)
            {
                throw new RFIDCommandCenterException("Failed to accept client connection", e);
            }
            try
            {
                int bytesRead = readFrom(acceptedSocket, buffer, 4);
                if (bytesRead != 4)
                    throw new RFIDCommandCenterException("Failed to read client initialization bytes (less than 4 bytes were read)", null);
            }
            catch(Exception e)
            {
                try
                {
                    acceptedSocket.Disconnect(false);
                    acceptedSocket.Close(5);
                }
                catch
                {

                }
                throw new RFIDCommandCenterException("Failed to read client initialization bytes", e);
            }
            int type = BitConverter.ToInt32(buffer, 0);
            Client returnval;
            switch(type)
            {
                case (int)ClientType.CLIENT_RFID_DEVICE:
                    returnval = new RFIDDeviceClient(acceptedSocket);
                    break;
                case (int)ClientType.CLIENT_UI_APP:
                    returnval = new RFIDUIClient(acceptedSocket);
                    break;
                default:
                    throw new RFIDCommandCenterException("The client initialization packet is invalid", null);
            }
            return returnval;
        }

        protected virtual int readFrom(Socket who, byte[] buffer,int size, int offset = 0)
        {
            return who.Receive(buffer, offset, size, SocketFlags.None);
        }

        protected virtual int writeTo(Socket who, byte[] buffer,int size, int offset =0)
        {
            return who.Send(buffer, offset, size, SocketFlags.None);
        }
    }
}
