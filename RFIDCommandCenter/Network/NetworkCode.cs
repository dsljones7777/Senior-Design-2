using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RFIDCommandCenter
{
    class DeviceCommandPacket : ICommandPacket
    {

        internal const int HEADER_SIZE = 16;
        internal const int MAX_PAYLOAD_SIZE = 112;
        int command;
        int payloadSize;
        ulong tickTime;

        public virtual void deserialize(Stream inputStream)
        {
            byte[] buffer = new byte[HEADER_SIZE];
            int ttlBytesReceived = 0;
            while(ttlBytesReceived < HEADER_SIZE)
            {
                int bytesRead = inputStream.Read(buffer, ttlBytesReceived, HEADER_SIZE - ttlBytesReceived);
                if(bytesRead == 0)
                    throw new CommandCenterException("The specified device network packet was too small", null);
                ttlBytesReceived += bytesRead;
            }
            command = BitConverter.ToInt32(buffer, 0);
            payloadSize = BitConverter.ToInt32(buffer, 4);
            tickTime = BitConverter.ToUInt64(buffer, 8);
            if(payloadSize > MAX_PAYLOAD_SIZE)
                throw new CommandCenterException("The specified device network packet was too large", null);
            while (ttlBytesReceived < HEADER_SIZE + payloadSize)
            {
                int bytesRead = inputStream.Read(buffer, ttlBytesReceived, HEADER_SIZE + payloadSize - ttlBytesReceived);
                if (bytesRead == 0)
                    throw new CommandCenterException("The specified device network packet was too small", null);
                ttlBytesReceived += bytesRead;
            }
        }

        public virtual void serialize(Stream outputStream)
        {
            outputStream.Write(BitConverter.GetBytes(command), 0, 4);
            outputStream.Write(BitConverter.GetBytes(payloadSize), 0, 4);
            outputStream.Write(BitConverter.GetBytes(tickTime), 0, 8);
        }

        protected abstract void de
        
    }
    
}
