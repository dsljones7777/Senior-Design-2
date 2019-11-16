#include "pch.h"
#include "ClientSettings.h"
using namespace RFIDDeviceController::Settings;

ClientSettings::ClientSettings()
{
	hostConnectionString = "127.0.0.1 52437";
	networkTickRate = 1000;
	readTickRate = 200;
	tagRememberanceTime = 100000;
	tagLeaveTime = 1000;
	lockByDefault = true;
	retriesBeforeReconnect = 25;
	tagArriveTime = 600;
}


ClientSettings::~ClientSettings()
{
}
