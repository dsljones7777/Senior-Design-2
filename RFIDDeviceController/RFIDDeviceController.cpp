// RFIDDeviceController.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include "RFIDReader.h"
#include "DefaultSettings.h"
#include "IClientToHostCommunication.h"
using namespace Identification;
using namespace Identification::Settings;
using namespace Identification::Communication;


int main()
{
	uint64_t tick = 0;
	DefaultDeviceSettings settings;

	//Create reader and get it ready
	RFIDReader reader(nullptr);
	if (!reader.initialize(settings.rdrSettings))
		return -1;


	//Load host. Chat w/ host about getting the party started. Send reader init info
	IClientToHostCommunication * comm =  getCommunicationObject();
	if (!comm->init())
		return -1;
RESTART_CONNECTION:
	while (!comm->connectTo(settings.clientSettings->hostConnectionString))
	{
		//Flash an LED or something
	}
	//Turn off LED or other status indicator
	
	//Tell the command center who we are
	GenericNetworkBytecode buffer;
	buffer.cmd = (uint32_t)RPCCommands::START;
	buffer.tickTime = 0;
	buffer.payloadSize = 0;
	if (!comm->write(&buffer))
	{
		comm->disconnect(nullptr);
		goto RESTART_CONNECTION;
	}

	//Wait for update command from command center
	bool breakLoop = false;
	while (!breakLoop)
	{
		if (!comm->read(&buffer))
		{
			comm->disconnect(nullptr);
			goto RESTART_CONNECTION;
		}
		
		switch (buffer.cmd)
		{
		case (uint32_t)RPCCommands::UNLOCK:
			//Flash LED as unlocking
			break;
		case (uint32_t)RPCCommands::UPDATE:
			breakLoop = true;
			break;
		case (uint32_t)RPCCommands::LOCK:
			//Turn on LED as locking
			break;
		default:
			break;
		}
	}
	
	//Wait for start command from command center
	breakLoop = false;
	while (!breakLoop)
	{
		switch (buffer.cmd)
		{
		case (uint32_t)RPCCommands::UNLOCK:
			//Flash LED as unlocking
			break;
		case (uint32_t)RPCCommands::UPDATE:
			breakLoop = true;
			break;
		case (uint32_t)RPCCommands::LOCK:
			//Turn on LED as locking
			break;
		default:
			break;
		}
	}
	
	//Start program loop. Reconnect when client socket error occurs
	while (true)
	{

	}

	TMR_TagReadData readTags[25];
	int totalTags = 25;
	do
	{
		if (!reader.readTags(readTags, 1000, totalTags))
			return -1;
		for (int i = 0; i < totalTags && i < 25; i++)
		{
			TMR_TagData data = readTags[i].tag;
			std::cout << "Read tag: ";
			for (int j = 0; j < data.epcByteCount; j++)
				std::cout << std::hex << (int)data.epc[j];
			std::cout << '\n';
		}
	} while (true);

	return 0;
}
