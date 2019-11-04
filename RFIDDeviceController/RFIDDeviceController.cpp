// RFIDDeviceController.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include "RFIDReader.h"
#include "DefaultSettings.h"
using namespace Identification;
using namespace Identification::Settings;
int main()
{
	DefaultDeviceSettings settings;

	//Create reader and get it ready
	RFIDReader reader(nullptr);
	if (!reader.initialize(settings.rdrSettings))
		return -1;

	//Load host. Chat w/ host about getting the party started. Send reader init info
	//IClientToHostCommunication * comm = getCommunicationMethod();
	//if (!comm->connect(settings.clientSettings->hostConnectionString))
		//return -1;
	uint64_t tick = 0;
	//do
	//{
	//	//Process all received rpcs
	//		//Interpret the command and act accordingly
	//	//Perform a read operation
	//	//Tell the host who was present during the read operations
	//} while (true);


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
