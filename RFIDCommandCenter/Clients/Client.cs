using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RFIDCommandCenter
{
    interface ICommandPacket
    {
        void serialize(Stream outputStream);
        void deserialize(Stream inputStream);
    }
    abstract class Client
    {
        public enum ClientType
        {
            DEVICE = 1,
            UI = 2
        }

        public abstract ClientType getTypeOfClient
        {
            get;
        }

        public abstract string identifier
        {
            get;
            set;
        }

        protected abstract void serverThreadRoutine(Object state);

        protected abstract void serverThreadOnStart(Object state);

        public bool exit
        {
            get
            {
                return m_exit;
            }
            set
            {
                m_exit = value;
            }
        }

        public bool pauseExecution
        {
            get
            {
                return m_pauseExecution;
            }
            set
            {
                m_pauseExecution = value;
            }
        }

        public string serverErrorMessage
        {
            get
            {
                return m_serverError;
            }
            set
            {
                m_serverError = value;
            }
        }

        public bool hasExited
        {
            get
            {
                return m_exited;
            }
            set
            {
                m_exited = value;
            }
        }

        public void clientThreadEntry(Object state)
        {
            try
            {
                serverThreadOnStart(state);
                while (!exit)
                    serverThreadRoutine(state);
            }
            catch(Exception e)
            {
                serverErrorMessage = e.Message;
            }
            hasExited = true;
        }

        public bool receivePacket(ICommandPacket data, int waitTimeUs)
        {
            if (!clientSocket.Poll(waitTimeUs, SelectMode.SelectRead))
                return false;
            data.deserialize(clientStream);
            return true;
        }

        public bool sendPacket(ICommandPacket data, int waitTimeUs)
        {
            if (!clientSocket.Poll(waitTimeUs, SelectMode.SelectWrite))
                return false;
            data.serialize(clientStream);
            return true;
        }

        protected Client(Socket who)
        {
            clientSocket = who;
            clientStream = new NetworkStream(clientSocket);
        }

        private volatile bool m_exit = false;
        private volatile bool m_pauseExecution = false;
        private volatile string m_serverError = null;
        private volatile bool m_exited = false;
        protected Socket clientSocket;
        protected NetworkStream clientStream;
    }

    
}
