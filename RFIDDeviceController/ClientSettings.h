#pragma once
namespace RFIDDeviceController
{
	namespace Settings
	{
		class ClientSettings
		{
		public:

			char const * hostConnectionString;

			//The host should receive messages at least this often. In ms
			int networkTickRate;				

			//How long to remember a tag for. If the tag arrive cmd occurs how long till a tag leave cmd  should occur.
			//If tag is present for longer than this then a TAG_PRESENT_TOO_LONG command is sent
			int tagRememberanceTime;

			//How long a tag is considered having left
			int tagLeaveTime;

			//How long a tag will be present before being considered arrived
			int tagArriveTime;

			//Does the reader lock the door?
			bool lockByDefault;

			//How many retrys that occur before the device disconnects from the server and attempts to reconnect
			int retriesBeforeReconnect;

			ClientSettings();
			virtual ~ClientSettings();

		};
	}
}


