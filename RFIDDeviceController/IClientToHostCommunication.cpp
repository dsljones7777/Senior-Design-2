#include "pch.h"
#include "IClientToHostCommunication.h"
#include "WindowsClientToHostCommunication.h";

using namespace RFIDDeviceController::Communication;

IClientToHostCommunication * RFIDDeviceController::Communication::getCommunicationObject()
{
#ifdef _WIN32
	return new WindowsClientToHostCommunication();
#else 
	return nullptr;
#endif
}

RFIDDeviceController::Communication::IClientToHostCommunication::IClientToHostCommunication()
{
}

RFIDDeviceController::Communication::IClientToHostCommunication::~IClientToHostCommunication()
{
}
