#pragma once
#include <stdint.h>
namespace Identification
{
	namespace Communication
	{
		enum class RPCCommands : uint32_t
		{
			UNLOCK = 1,		//Unlock the door
			START = 2,		//Sending : device startup / indentification, Receiving : device start process loop
			LOCK = 3,		//Lock the door
			UPDATE = 4,		//Update device settings
			STOP = 5,		//Stop program loop, shutdown
			TAG_ARRIVE = 6,	//First time tag has been recognized
			TAG_LEAVE = 7,	//Tag no longer present before TAG_PRESENT_TOO_LONG timeframe
			OBJECT_PRESENT = 8,	//NOT USED - made for infared sensor detecting when an object is present
			TAG_PRESENT_TOO_LONG = 9,	//Tag has been identified by the reader as being in spot too long
			GET_DEVICE_TICK_COUNT = 10,	//NOT USED - Have the device report it's current tick count
			RESET_DEVICE_TICK_COUNT = 11,	//Have the device reset it's tick count
			WRITE_TAG = 12,					//Write data to the tag (reduce power, read to ensure only one tag is present, write)
			ALIVE = 13,						//Send dummy frame every time to live
			PING = 14,						//Turn device light on/off for the default read timeout rate
			DEVICE_ERROR=15
		};

		struct NetworkBytecode
		{
		public:
			static int const MAX_PAYLOAD_SIZE = 112;
			uint32_t cmd;
			uint32_t payloadSize;
			uint64_t tickTime;

			//Determines if a received packet is valid. Only use on device client
			bool isValid();
		};

		struct GenericNetworkBytecode : public NetworkBytecode
		{
		public:
			char payload[MAX_PAYLOAD_SIZE];
		};

		

		struct UnlockNetParam : public NetworkBytecode
		{
		};

		struct StartNetParam : public NetworkBytecode
		{
		public:
			StartNetParam()
			{
				cmd = (uint32_t)RPCCommands::START;
				payloadSize = 0;
			}
		};

		struct LockNetParam : public NetworkBytecode
		{
		};

		struct UpdateNetParam : public NetworkBytecode
		{
		public:
			int readPower;
			int writePower;
			int baudRate;
			int commandTimeout;
			int transportTimeout;
			int powerMode;
			int netTickRate;
			int readTickRate;
			int tagRememberTime;
			int tagLeaveTime;
			bool lockByDefault;
		};

		struct StopNetParam : public NetworkBytecode
		{

		};

		struct ResetTickCountNetParam : public NetworkBytecode
		{

		};

		struct WriteTagNetParam : public NetworkBytecode
		{
		public:
			int numberOfTags;
			int readTickTime;
			int readPowerLevel;
			int successful;
			char epc[12];
		};

		struct TagArriveNetParam : public NetworkBytecode
		{

			char epc[12];
		public:
			TagArriveNetParam(char const * pEPC)
			{
				cmd = (uint32_t)RPCCommands::TAG_ARRIVE;
				payloadSize = sizeof(TagArriveNetParam) - sizeof(NetworkBytecode);
				for (int i = 0; i < 12; i++)
					epc[i] = pEPC[i];
			}
		};

		struct TagLeaveNetParam : public TagArriveNetParam
		{
		public:
			TagLeaveNetParam(char const * pEPC) : TagArriveNetParam(pEPC)
			{
				cmd = (uint32_t)RPCCommands::TAG_LEAVE;
			}
		};

		struct TagPresentTooLongNetParam : public TagArriveNetParam
		{
			char epc[12];
		public:
			TagPresentTooLongNetParam(char * pEPC) : TagArriveNetParam(pEPC)
			{
				cmd = (uint32_t)RPCCommands::TAG_PRESENT_TOO_LONG;
			}
		};

		struct AliveNetParam : public NetworkBytecode
		{
		public:
			AliveNetParam()
			{
				cmd = (uint32_t)RPCCommands::ALIVE;
				payloadSize = sizeof(AliveNetParam) - sizeof(NetworkBytecode);
			}
		};

		struct PingNetParam : public NetworkBytecode
		{
		public:
			int totalTickTime;
		};

		struct DeviceErrorNetParam : public NetworkBytecode
		{
		public:
			int errorCode;
			DeviceErrorNetParam(int errCode)
			{
				cmd = (uint32_t)RPCCommands::DEVICE_ERROR;
				payloadSize = sizeof(DeviceErrorNetParam) - sizeof(NetworkBytecode);
				errorCode = errCode;
			}
		};
	}
}

