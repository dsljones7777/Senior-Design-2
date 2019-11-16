#pragma once

#include "AntennaSettings.h"
#include <tm_reader.h>
namespace RFIDDeviceController
{
	namespace Settings
	{

		class ReaderSettings
		{

		public:
			static const int MAX_ANTENNAS = 4;
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

			void resetSettings();
			ReaderSettings();
			virtual ~ReaderSettings();
#ifdef _WIN32
		private:
			bool initWindows();
			char uriBuffer[16] = "tmr:///";
#endif
		};




	}
}

