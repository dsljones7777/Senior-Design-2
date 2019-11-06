#pragma once
#include "Settings.h"
namespace Identification
{
	namespace Settings
	{
		class DeviceSettings
		{
		public:
			RFID::ReaderSettings * rdrSettings;
			ClientSettings * clientSettings;
			DeviceSettings()
			{

			}
			virtual ~DeviceSettings()
			{

			}
		};
	}
}


