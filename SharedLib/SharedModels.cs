using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Network.NetworkLib;

namespace SharedLib
{
    public class SharedModels
    {
        [Serializable]
        public class SharedUsers
        {
            public string Username { get; set; }
            public Role UserRole { get; set; }
        }

        [Serializable]
        public class ViewTagModel
        {
            public string TagName { get; set; }
            public byte[] TagNumber { get; set; }
            public string LastLocation { get; set; }
            public bool InLocation { get; set; }
            public bool LostTag { get; set; }
            public bool GuestTag { get; set; }
        }

        [Serializable]
        public class LocationModel
        {
            public string LocationName { get; set; }
            public string ReaderSerialIn { get; set; }
            public string ReaderSerialOut { get; set; }
        }

        [Serializable]
        public class ViewAllowedLocationsModel
        {
            public int ID { get; set; }
            public string LocationName { get; set; }
            public bool TagAllowedInLoc { get; set; }
        }
    }
}
