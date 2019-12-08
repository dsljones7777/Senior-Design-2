#pragma once
#include "ReaderSettings.h"
#include "ClientSettings.h"
namespace RFIDDeviceController
{
	namespace Settings
	{
		class DeviceSettings
		{
		public:
			ReaderSettings * rdrSettings;
			ClientSettings * clientSettings;
			AntennaSettings * antennaSettings;
			DeviceSettings();
			virtual ~DeviceSettings();
		};
	}
}


