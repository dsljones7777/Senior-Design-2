using System;

namespace Network
{
    public class NetworkLib
    {
        [Serializable]
        public enum NetworkCommands
        {
            CONNECT = 1,
            SAVE_TAG,
            WRITE_TAG,
            DELETE_TAG,
            SAVE_SYSTEM_USER,
            DELETE_SYSTEM_USER,
            GET_LOCATION_LIST,
            SAVE_LOCATION,
            DELETE_LOCATION,
            PING_DEVICE,
            SETUP_DEVICE,
            ERROR_PROMPT
        };

        [Serializable]
        public class NullSerializer
        {

        }
    }
}
