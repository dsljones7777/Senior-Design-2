#include "pch.h"
#include "ClientSettings.h"
using namespace RFIDDeviceController::Settings;

ClientSettings::ClientSettings()
{
	//hostConnectionString = "127.0.0.1 52437";
	networkTickRate = 1000;
	tagRememberanceTime = 100000;
	tagLeaveTime = 800;
	lockByDefault = true;
	retriesBeforeReconnect = 25;
	tagArriveTime = 500;
}


ClientSettings::~ClientSettings()
{
}
