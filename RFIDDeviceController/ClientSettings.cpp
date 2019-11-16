#include "pch.h"
#include "ClientSettings.h"
using namespace RFIDDeviceController::Settings;

ClientSettings::ClientSettings()
{
	hostConnectionString = "127.0.0.1 52437";
	networkTickRate = 500;
	readTickRate = 100;
	tagRememberanceTime = 8000;
	tagLeaveTime = 2000;
	lockByDefault = true;
	retriesBeforeReconnect = 25;
	tagArriveTime = 1000;
}


ClientSettings::~ClientSettings()
{
}
