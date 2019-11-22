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
	if (!reader.initialized)
		return;
	if (!reader.setLedState(ledNumber, true))
		return;
	if (ledNumber == 1)
		light1On = true;
	else
		light2On = true;
}

void RFIDDeviceController::DeviceController::turnOffLed(int ledNumber)
{
	if (!reader.initialized)
		return;
	if (!reader.setLedState(ledNumber, false))
		return;
	if (ledNumber == 1)
		light1On = false;
	else
		light2On = false;
}

bool RFIDDeviceController::DeviceController::isLedOn(int ledNumber)
{
	return (ledNumber == 1) ? light1On : light2On;
}

bool RFIDDeviceController::DeviceController::isLedOff(int ledNumber)
{
	return (ledNumber == 1) ? !light1On : !light2On;
}

void RFIDDeviceController::DeviceController::resetTicksTillDead()
{
	ticksTillDead = settings.clientSettings->networkTickRate + currentTick;
}

void RFIDDeviceController::DeviceController::startReader()
{
	//Tell server device failed to connect
	do
	{
		if (settings.rdrSettings->uriConnectionString != nullptr)
			break;
		
		this->sendDeviceError((int)ErrorCodes::DEVICE_FAILED_TO_CONNECT);
		settings.rdrSettings->resetSettings();
	} while (true);
	//Create reader and get it ready
	do
	{
		if (reader.initialize(settings.rdrSettings))
			break;
		sendDeviceError((int)ErrorCodes::DEVICE_FAILED_TO_START);
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
	//Adjust tick rates
	int adjustmentValue = (int32_t)(buffer.tickTime - currentTick);
	int realTimeAdjustment = realReadTickRate - adjustmentValue;
	if (realTimeAdjustment < settings.rdrSettings->MIN_READER_TIMEOUT)
		realReadTickRate = settings.rdrSettings->MIN_READER_TIMEOUT;
	else if (realTimeAdjustment > settings.clientSettings->networkTickRate)
		realReadTickRate = settings.clientSettings->networkTickRate;
	else if (realTimeAdjustment > settings.rdrSettings->MAX_READER_TIMEOUT)
		realReadTickRate = settings.rdrSettings->MAX_READER_TIMEOUT;
	else
		realReadTickRate = realTimeAdjustment;
	currentTick = buffer.tickTime;

	if (!expectedCommand  && buffer.cmd != expectedCommand)
		return false;
	switch (buffer.cmd)
	{
		case (int)CommandCodes::CONFIRMATION_SYNC_TICK_COUNT:
			
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
		case (int)CommandCodes::DEVICE_ERROR:
			return handleDeviceError();

		case(int)CommandCodes::START:
			return true;

		default:
			return false;
	}
	return true;
}

bool RFIDDeviceController::DeviceController::handleDeviceError()
{
	DeviceErrorNetParam * errorHdlCmd = (DeviceErrorNetParam *)&buffer;
	return  !errorHdlCmd->wait;
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
	sendWithoutAssurance();
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

bool RFIDDeviceController::DeviceController::sendDeviceError(int errorCode)
{
	DeviceErrorNetParam * pErrorParam = (DeviceErrorNetParam *)&buffer;
	*pErrorParam = DeviceErrorNetParam(errorCode);
	pErrorParam->tickTime = currentTick;
	sendWithoutAssurance();
	checkAndExecuteCommand((int)CommandCodes::DEVICE_ERROR);
	if (pErrorParam->abortOperation)
		return false;
	return true;
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
	//Reset all tags found last read value
	for (int i = 0; i < REMEMBERANCE_TAG_BUFFER_SIZE; i++)
		epcMemoryBuffer[i].foundDuringCurrentRead = false;
	int totalTagsRead = TAG_BUFFER_SIZE, retryCount;

	//Try to read, if error occurs determine whether to continue function or abort
	if (!reader.readTags((char*)&epcBuffer, realReadTickRate, totalTagsRead))
		if (!sendDeviceError((int)ErrorCodes::DEVICE_FAILED_TO_READ))
			return;

	readTagCount += totalTagsRead;
	currentTick += realReadTickRate;
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
			if (!this->sendDeviceError((int)ErrorCodes::TAG_MEMORY_BUFFER_FULL))
				return;
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
					if (realReadTickRate > epcMemoryBuffer[i].leaveTime)
						epcMemoryBuffer[i].leaveTime = 0;
					else
						epcMemoryBuffer[i].leaveTime -= realReadTickRate;
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
			epcMemoryBuffer[i].stayTime += realReadTickRate;

			
		}
		else
		{
			//If a tag has not been reported set stay time to left(no server notification) or decrement by read tick rate (keep positive)
			if (!epcMemoryBuffer[i].sentServerTagPresent && realReadTickRate > epcMemoryBuffer[i].stayTime)
				epcMemoryBuffer[i].valid = false;
			else if (!epcMemoryBuffer[i].sentServerTagPresent)
				epcMemoryBuffer[i].stayTime -= realReadTickRate;
			else if (epcMemoryBuffer[i].leaveTime >= settings.clientSettings->tagLeaveTime)
			{
				epcMemoryBuffer[i].valid = false;
				this->sendTagLeaveToServer((char*)&epcMemoryBuffer[i].epcFront);
			}
			epcMemoryBuffer[i].leaveTime += realReadTickRate;
		}
	}
	
}

int RFIDDeviceController::DeviceController::run()
{

	//Connect to server / host
	resetTicksTillDead();
	realReadTickRate = settings.rdrSettings->readTickRate;
	comm = RFIDDeviceController::Communication::getCommunicationObject();
	if (!comm || !comm->init())
		return -1;
	connectToCommandCenter(false);
	startReader();
	turnOffLed(1);
	turnOffLed(2);/*
	char epcMsg[] = "u r a bitch";
	reader.writeTag(epcMsg, 1000, 1000);*/

	//Start main program loop
	while (!exitProgram)
	{
		checkAndExecuteCommand();
		

		//Poll for tags, descrease ticks till tell server it is dead if no tags are found (update stats)
		updateTagsWithServer();
		if (ticksTillDead <= currentTick)
			tellServerAlive();
		ticksTillDead--;
	}
	return 0;
}
