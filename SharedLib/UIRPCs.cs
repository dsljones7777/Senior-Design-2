using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedLib.Network.UIClientConnection;

namespace SharedLib
{
    namespace Network
    {
        [Serializable]
        public class ErrorReplyRPC : UINetworkPacket
        {
            public string msg;
            public string serialNumber;
            public bool retry;
        }

        [Serializable]
        public class SaveTagRPC : UINetworkPacket
        {
            public byte[] tagNumber;
            public string name;
        }

        [Serializable]
        public class DeleteTagRPC : UINetworkPacket
        {
            public byte[] tagNumber;
            string name;
        }

        [Serializable]
        public class WriteTagRPC : UINetworkPacket
        {
            public byte[] targetTag;
            public byte[] newTagBytes;
        }

        [Serializable]
        public class SaveSystemUserRPC : UINetworkPacket
        {
            public string username;
            public string password;
            public int role;
        }

        [Serializable]
        public class DeleteSystemUserRPC : UINetworkPacket
        {
            public string username;
        }

        [Serializable]
        public class SaveLocationRPC : UINetworkPacket
        {
            public string locationName;
            public string readerSerialIn;
            public string readerSerialOut;
        }

        [Serializable]
        public class DeleteLocationRPC : UINetworkPacket
        {
            public string locationName;
        }

        [Serializable]
        public class GetUnconnectedDevicesRPC : UINetworkPacket
        {
            //TODO: Implement server side
            public List<string> serialNumbers;

            public GetUnconnectedDevicesRPC()
            {
                this.bothWay = true;
            }
        }

        [Serializable]
        public class DeviceStatus
        {
            //TODO: refactor to different file
            public string serialNumber;
            public bool connected;
            public bool inDB;
        }

        [Serializable]
        public class GetAllDevicesRPC : UINetworkPacket
        {
            //TODO: implement server side
            public List<DeviceStatus> devices;

            public GetAllDevicesRPC()
            {
                this.bothWay = true;
            }
        }

        [Serializable]
        public class LoginUserRPC : UINetworkPacket
        {
            public string username;
            public string password;
            public bool isValidLogin;
            public bool isAdmin;

            public LoginUserRPC()
            {
                this.bothWay = true;
            }
        }
    }
    
}
