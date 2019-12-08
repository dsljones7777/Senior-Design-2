using System;
using System.Collections.Generic;
using static SharedLib.Network.UIClientConnection;
using static Network.NetworkLib;
using static SharedLib.SharedModels;

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
            public bool guest;
        }

        [Serializable]
        public class DeleteTagRPC : UINetworkPacket
        {
            public string name;
        }

        [Serializable]
        public class WriteTagRPC : UINetworkPacket
        {
            public string targetSerialNumber;
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

            public List<string> removedSerials;
        }

        [Serializable]
        public class GetAllConnectedDevicesRPC : UINetworkPacket
        {
            public List<string> serialNumbers;

            public GetAllConnectedDevicesRPC()
            {
            }
        }

        [Serializable]
        public class GetUnconnectedDevicesRPC : UINetworkPacket
        {
            //TODO: Implement server side
            public List<string> serialNumbers;

            public GetUnconnectedDevicesRPC()
            {
            }
        }

        [Serializable]
        public class DeviceStatus
        {
            //TODO: refactor to different file
            public string serialNumber;
            public bool connected;
            public bool inDB;
            public bool isVirtual;
        }

        [Serializable]
        public class GetAllDevicesRPC : UINetworkPacket
        {
            //TODO: implement server side
            public List<DeviceStatus> devices;

            public GetAllDevicesRPC()
            {
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
            }
        }

        [Serializable]
        public class EditUserRPC : UINetworkPacket
        {

            public Nullable<Role> userRole;
            public string pass;
            public string username;

            public EditUserRPC(string usrName, string password, Role usrRole, bool roleProvided)
            {
                username = usrName;
                pass = String.IsNullOrWhiteSpace(password) ? null : password;
                if (roleProvided)
                    userRole = usrRole;
            }
        }

        [Serializable]
        public class ViewUserRPC : UINetworkPacket
        {
            public List<SharedModels.SharedUsers> userList;
            public ViewUserRPC()
            {
            }
        }

        [Serializable]
        public class ViewTagsRPC : UINetworkPacket
        {
            public List<SharedModels.ViewTagModel> tagList;
            public bool nonSystemTagsOnly = false;
            public ViewTagsRPC()
            {
            }
        }

        [Serializable]
        public class ViewLocationsRPC : UINetworkPacket
        {
            public List<SharedModels.LocationModel> locationList;
            public ViewLocationsRPC()
            {
            }
        }

        [Serializable]
        public class EditLocationRPC : UINetworkPacket
        {
            public string currentLocationName;
            public string newLocationName;
            public string readerSerialIn;
            public string readerSerialOut;

            public string oldReaderSerialIn;
            public string oldReaderSerialOut;
        }

        [Serializable]
        public class AddTagToLocation : UINetworkPacket
        {
            public int tagID;
            public int locationID;
        }

        [Serializable]
        public class EditTagRPC : UINetworkPacket
        {
            public byte[] tagNumber;
            public string name;
            public bool? lost;
            public bool? guest;
        }

        [Serializable]
        public class RemoveConnectedDevicesRPC : UINetworkPacket
        {

        }

        [Serializable]
        public class SaveAllowedLocationsRPC : UINetworkPacket
        {
            public string tagName;
            public List<string> locationNames;
        }   
        
        [Serializable]
        public class SaveConnectedDeviceRPC : UINetworkPacket
        {
            public string serialNumber;
        }

        [Serializable]
        public class TagArriveRPC : UINetworkPacket
        {
            public byte[] tagNumber;
            public string deviceSerialNumber;
        }

        [Serializable]
        public class TagLeaveRPC : UINetworkPacket
        {
            public byte[] tagNumber;
        }
        
        [Serializable]
        public class ViewAllowedLocationsRPC : UINetworkPacket
        {
            public List<ViewAllowedLocationsModel> allowedLocationList;
            public string TagName;

            public ViewAllowedLocationsRPC()
            {
            }
        }

        [Serializable]
        public class ViewGuestTagsRPC : UINetworkPacket
        {
            public List<SharedModels.ViewTagModel> tagList;

            public ViewGuestTagsRPC()
            {
            }
        }

        [Serializable]
        public class GetUniqueSerialNumbersRPC : UINetworkPacket
        {
            public List<string> serialNumberList;

            public GetUniqueSerialNumbersRPC()
            {
            }
        }

        [Serializable]
        public class FunctionCallStatusRPC : UINetworkPacket
        {
            public string error;
            public bool waitForResponse;
        }

        [Serializable]
        public class DeleteAllowedLocationsRPC : UINetworkPacket
        {
            public string tagName;
            public string locationName;
        }

        [Serializable]
        public class ChangeDeviceModeRPC : UINetworkPacket
        {
            public string deviceSerial;
            public bool virtualMode;
        }
    }
    
}
