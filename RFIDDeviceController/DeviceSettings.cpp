#include "pch.h"
#include "DeviceSettings.h"
using namespace RFIDDeviceController::Settings;
ReaderSettings currentRdrSettings = ReaderSettings();
ClientSettings currentClientSettings = ClientSettings();
AntennaSettings  currentAntennaSettings = AntennaSettings();

RFIDDeviceController::Settings::DeviceSettings::DeviceSettings()
{
	this->rdrSettings = &currentRdrSettings;
	this->clientSettings = &currentClientSettings;
	this->antennaSettings = &currentAntennaSettings;
}

RFIDDeviceController::Settings::DeviceSettings::~DeviceSettings()
{
}
