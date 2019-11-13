using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RFIDCommandCenter
{
    class NetworkCode
    {
        internal const int MAX_PAYLOAD_SIZE = 112;
        internal const int HEADER_SIZE = 16;

        public int command;
        public int payloadSize;
        public ulong tickTime;
        public byte[] payload = new byte[MAX_PAYLOAD_SIZE];
    }

    enum ClientType
    {
        CLIENT_UI_APP = 1,
        CLIENT_RFID_DEVICE = 2
    };

    enum DeviceNetworkCommands
    {
        UNLOCK = 1,
        LOCK = 2,
        START = 3,
        UPDATE = 4,
        STOP = 5,
        TAG_ARRIVE = 6,
        TAG_LEAVE = 7,
        OBJECT_PRESENT = 8,
        TAG_PRESENT_TOO_LONG = 9,
        GET_DEVICE_TICK_COUNT = 10,
        RESET_DEVICE_TICK_COUNT = 11,
        WRITE_TAG = 12
    }

    enum UINetworkCommands
    {
        SAVE_TAG =1,
        DELETE_TAG = 2,
        SAVE_SYSTEM_USER = 3,
        DELETE_SYSTEM_USER = 4,
        GET_LOCATION_LIST=5,
        SAVE_LOCATION =6,
        DELETE_LOCATION =7
    }

    class Client
    {
        byte[] networkCache;
        protected Socket clientSocket;
        protected NetworkCommunication netCommObject;
        bool receivePacket(NetworkCode data,int waitTimeUs)
        {
            if (!clientSocket.Poll(waitTimeUs, SelectMode.SelectRead))
                return false;
            int ttlBytesRecv = netCommObject.readFrom(clientSocket,networkCache);
            if (ttlBytesRecv < NetworkCode.HEADER_SIZE)
                throw new RFIDCommandCenterException("The specified network packet was too small", null);
            data.command = BitConverter.ToInt32(networkCache, 0);
            data.payloadSize = BitConverter.ToInt32(networkCache, 4);
            data.tickTime = BitConverter.ToUInt64(networkCache, 8);
            //Verify payload size and remaining bytes are equivalent
            if (ttlBytesRecv - NetworkCode.HEADER_SIZE != data.payloadSize)
                throw new RFIDCommandCenterException("The specified network packet size is incorrect", null);
            if(data.payloadSize > 0)
                Array.Copy(networkCache, 16, data.payload, 0, data.payloadSize);
            return true;
        }
        bool sendPacket(NetworkCode data, int timeoutUs)
        {
            if (!clientSocket.Poll(timeoutUs, SelectMode.SelectWrite))
                return false;
            BitConverter.GetBytes(data.command).CopyTo(networkCache, 0);
            BitConverter.GetBytes(data.payloadSize).CopyTo(networkCache, 4);
            BitConverter.GetBytes(data.tickTime).CopyTo(networkCache, 8);
            if (data.payloadSize != 0)
                Array.Copy(data.payload, 0, networkCache, 16, data.payloadSize);
            int totalPacketSize = NetworkCode.HEADER_SIZE + data.payloadSize;
            if (netCommObject.writeTo(clientSocket,networkCache, totalPacketSize) != totalPacketSize)
                return false;
            return true;
        }

        protected Client(Socket who,NetworkCommunication netObject)
        {
            clientSocket = who;
            netCommObject = netObject;
        }
    }

    class RFIDDeviceClient : Client
    {

        public RFIDDeviceClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
        {

        }
    }

    class RFIDUIClient : Client
    {
        public RFIDUIClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
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
        internal override int readFrom(Socket who, byte[] buffer, int size = 0)
        {
            return base.readFrom(who, buffer, size);
        }

        internal override int writeTo(Socket who, byte[] buffer, int size)
        {
            return base.writeTo(who, buffer, size);
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
                    returnval = new RFIDDeviceClient(acceptedSocket,this);
                    break;
                case (int)ClientType.CLIENT_UI_APP:
                    returnval = new RFIDUIClient(acceptedSocket,this);
                    break;
                default:
                    throw new RFIDCommandCenterException("The client initialization packet is invalid", null);
            }
            return returnval;
        }

        internal virtual int readFrom(Socket who, byte[] buffer,int size = 0)
        {
            if (size == 0)
                return who.Receive(buffer);
            else
                return who.Receive(buffer,0, size, SocketFlags.None);
        }

        internal virtual int writeTo(Socket who, byte[] buffer,int size)
        {
           return who.Send(buffer, 0, size, SocketFlags.None);
        }
    }
}
