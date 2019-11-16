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
}
