#include "pch.h"
#include "IClientToHostCommunication.h"
#include "WindowsClientToHostCommunication.h";

using namespace Identification::Communication;

IClientToHostCommunication * Identification::Communication::getCommunicationObject()
{
#ifdef _WIN32
	return new WindowsClientToHostCommunication();
#else 
	return nullptr;
#endif
}

Identification::Communication::IClientToHostCommunication::IClientToHostCommunication()
{
}

Identification::Communication::IClientToHostCommunication::~IClientToHostCommunication()
{
}
