#include "pch.h"
#include "DeviceController.h"

using namespace RFIDDeviceController;
using namespace RFIDDeviceController::Communication;
using namespace RFIDDeviceController::Settings;

DeviceController::DeviceController()
{
	
}

DeviceController::~DeviceController()
{
}

void RFIDDeviceController::DeviceController::turnOnLed(int ledNumber)
{

}

void RFIDDeviceController::DeviceController::turnOffLed(int ledNumber)
{

}

bool RFIDDeviceController::DeviceController::isLedOn(int ledNumber)
{
	return false;
}

bool RFIDDeviceController::DeviceController::isLedOff(int ledNumber)
{
	return true;
}

void RFIDDeviceController::DeviceController::resetTicksTillDead()
{
	ticksTillDead = settings.clientSettings->networkTickRate / settings.clientSettings->readTickRate + 1;
}

void RFIDDeviceController::DeviceController::startReader()
{
	//Tell server device failed to connect
	do
	{
		if (settings.rdrSettings->uriConnectionString != nullptr)
			break;
		this->sendDeviceError((int)ErrorCodes::DEVICE_FAILED_TO_CONNECT);
	} while (true);
}

void RFIDDeviceController::DeviceController::unlockDoor()
{

}

void RFIDDeviceController::DeviceController::lockDoor()
{

}

bool RFIDDeviceController::DeviceController::startConnection(bool dueToFault)
{
	//Connect to the host (command center)
	if (!comm->connectTo(settings.clientSettings->hostConnectionString))
		return false;
	sendStartToServer();
	this->checkAndExecuteCommand((int)CommandCodes::START);

	//TODO: Device Initialization 
	//Tell the command center who we are
	//Update settings or program
	return true;
}

void RFIDDeviceController::DeviceController::connectToCommandCenter(bool onFault)
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

bool RFIDDeviceController::DeviceController::executeCommand(int expectedCommand)
{
	if (!expectedCommand  && buffer.cmd != expectedCommand)
		return false;
	int32_t adjustmentValue;
	switch (buffer.cmd)
	{
		case (int)CommandCodes::CONFIRMATION_SYNC_TICK_COUNT:
			adjustmentValue = (int32_t)(buffer.tickTime - currentTick);
			for (int i = 0; i < REMEMBERANCE_TAG_BUFFER_SIZE; i++)
			{
				if (epcMemoryBuffer[i].valid)
				{
					epcMemoryBuffer[i].stayTime = (uint32_t)((int32_t)epcMemoryBuffer[i].stayTime + adjustmentValue);
					if(epcMemoryBuffer[i].leaveTime > 0)
						epcMemoryBuffer[i].leaveTime = (uint32_t)((int32_t)epcMemoryBuffer[i].leaveTime + adjustmentValue);
				}
			}
			currentTick = buffer.tickTime;
			break;
		case (int)CommandCodes::LOCK:
			//lock door
			break;
		case (int)CommandCodes::UNLOCK:
			//unlock door
			break;
		case (int)CommandCodes::UPDATE:
			//update device params 
			break;
		case (int)CommandCodes::PING:
			//ping device by flashing leds
			break;
		case (int)CommandCodes::WRITE_TAG:
			//write to a tag
			break;
		case (int)CommandCodes::WAIT:
			//Pump alive messages
#ifdef _WIN32
			Sleep(settings.clientSettings->readTickRate);
#else
//TODO: ADD warning of missing wait/sleep function
#endif
			*(AliveNetParam*)&buffer = AliveNetParam();
			buffer.tickTime = currentTick;
			sendWithoutAssurance();
			break;
	}
	return true;
}

void RFIDDeviceController::DeviceController::checkAndExecuteCommand(int expectedCommand)
{
	/*
		Keep in loop until all condition are met:
			-network is ok, client is connected to server, the socket has been polled
			-if a command was expected it has been received and guaranteed to be executed
			-if a command packet was present from server it was executed
				*unless it was received before an expected command packet (Exec not guaranteed)
	*/
	do 
	{
		int retryCount, bytesAvailable;
		//Check for command from command center
		for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
			if (comm->peek(bytesAvailable))
				break;
		if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
		{
			connectToCommandCenter(true);
			continue;
		}

		//If data is available read and decipher the command
		if (bytesAvailable >= sizeof(NetworkBytecode))
		{
			//Keep in loop until command packet is read
			while (true)
			{
				for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
					if (comm->read(&buffer))
						break;
				if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
					connectToCommandCenter(true);
				else
					break;
			}
			if (executeCommand(expectedCommand))
				break;
		}
		else if (!expectedCommand)
			break;
		
	} while (true);
}

void RFIDDeviceController::DeviceController::sendStartToServer()
{
	*(StartNetParam *)&buffer = StartNetParam();
	buffer.tickTime = currentTick;
	while (true)
	{
		int retryCount;
		for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
			if (comm->write(&buffer))
				break;
		if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
			connectToCommandCenter(true);
		else
			break;
	}
}

void RFIDDeviceController::DeviceController::sendTagArriveToServer(char const * epc)
{
	*(TagArriveNetParam *)&buffer = TagArriveNetParam(epc);
	buffer.tickTime = currentTick;
	sendAndAssure();
}

void RFIDDeviceController::DeviceController::tellServerAlive()
{
	*(AliveNetParam*)&buffer = AliveNetParam();
	buffer.tickTime = currentTick;
	sendAndAssure();
}

void RFIDDeviceController::DeviceController::sendTagLeaveToServer(char const * epc)
{
	*(TagLeaveNetParam *)&buffer = TagLeaveNetParam(epc);
	buffer.tickTime = currentTick;
	sendAndAssure();
}

void RFIDDeviceController::DeviceController::sendTagPresentTooLongToServer(char const * epc)
{
	*(TagPresentTooLongNetParam *)&buffer = TagPresentTooLongNetParam(epc);
	buffer.tickTime = currentTick;
	sendAndAssure();
}

void RFIDDeviceController::DeviceController::sendDeviceError(int errorCode)
{
	*(DeviceErrorNetParam *)&buffer = DeviceErrorNetParam(errorCode);
	buffer.tickTime = currentTick;
	sendAndAssure();
}

void RFIDDeviceController::DeviceController::sendAndAssure()
{
	while (true)
	{
		int retryCount;
		for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
			if (comm->write(&buffer))
				break;
		if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
			connectToCommandCenter(true);
		else
			break;
	}
	checkAndExecuteCommand((int)CommandCodes::CONFIRMATION_SYNC_TICK_COUNT);
	resetTicksTillDead();
}

void RFIDDeviceController::DeviceController::sendWithoutAssurance()
{
	while (true)
	{
		int retryCount;
		for (retryCount = 0; retryCount < settings.clientSettings->retriesBeforeReconnect; retryCount++)
			if (comm->write(&buffer))
				break;
		if (retryCount >= settings.clientSettings->retriesBeforeReconnect)
			connectToCommandCenter(true);
		else
			break;
	}
	resetTicksTillDead();
}


void RFIDDeviceController::DeviceController::updateTagsWithServer()
{
	int totalTagsRead = TAG_BUFFER_SIZE, retryCount;
	if (!reader.readTags((char*)&epcBuffer, settings.clientSettings->readTickRate, totalTagsRead))
	{
		while (true)
		{
			*(DeviceErrorNetParam*)&buffer = DeviceErrorNetParam((int)ErrorCodes::DEVICE_FAILED_TO_READ);
			sendAndAssure();
		}
	}
	readTagCount += totalTagsRead;
	currentTick += settings.clientSettings->readTickRate;
	//Find or create tag in memory
	for (int epcIndex = 0; epcIndex < totalTagsRead && epcIndex < TAG_BUFFER_SIZE; sentTagCount++, epcIndex++)
	{
		bool found = false;
		char const * epc = ((char const *)epcBuffer) + 12 * epcIndex;

		//Find in memory
		for (int j = 0; j < REMEMBERANCE_TAG_BUFFER_SIZE; j++)
			if (epcMemoryBuffer[j].valid && epcMemoryBuffer[j].epcFront == *(uint64_t const *)epc && epcMemoryBuffer[j].epcEnd == *((uint32_t const *)epc + 2))
			{
				epcMemoryBuffer[j].foundDuringCurrentRead = true;
				found = true;
				break;
			}
		if (found)
			continue;

		//Find an empty memory slot
		int rIndex;
		for (rIndex = 0; rIndex < REMEMBERANCE_TAG_BUFFER_SIZE; rIndex++)
			if (!epcMemoryBuffer[rIndex].valid)
			{
				epcMemoryBuffer[rIndex] = EPCRememberance(epc);
				break;
			}
		if (rIndex >= REMEMBERANCE_TAG_BUFFER_SIZE)
		{
			this->sendDeviceError((int)ErrorCodes::TAG_MEMORY_BUFFER_FULL);
			epcIndex--;
		}

	}

	//Update tag times and send to server
	for (int i = 0; i < REMEMBERANCE_TAG_BUFFER_SIZE; i++)
	{
		if (!epcMemoryBuffer[i].valid)
			continue;
		//2 cases found during current read or not found
		if (epcMemoryBuffer[i].foundDuringCurrentRead)
		{
			if (!epcMemoryBuffer[i].sentServerTagPresent)
			{
				if (epcMemoryBuffer[i].stayTime >= settings.clientSettings->tagArriveTime)
				{
					this->sendTagArriveToServer((char*)&epcMemoryBuffer[i].epcFront);
					epcMemoryBuffer[i].sentServerTagPresent = true;
				}
				else
				{
					if (settings.clientSettings->readTickRate > epcMemoryBuffer[i].leaveTime)
						epcMemoryBuffer[i].leaveTime = 0;
					else
						epcMemoryBuffer[i].leaveTime -= settings.clientSettings->readTickRate;
				}
			}
			else if(!epcMemoryBuffer[i].toldServerTagPresentTooLong)
			{
				if (epcMemoryBuffer[i].stayTime >= settings.clientSettings->tagRememberanceTime)
				{
					this->sendTagPresentTooLongToServer((char*)&epcMemoryBuffer[i].epcFront);
					epcMemoryBuffer[i].toldServerTagPresentTooLong = true;
				}
			}
			epcMemoryBuffer[i].stayTime += settings.clientSettings->readTickRate;

			
		}
		else
		{
			//If a tag has not been reported set stay time to left(no server notification) or decrement by read tick rate (keep positive)
			if (!epcMemoryBuffer[i].sentServerTagPresent && settings.clientSettings->readTickRate > epcMemoryBuffer[i].stayTime)
				epcMemoryBuffer[i].valid = false;
			else if (!epcMemoryBuffer[i].sentServerTagPresent)
				epcMemoryBuffer[i].stayTime -= settings.clientSettings->readTickRate;
			else if (epcMemoryBuffer[i].leaveTime >= settings.clientSettings->tagLeaveTime)
			{
				epcMemoryBuffer[i].valid = false;
				this->sendTagLeaveToServer((char*)&epcMemoryBuffer[i].epcFront);
			}
			epcMemoryBuffer[i].leaveTime += settings.clientSettings->readTickRate;
		}
	}

}



int RFIDDeviceController::DeviceController::run()
{

	//Load host. Chat w/ host about getting the party started. Send reader init info
	comm = RFIDDeviceController::Communication::getCommunicationObject();
	if (!comm || !comm->init())
		return -1;
	connectToCommandCenter(false);

	//Create reader and get it ready
	do
	{
		if (!settings.rdrSettings->uriConnectionString)
			sendDeviceError((int)ErrorCodes::DEVICE_FAILED_TO_CONNECT);
		else if (!reader.initialize(settings.rdrSettings))
			sendDeviceError((int)ErrorCodes::DEVICE_FAILED_TO_START);
		else
			break;
	} while (true);

	
	resetTicksTillDead();


	//Start main program loop
	while (!exitProgram)
	{
		checkAndExecuteCommand();
		if (ticksTillDead <= 0)
			tellServerAlive();

		//Reset all tags found last read value
		for (int i = 0; i < REMEMBERANCE_TAG_BUFFER_SIZE; i++)
			epcMemoryBuffer[i].foundDuringCurrentRead = false;

		//Poll for tags, descrease ticks till tell server it is dead if no tags are found (update stats)
		updateTagsWithServer();
		
		
		ticksTillDead--;
	}
	return 0;
}
