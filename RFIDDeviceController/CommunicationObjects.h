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

		

		struct UnlockParam : public NetworkBytecode
		{
		};

		struct StartParam : public NetworkBytecode
		{
		public:
			StartParam()
			{
				cmd = (uint32_t)RPCCommands::START;
				payloadSize = 0;
			}
		};

		struct LockParam : public NetworkBytecode
		{
		};

		struct UpdateParam : public NetworkBytecode
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

		struct StopParam : public NetworkBytecode
		{

		};

		struct ResetTickCountParam : public NetworkBytecode
		{

		};

		struct WriteTagParam : public NetworkBytecode
		{
		public:
			int numberOfTags;
			int readTickTime;
			int readPowerLevel;
			int successful;
			char epc[12];
		};

		struct TagArriveParam : public NetworkBytecode
		{

			char epc[12];
		public:
			TagArriveParam(char const * pEPC)
			{
				cmd = (uint32_t)RPCCommands::TAG_ARRIVE;
				payloadSize = sizeof(TagArriveParam) - sizeof(NetworkBytecode);
				for (int i = 0; i < 12; i++)
					epc[i] = pEPC[i];
			}
		};

		struct TagLeaveParam : public TagArriveParam
		{
		public:
			TagLeaveParam(char const * pEPC) : TagArriveParam(pEPC)
			{
				cmd = (uint32_t)RPCCommands::TAG_LEAVE;
			}
		};

		struct TagPresentTooLongParam : public TagArriveParam
		{
			char epc[12];
		public:
			TagPresentTooLongParam(char * pEPC) : TagArriveParam(pEPC)
			{
				cmd = (uint32_t)RPCCommands::TAG_PRESENT_TOO_LONG;
			}
		};

		struct AliveParam : public NetworkBytecode
		{
		public:
			AliveParam()
			{
				cmd = (uint32_t)RPCCommands::ALIVE;
				payloadSize = sizeof(AliveParam) - sizeof(NetworkBytecode);
			}
		};

		struct PingParam : public NetworkBytecode
		{
		public:
			int totalTickTime;
		};
	}
}

