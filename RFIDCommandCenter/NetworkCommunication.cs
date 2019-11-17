using System;
using System.Net;
using System.Net.Sockets;

namespace RFIDCommandCenter
{
    
    

    

    class NetworkCommunication
    {
        public enum ClientType
        {
            CLIENT_UI_APP = 1,
            CLIENT_RFID_DEVICE = 2
        };

        internal static NetworkCommunication createNetworkCommunicationObject()
        {
            return new SecureNetworkCommunication();
        }


        public void start()
        {
            if (hasStarted)
                return;
            IPHostEntry hostIPAddressEntries = Dns.GetHostEntry("localhost");
            IPAddress hostIPAddress = hostIPAddressEntries.AddressList[1];
            IPEndPoint hostEndpoint = new IPEndPoint(hostIPAddress, 52437);
            serverSocket = new Socket(hostIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(hostEndpoint);
            serverSocket.Listen(BACKLOG_CONNECTION_AMOUNT);
            hasStarted = true;
        }

        public Client acceptPendingClient( int timeoutUs)
        {
            Socket acceptedSocket;
            byte[] buffer = new byte[NetworkCode.HEADER_SIZE];
            try
            {
                if (serverSocket.Poll(timeoutUs, SelectMode.SelectRead))
                    acceptedSocket = serverSocket.Accept();
                else
                    return null;
            }
            catch (Exception e)
            {
                throw new CommandCenterException("Failed to accept client connection", e);
            }
            try
            {

                int bytesRead = readFrom(acceptedSocket, buffer, 4);
                if (bytesRead != 4)
                    throw new CommandCenterException("Failed to read client initialization bytes (less than 4 bytes were read)", null);

            }
            catch (Exception e)
            {
                try
                {
                    acceptedSocket.Disconnect(false);
                    acceptedSocket.Close(5);
                }
                catch
                {

                }
                throw new CommandCenterException("Failed to read client initialization bytes", e);
            }
            int type = BitConverter.ToInt32(buffer, 0);
            Client returnval;
            switch (type)
            {
                case (int)ClientType.CLIENT_RFID_DEVICE:
                    acceptedSocket.Receive(buffer, NetworkCode.HEADER_SIZE - 4, SocketFlags.None);
                    returnval = new RFIDDeviceClient(acceptedSocket, this);
                    break;
                case (int)ClientType.CLIENT_UI_APP:
                    returnval = new UIClient(acceptedSocket, this);
                    break;
                default:
                    throw new CommandCenterException("The client initialization packet is invalid", null);
            }
            return returnval;
        }

        internal virtual int readFrom(Socket who, byte[] buffer, int size = 0)
        {
            if (size == 0)
                return who.Receive(buffer);
            else
                return who.Receive(buffer, 0, size, SocketFlags.None);
        }

        internal virtual int writeTo(Socket who, byte[] buffer, int size)
        {
            return who.Send(buffer, 0, size, SocketFlags.None);
        }
        
        protected NetworkCommunication()
        {
            
        }

        const int BACKLOG_CONNECTION_AMOUNT = 5;
        bool hasStarted = false;
        Socket serverSocket;
    }
}
