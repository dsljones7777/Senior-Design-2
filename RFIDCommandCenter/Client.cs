using System;
using System.Net.Sockets;

namespace RFIDCommandCenter
{
    abstract class Client
    {
        byte[] networkCache = new byte[NetworkCode.HEADER_SIZE];
        protected Socket clientSocket;
        protected NetworkCommunication netCommObject;
        public bool receivePacket(NetworkCode data,int waitTimeUs)
        {
            if (!clientSocket.Poll(waitTimeUs, SelectMode.SelectRead))
                return false;
            int ttlBytesRecv = netCommObject.readFrom(clientSocket,networkCache,NetworkCode.HEADER_SIZE);
            if (ttlBytesRecv != NetworkCode.HEADER_SIZE)
                throw new RFIDCommandCenterException("The specified network packet was too small", null);
            data.command = BitConverter.ToInt32(networkCache, 0);
            data.payloadSize = BitConverter.ToInt32(networkCache, 4);
            data.tickTime = BitConverter.ToUInt64(networkCache, 8);
            if(data.payloadSize > NetworkCode.MAX_PAYLOAD_SIZE)
                throw new RFIDCommandCenterException("The specified network packet size is incorrect", null);
            if (data.payloadSize > 0)
            {
                if(netCommObject.readFrom(clientSocket, data.payload, data.payloadSize) != data.payloadSize)
                    throw new RFIDCommandCenterException("The specified network packet size is incorrect", null);
            }
            return true;
        }
        public bool sendPacket(NetworkCode data, int timeoutUs)
        {
            if (!clientSocket.Poll(timeoutUs, SelectMode.SelectWrite))
                return false;
            if(netCommObject.writeTo(clientSocket, BitConverter.GetBytes(data.command),4) != 4)
                throw new RFIDCommandCenterException("Falied to write command bytes", null);
            if (netCommObject.writeTo(clientSocket, BitConverter.GetBytes(data.payloadSize), 4) != 4)
                throw new RFIDCommandCenterException("Failed to write command bytes", null);
            if (netCommObject.writeTo(clientSocket, BitConverter.GetBytes(data.tickTime), 8) != 8)
                throw new RFIDCommandCenterException("Falied to write command bytes", null);
            if (data.payloadSize != 0)
                if (netCommObject.writeTo(clientSocket, data.payload, data.payloadSize) != data.payloadSize)
                    throw new RFIDCommandCenterException("Falied to write command bytes", null);
            return true;
        }
        public abstract void serverThreadRoutine(Object state);
        //public bool sendStream(byte[] data,int timeoutUs,int size)
        //{

        //}

        //public int receiveStream(byte[]data,int waitTimeUs,int size)
        //{
        //    if (!clientSocket.Poll(waitTimeUs, SelectMode.SelectRead))
        //        return 0;
        //    int ttlBytesRecv = netCommObject.readFrom(clientSocket, data,size);

        //}

        protected Client(Socket who,NetworkCommunication netObject)
        {
            clientSocket = who;
            netCommObject = netObject;
        }
    }
}
