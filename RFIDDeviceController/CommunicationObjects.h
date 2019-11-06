#pragma once
#include <stdint.h>
namespace Identification
{
	namespace Communication
	{
		struct NetworkBytecode
		{
		public:
			static int const MAX_PAYLOAD_SIZE = 112;
			uint32_t cmd;
			uint32_t payloadSize;
			uint64_t tickTime;
		};

		struct GenericNetworkBytecode : public NetworkBytecode
		{
		public:
			char payload[MAX_PAYLOAD_SIZE];
		};

		class RPCCommands
		{
		public:
			static int const UNLOCK = 1;
			static int const LOCK = 2;
			static int const START = 3;
			static int const UPDATE = 4;
			static int const STOP = 5;
			static int const TAG_ARRIVE = 6;
			static int const TAG_LEAVE = 7;
			static int const OBJECT_PRESENT = 8;
			static int const TAG_PRESENT_TOO_LONG = 9;
			static int const GET_DEVICE_TICK_COUNT = 10;
			static int const RESET_DEVICE_TICK_COUNT = 11;
		private:
			RPCCommands();
		};

		struct TagArriveParam : public NetworkBytecode
		{
		public:
			char epc[12];

			TagArriveParam()
			{
				cmd = RPCCommands::TAG_ARRIVE;
				payloadSize = sizeof(TagArriveParam) - sizeof(NetworkBytecode);
			}
		};

		struct TagLeaveParam : public TagArriveParam
		{
		public:
			TagLeaveParam()
			{
				cmd = RPCCommands::TAG_LEAVE;
			}
		};
	}
}

