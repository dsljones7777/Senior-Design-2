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
	//Create the reader
	TMR_Status startupError = TMR_create(&reader, devSettings->uriConnectionString);
	if (startupError != TMR_SUCCESS)
		return false;

	//Connect to the device
	startupError = TMR_connect(&reader);
	if (startupError != TMR_SUCCESS)
		return false;

	//Get version information
	if (!initVersionInfo())
		return false;

	//Initialize power info
	if (!initPowerInfo(devSettings))
		return false;

	//Set up the region according to the settings
	if (!initRegionSettings(devSettings))
		return false;

	//Set up operation info
	if (!initOperationSettings(devSettings))
		return false;

	//Initialize the antennas
	if (!initAntennas(devSettings))
		return false;
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

bool RFIDDeviceController::RFIDReader::turnOnLed1()
{
	return true;
}

bool RFIDDeviceController::RFIDReader::turnOnLed2()
{
	return true;
}
