using System;

namespace Network
{
    public class NetworkLib
    {
        [Serializable]
        public class LocationObjList
        {
            public string LocationName { get; set; }
            public string ReaderSerialIn { get; set; }
        }

        [Serializable]
        public class ServerMessage
        {
            public string deviceSerial;     //Null serials indicate it is a server related issue not particular to any device
            public string message;
            public bool retry = false;
        }

        public enum Role
        {
            Admin = 1,
            BaseUser = 2
        }
    }
}
