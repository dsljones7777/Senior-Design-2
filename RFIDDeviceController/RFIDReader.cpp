#include "pch.h"
#include "RFIDReader.h"
using namespace RFIDDeviceController;
using namespace RFIDDeviceController::Settings;
RFIDDeviceController::RFIDReader::RFIDReader(ILog * logAssistant)
{
	log = logAssistant;
}
RFIDDeviceController::RFIDReader::~RFIDReader()
{

}

bool RFIDDeviceController::RFIDReader::initialize(ReaderSettings * devSettings)
{
	//Create the reader,connect to device, get version,power,region and antenna info
	TMR_Status startupError = TMR_create(&reader, devSettings->uriConnectionString);
	if (startupError != TMR_SUCCESS)
		return false;
	startupError = TMR_connect(&reader);
	if (startupError != TMR_SUCCESS)
		return false;
	if (!initVersionInfo())
		return false;
	if (!initPowerInfo(devSettings))
		return false;
	if (!initRegionSettings(devSettings))
		return false;
	if (!initOperationSettings(devSettings))
		return false;
	if (!initAntennas(devSettings))
		return false;
	if (!initGPIO(devSettings))
		return false;
	if (!initMode())
		return false;
	initialized = true;
	return true;
}

bool RFIDDeviceController::RFIDReader::setReadPower(int32_t pwr)
{
	if (pwr > maxPower)
		pwr = maxPower;
	else if (pwr < minPower)
		pwr = minPower;

	TMR_Status status = TMR_paramSet(&reader, TMR_Param::TMR_PARAM_RADIO_READPOWER, &pwr);
	if (status != TMR_SUCCESS)
		return false;
	readPower = pwr;
	return true;
}

bool RFIDDeviceController::RFIDReader::setWritePower(int32_t pwr)
{
	if (pwr > maxPower)
		pwr = maxPower;
	else if (pwr < minPower)
		pwr = minPower;
	TMR_Status status = TMR_paramSet(&reader, TMR_Param::TMR_PARAM_RADIO_WRITEPOWER, &pwr);
	if (status != TMR_SUCCESS)
		return false;
	writePower = pwr;
	return true;
}

//Total tags must be <= total tags in TMR_getNextTag

bool RFIDDeviceController::RFIDReader::readRemainingTags(TMR_TagReadData * tags, int & totalTags)
{
	for (int i = 0; i < totalTags; i++)
	{
		TMR_Status status = TMR_getNextTag(&reader, tags + i);
		if (status != TMR_SUCCESS)
		{
			totalTags = i;
			return false;
		}
	}
}

bool RFIDDeviceController::RFIDReader::readTags(char * epcBufferArray, int timeoutMs, int & totalTags)
{
	int tagLimit = totalTags;
	TMR_TagReadData tag;
	TMR_Status status = TMR_read(&reader, timeoutMs, &totalTags);
	if (status != TMR_SUCCESS)
		return false;
	for (int i = 0; i < totalTags && i < tagLimit; i++)
	{
		status = TMR_getNextTag(&reader, &tag);
		if (status != TMR_SUCCESS)
		{
			lastError = ReaderErrors::GET_NEXT_TAG;
			totalTags = i;
			return false;
		}
		if (tag.tag.epcByteCount != 12)
		{
			lastError = ReaderErrors::EPC_TOO_LONG;
			totalTags = i;
			return false;
		}
		for (int j = 0; j < 12; j++)
			epcBufferArray[i * 12 + j] = tag.tag.epc[j];
	}
	return true;
}

uint32_t RFIDDeviceController::RFIDReader::getDefaultReadPower()
{
	return readPower;
}

uint32_t RFIDDeviceController::RFIDReader::getDefaultWritePower()
{
	return writePower;
}

bool RFIDDeviceController::RFIDReader::setCommandTimeout(uint32_t timeoutMs)
{
	if (TMR_paramSet(&reader, TMR_Param::TMR_PARAM_COMMANDTIMEOUT, &timeoutMs) == TMR_SUCCESS)
	{
		commandTimeout = timeoutMs;
		return true;
	}
	return false;
}

bool RFIDDeviceController::RFIDReader::setTransportTimeout(uint32_t timeoutMs)
{
	if (TMR_paramSet(&reader, TMR_Param::TMR_PARAM_TRANSPORTTIMEOUT, &timeoutMs) == TMR_SUCCESS)
	{
		transportTimeout = timeoutMs;
		return true;
	}
	return false;
}

int RFIDDeviceController::RFIDReader::getTotalAntennas() const
{
	return totalAntennas;
}

RFIDAntenna const * RFIDDeviceController::RFIDReader::getAntenna(int index) const
{
	if (index >= totalAntennas)
		return nullptr;
	return antennas + index;
}

bool RFIDDeviceController::RFIDReader::setCurrentRegion(TMR_Region region)
{
	TMR_Status status = TMR_paramSet(&reader, TMR_PARAM_REGION_ID, &region);

	//Apply the region if the parameter was successfully set
	if (status == TMR_SUCCESS)
		currentRegion = region;
	return status == TMR_SUCCESS;
}

bool RFIDDeviceController::RFIDReader::isRegionSupported(TMR_Region region)
{
	//Select the maximum index to iterate, go through each region and determine if the specified region is in the list
	int maxIndex = regionList.len < regionList.max ? regionList.len : regionList.max;
	for (int i = 0; i < maxIndex; i++)
		if (supportedRegions[i] == region)
			return true;
	return false;
}

bool RFIDDeviceController::RFIDReader::setBaudRate(uint32_t newBaud)
{
	switch (newBaud)
	{
	case 11520:
	case 9600:
	case 921600:
	case 19200:
	case 38400:
	case 57600:
	case 230400:
	case 460800:
		break;
	default:
		return false;
	}
	if (TMR_paramSet(&reader, TMR_Param::TMR_PARAM_BAUDRATE, &newBaud) == TMR_SUCCESS)
	{
		baudRate = newBaud;
		return true;
	}
	return false;
}

bool RFIDDeviceController::RFIDReader::setReaderPowerMode(TMR_SR_PowerMode mode)
{
	if (TMR_paramSet(&reader, TMR_Param::TMR_PARAM_POWERMODE, &mode) == TMR_SUCCESS)
	{
		powerMode = mode;
		return true;
	}
	return false;
}

bool RFIDDeviceController::RFIDReader::setDevicePowerMode(TMR_SR_PowerMode mode)
{
	TMR_Status status = TMR_paramSet(&reader, TMR_Param::TMR_PARAM_POWERMODE, &mode);
	if (status != TMR_SUCCESS)
		return false;
	powerMode = mode;
	return true;
}

bool RFIDDeviceController::RFIDReader::setLedState(int led, bool high)
{
	TMR_GpioPin pin[1];
	pin[0].id = led == 1 ? led1GPIO : led2GPIO;
	pin[0].output = true;
	pin[0].high = high;
	if (TMR_gpoSet(&reader,1,pin) == TMR_SUCCESS)
		return false;
	return true;
}

bool RFIDDeviceController::RFIDReader::initMode()
{
	TMR_ReadPlan readPlan;
	memset(&readPlan, 0, sizeof(TMR_ReadPlan));
	TMR_uint8List antennaList;
	readPlan.enableAutonomousRead = false;
	readPlan.type = TMR_ReadPlanType::TMR_READ_PLAN_TYPE_SIMPLE;
	readPlan.weight = 1;
	readPlan.u.simple.antennas.len = 1;
	readPlan.u.simple.antennas.list = &antennas[1].antennaPort;
	readPlan.u.simple.antennas.max = 1;
	readPlan.u.simple.protocol = TMR_TAG_PROTOCOL_GEN2;
	if (TMR_paramSet(&reader, TMR_Param::TMR_PARAM_READ_PLAN, (void*)&readPlan) != TMR_SUCCESS)
		return false;
	return true;

}

//Initializes the version info contained by this object by getting the information from the rfid device
//Override to allow more information to be obtained

bool RFIDDeviceController::RFIDReader::initVersionInfo()
{
	//Get the serial number
	TMR_String serialNumberString;
	serialNumberString.max = 32;
	serialNumberString.value = serialNumber;
	TMR_Status startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_VERSION_SERIAL, &serialNumberString);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get the software version
	TMR_String softwareVerString;
	softwareVerString.max = 32;
	softwareVerString.value = softwareVersion;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_VERSION_SOFTWARE, &softwareVerString);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get the hardware version
	TMR_String hwVerString;
	hwVerString.max = 32;
	hwVerString.value = hardwareVersion;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_VERSION_HARDWARE, &hwVerString);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get the model number
	TMR_String modelString;
	modelString.max = 32;
	modelString.value = model;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_VERSION_MODEL, &modelString);
	if (startupError != TMR_SUCCESS)
		return false;
	return  true;
}

//Initializes the region by getting the supported regions and trying to set the reader to the specified region if supported
//Override to change the way regions are initialized

bool RFIDDeviceController::RFIDReader::initRegionSettings(Settings::ReaderSettings * devSettings)
{
	//Get the current region
	currentRegion = TMR_REGION_NONE;
	TMR_Status startupError = TMR_paramGet(&reader, TMR_PARAM_REGION_ID, &currentRegion);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get the list of supported regions
	regionList.list = supportedRegions;
	regionList.max = Settings::MAX_REGIONS;
	regionList.len = 0;
	startupError = TMR_paramGet(&reader, TMR_PARAM_REGION_SUPPORTEDREGIONS, &regionList);
	if (startupError != TMR_SUCCESS)
		return false;

	//Set to the specified region
	if (isRegionSupported(devSettings->regionToUse))
		setCurrentRegion(devSettings->regionToUse);
	else
		return false;

	return true;
}

//Initializes the antennas and their respective states

bool RFIDDeviceController::RFIDReader::initAntennas(Settings::ReaderSettings * devSettings)
{
	//Get list of all antenna ports
	uint8_t antennaPorts[Settings::MAX_ANTENNAS];
	TMR_uint8List antennaList;
	antennaList.list = antennaPorts;
	antennaList.max = Settings::MAX_ANTENNAS;
	TMR_Status startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_ANTENNA_PORTLIST, &antennaList);
	if (startupError != TMR_SUCCESS)
		return false;
	totalAntennas = antennaList.len < antennaList.max ? antennaList.len : antennaList.max;

	//Get list of all connected antenna ports
	TMR_uint8List connectedAntennaList;
	uint8_t connectedAntennas[Settings::MAX_ANTENNAS];
	connectedAntennaList.list = connectedAntennas;
	connectedAntennaList.max = Settings::MAX_ANTENNAS;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_ANTENNA_CONNECTEDPORTLIST, &connectedAntennaList);
	if (startupError != TMR_SUCCESS)
		return false;

	//Measure the return losses of each antenna
	TMR_PortValueList returnLossesList;
	TMR_PortValue portReturnLosses[Settings::MAX_ANTENNAS];
	returnLossesList.list = portReturnLosses;
	returnLossesList.max = Settings::MAX_ANTENNAS;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_ANTENNA_RETURNLOSS, &returnLossesList);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get the read power of each antenna
	TMR_PortValueList readPowerList;
	TMR_PortValue readPowers[Settings::MAX_ANTENNAS];
	readPowerList.list = readPowers;
	readPowerList.max = Settings::MAX_ANTENNAS;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_PORTREADPOWERLIST, &readPowerList);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get the write power of each antenna
	TMR_PortValueList writePowerList;
	TMR_PortValue writePowers[Settings::MAX_ANTENNAS];
	writePowerList.list = writePowers;
	writePowerList.max = Settings::MAX_ANTENNAS;
	startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_PORTREADPOWERLIST, &writePowerList);
	if (startupError != TMR_SUCCESS)
		return false;

	//Initialize each of the antenna object
	int maxIndex = (antennaList.len < antennaList.max) ? antennaList.len : antennaList.max;
	for (int i = 0; i < maxIndex; i++)
	{
		antennas[i] = RFIDAntenna(&reader, antennaList.list[i]);

		//Determine if antenna is connected (ON or OFF)
		bool found = false;
		int maxConAntIndex = (connectedAntennaList.len < connectedAntennaList.max) ? connectedAntennaList.len : connectedAntennaList.max;
		for (int j = 0; j < maxConAntIndex; j++)
			if (connectedAntennaList.list[j] == antennas[i].antennaPort)
			{
				found = true;
				break;
			}
		antennas[i].state = found ? RFIDAntenna::AntennaState::ON : RFIDAntenna::AntennaState::OFF;
		
		//Find the return lost value for the current antenna
		for (int j = 0; j < maxIndex; j++)
			if (returnLossesList.list[j].port == antennas[i].antennaPort)
			{
				antennas[i].returnLoss = returnLossesList.list[j].value;
				break;
			}

		//Determine the read power for the current antenna
		int maxReadPowerIndex = (readPowerList.len < readPowerList.max) ? readPowerList.len : readPowerList.max;
		for (int j = 0; j < maxReadPowerIndex; j++)
			if (readPowerList.list[j].port == antennas[i].antennaPort)
			{
				antennas[i].readPower = readPowerList.list[j].value;
				break;
			}

		//if a readpower was not found then read power is the device default
		if (antennas[i].readPower == -1)
		{
			antennas[i].readPower = readPower;
			antennas[i].isReadPwrDefault = true;
		}

		//Determine the write power for the current antenna
		int maxWritePowerIndex = (writePowerList.len < writePowerList.max) ? writePowerList.len : writePowerList.max;
		for (int j = 0; j < maxWritePowerIndex; j++)
			if (writePowerList.list[j].port == antennas[i].antennaPort)
			{
				antennas[i].writePower = writePowerList.list[j].value;
				break;
			}

		//if a write power was not found then write power is the device default
		if (antennas[i].writePower == -1)
		{
			antennas[i].writePower = writePower;
			antennas[i].isWritePwrDefault = true;
		}

	}

	return true;
}

//Retreives all power information for the device and sets the desired power 

bool RFIDDeviceController::RFIDReader::initPowerInfo(Settings::ReaderSettings * settings)
{
	//Get max radio power
	TMR_Status status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_POWERMAX, &maxPower);
	if (status != TMR_SUCCESS)
		return false;

	//Get min radio power
	status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_POWERMIN, &minPower);
	if (status != TMR_SUCCESS)
		return false;

	//Get the power mode of the device
	status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_POWERMODE, &powerMode);
	if (status != TMR_SUCCESS)
		return false;

	//Get the reader power
	status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_READPOWER, &readPower);
	if (status != TMR_SUCCESS)
		return false;

	//Get the writer power
	status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_WRITEPOWER, &writePower);
	if (status != TMR_SUCCESS)
		return false;

	//Set the default power mode
	if (settings->powerMode != TMR_SR_PowerMode::TMR_SR_POWER_MODE_INVALID && !setDevicePowerMode(settings->powerMode))
		return false;

	//Set the default read power
	if (settings->defaultReadPower != -1 && !setReadPower(settings->defaultReadPower))
		return false;

	//Set the default write power
	if (settings->defaultWritePower != -1 && !setWritePower(settings->defaultWritePower))
		return  false;
	return true;
}

bool RFIDDeviceController::RFIDReader::initOperationSettings(Settings::ReaderSettings * settings)
{
	//Get the baud rate
	TMR_Status status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_BAUDRATE, &baudRate);
	if (status != TMR_SUCCESS)
		return false;

	//Get the command timeout
	status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_COMMANDTIMEOUT, &commandTimeout);
	if (status != TMR_SUCCESS)
		return false;

	//Get the transport timeout
	status = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_TRANSPORTTIMEOUT, &transportTimeout);
	if (status != TMR_SUCCESS)
		return false;

	//Set the default baud rate
	if (settings->defaultBaudRate > 0 && !setBaudRate(settings->defaultBaudRate))
		return false;

	//Set the default command timeout
	if (settings->defaultCommandTimeout > 0 && !setCommandTimeout(settings->defaultCommandTimeout))
		return false;

	//Set the default transport timeout
	if (settings->defaultTransportTimeout > 0 && !setTransportTimeout(settings->defaultTransportTimeout))
		return false;
	return true;
}

bool RFIDDeviceController::RFIDReader::initGPIO(Settings::ReaderSettings * settings)
{
	TMR_uint8List inputs;
	uint8_t buffer[16];
	inputs.list = buffer;
	inputs.max = 16;
	if (TMR_paramGet(&reader, TMR_Param::TMR_PARAM_GPIO_OUTPUTLIST, &inputs) != TMR_SUCCESS)
		return false;
	if (inputs.len < 2)
		return false;
	led1GPIO = buffer[0];
	led2GPIO = buffer[1];
	return true;
}
