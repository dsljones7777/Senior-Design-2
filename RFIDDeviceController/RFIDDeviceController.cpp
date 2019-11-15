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
enum class ErrorCodes : int
{
	NONE = 0,
	DEVICE_FAILED_TO_READ = 1,
	TAG_TOO_LONG = 2,
};
class DeviceHandler
{
	static const int TAG_BUFFER_SIZE = 25;
	uint64_t currentTick = 0;
	DefaultDeviceSettings settings;
	RFIDReader reader= RFIDReader(nullptr);
	IClientToHostCommunication * comm = nullptr;
	bool exitProgram = false;
	TMR_TagReadData tagsRead[TAG_BUFFER_SIZE];

	GenericNetworkBytecode buffer;
protected:

	void turnOnLed(int ledNumber)
	{

	}
	void turnOffLed(int ledNumber)
	{

	}
	bool isLedOn(int ledNumber)
	{
		return false;
	}
	bool isLedOff(int ledNumber)
	{
		return true;
	}
	static const int ERROR_LED_NUMBER = 1;
	static const int PING_LED_NUMBER = 2;

	void startReader()
	{

	}
	void unlockDoor()
	{

	}

	void lockDoor()
	{

	}

	bool startConnection(bool dueToFault)
	{
		//Connect to the host (command center)
		if (!comm->connectTo(settings.clientSettings->hostConnectionString))
			return false;

		//Tell the command center who we are
		*(StartNetParam *)(&buffer) = StartNetParam();
		buffer.tickTime = currentTick;
		if (!comm->write(&buffer))
		{
			comm->disconnect(nullptr);
			return false;
		}

		//Wait for start command from command center
		bool breakLoop = false;
		while (!breakLoop)
		{
			if (!comm->read(&buffer))
			{
				comm->disconnect(nullptr);
				return false;
			}

			switch (buffer.cmd)
			{
			case (uint32_t)RPCCommands::UNLOCK:
				unlockDoor();
				break;
			case (uint32_t)RPCCommands::UPDATE:
				break;
			case (uint32_t)RPCCommands::LOCK:
				//Turn on LED as locking
				break;
			case (uint32_t)RPCCommands::START:
				breakLoop = true;
				break;
			default:
				break;
			}
		}
		return true;
	}

	void connectToCommandCenter(bool onFault)
	{
		if (onFault)
			comm->disconnect(nullptr);
		//Flash led's if connection fails, until connection has been made
		while (!startConnection(onFault))
		{
			if (isLedOn(ERROR_LED_NUMBER))
				turnOffLed(ERROR_LED_NUMBER);
			else
				turnOnLed(ERROR_LED_NUMBER);
		}
		turnOffLed(ERROR_LED_NUMBER);
	}

	bool interpretCommand()
	{
		return true;
	}

	bool checkForAndInterpretCommand()
	{
		int retryCount, bytesAvailable;
		//Check for command from command center
		for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
			if (comm->peek(bytesAvailable))
				break;
		if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
		{
			connectToCommandCenter(true);
			return false;
		}

		//If data is available read and decipher the command
		if (bytesAvailable >= sizeof(NetworkBytecode))
		{
			for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
				if (interpretCommand())
					break;
			if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
			{
				connectToCommandCenter(true);
				return false;
			}
		}

		//Interpret
		return interpretCommand();
	}

	bool sendTagToServer(TMR_TagReadData * rTag)
	{
		TMR_TagData data = rTag->tag;
		*(TagArriveNetParam *)&buffer = TagArriveNetParam((char const *)data.epc);
		int retryCount;
		//Check for command from command center
		while (true)
		{
			for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
				if (comm->write(&buffer))
					break;
			if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
				connectToCommandCenter(true);
			else
				break;
		}
		return true;
	}

	int readTags()
	{
		int totalTagsRead = 0, retryCount;
		if (!reader.readTags(tagsRead, settings.clientSettings->readTickRate, totalTagsRead))
		{
			*(DeviceErrorNetParam*)&buffer = DeviceErrorNetParam((int)ErrorCodes::DEVICE_FAILED_TO_READ);
			for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
				if (comm->write((NetworkBytecode *)&buffer))
					break;
			if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
			{
				connectToCommandCenter(true);
				return -1;
			}
		}
		return totalTagsRead;
	}
public:
	int run()
	{
		//Create reader and get it ready
		if (!reader.initialize(settings.rdrSettings))
			return -1;
		
		//Load host. Chat w/ host about getting the party started. Send reader init info
		comm = getCommunicationObject();
		if (!comm || !comm->init())
			return -1;
		connectToCommandCenter(false);

		//Start main program loop
		while (!exitProgram)
		{	
			if (!checkForAndInterpretCommand())
				continue;
			
			int totalTagsRead = readTags();
			if (totalTagsRead <= 0)
				continue;
			
			for (int i = 0; i < totalTagsRead && i < TAG_BUFFER_SIZE; i++)
				sendTagToServer(tagsRead + i);
		}
		return 0;
	}
};
int main()
{
	DeviceHandler handler;
	int returnval = handler.run();
	return returnval;
}
