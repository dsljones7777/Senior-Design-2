#pragma once
namespace RFIDDeviceController
{
	namespace Settings
	{
		class ClientSettings
		{
		public:

			char const * hostConnectionString;
			int networkTickRate;		//The host should receive messages at least this often. In ms
			int readTickRate;			//How long the reader's read timeout is

			//How long to remember a tag for. If the tag arrive cmd occurs how long till a tag leave cmd  should occur.
			//If tag is present for longer than this then a TAG_PRESENT_TOO_LONG command is sent
			int tagRememberanceTime;

			//How long a tag is considered having left
			int tagLeaveTime;

			//How long a tag will be present before being considered arrived
			int tagArriveTime;

			bool lockByDefault;

			int retriesBeforeReconnect;

			ClientSettings();
			virtual ~ClientSettings();

		};
	}
}


