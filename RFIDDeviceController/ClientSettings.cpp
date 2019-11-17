#include "pch.h"
#include "ClientSettings.h"
using namespace RFIDDeviceController::Settings;

ClientSettings::ClientSettings()
{
	hostConnectionString = "127.0.0.1 52437";
	networkTickRate = 5000;
	tagRememberanceTime = 100000;
	tagLeaveTime = 700;
	lockByDefault = true;
	retriesBeforeReconnect = 25;
	tagArriveTime = 0;
}


ClientSettings::~ClientSettings()
{
}
