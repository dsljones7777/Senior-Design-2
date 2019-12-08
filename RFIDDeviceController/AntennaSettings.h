#pragma once

#include <stdint.h>
namespace RFIDDeviceController
{
	namespace Settings
	{
		class AntennaSettings
		{
		public:
			int32_t readPower;
			int32_t writePower;
			uint8_t port;
			bool useToRead;
			bool useToWrite;
		};
	}
}