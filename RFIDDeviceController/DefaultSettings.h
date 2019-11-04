#pragma once
#include "Settings.h"
#include "DeviceSettings.h"
namespace Identification
{
	namespace Settings
	{
		class DefaultReaderDeviceSettings : public RFID::ReaderSettings
		{
		public:
			DefaultReaderDeviceSettings()
			{
				regionToUse = TMR_REGION_NA;
				uriConnectionString = "tmr:///COM5";
				defaultReadPower = 3000;
				defaultWritePower = 250;
				defaultBaudRate = 921600;
				defaultCommandTimeout = 0;
				defaultTransportTimeout = 0;
				powerMode = TMR_SR_PowerMode::TMR_SR_POWER_MODE_FULL;
			}
		};

		struct DefaultClientSettings : public ClientSettings
		{
		public:
			DefaultClientSettings()
			{
				hostConnectionString = "127.0.0.1:30563";
				networkTickRate = 1000;
			}
		};

		class DefaultDeviceSettings : public DeviceSettings
		{
		private:
			DefaultReaderDeviceSettings defReaderSettings;
			DefaultClientSettings defClientSettings;
		public:
			DefaultDeviceSettings()
			{
				rdrSettings = &defReaderSettings;
				clientSettings = &defClientSettings;
			}

		};
	}
}

