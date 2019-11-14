#pragma once
#include <stdint.h>
#include <tm_reader.h>
namespace Identification
{
	namespace Settings
	{
		

		struct HostSettings
		{
		public:

			int doorUnlockTime = 2500;
			int doorUnlockDelay = 1000;
		};

		struct ClientSettings
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

			bool lockByDefault;

			int retriesBeforeReconnect;
		};

		namespace RFID
		{
			const static int MAX_ANTENNAS = 32;
			const static int MAX_REGIONS = 32;
			const static int MAX_STRING_SIZE = 32;

			struct AntennaSettings
			{
			public:
				int32_t readPower;
				int32_t writePower;
				uint8_t port;
				bool useToRead;
				bool useToWrite;
			};

			struct ReaderSettings
			{
			public:
				char const * uriConnectionString = nullptr;
				int32_t defaultReadPower = -1;
				int32_t defaultWritePower = -1;
				uint32_t defaultBaudRate = -1;
				uint32_t defaultCommandTimeout = -1;
				uint32_t defaultTransportTimeout = -1;
				TMR_SR_PowerMode powerMode = TMR_SR_PowerMode::TMR_SR_POWER_MODE_INVALID;
				TMR_Region regionToUse = TMR_REGION_NONE;
				AntennaSettings antennasParams[MAX_ANTENNAS];
				int numOfAntennas = -1;
			};
		}
	}
}