// RFIDCommandCenter.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <tm_reader.h>
#include <tmr_params.h>
#include <string>
#include <list>

/*
	Helps convert the ThingMagic Region to a human readable format or from a human readable format to
	a ThingMagic region
*/

class RegionHelper
{
public:
	static std::string const & getRegionName(TMR_Region region)
	{
		switch (region)
		{
		case TMR_Region::TMR_REGION_AR:
		case TMR_Region::TMR_REGION_AU:
		case TMR_Region::TMR_REGION_BD:
		case TMR_Region::TMR_REGION_EU:
		case TMR_Region::TMR_REGION_EU2:
		case TMR_Region::TMR_REGION_EU3:
		case TMR_Region::TMR_REGION_EU4:
		case TMR_Region::TMR_REGION_HK:
		case TMR_Region::TMR_REGION_ID:
		case TMR_Region::TMR_REGION_IN:
		case TMR_Region::TMR_REGION_IS:
		case TMR_Region::TMR_REGION_JP:
		case TMR_Region::TMR_REGION_JP2:
		case TMR_Region::TMR_REGION_JP3:
		case TMR_Region::TMR_REGION_KR:
		case TMR_Region::TMR_REGION_KR2:
		case TMR_Region::TMR_REGION_MO:
		case TMR_Region::TMR_REGION_MY:
		case TMR_Region::TMR_REGION_NA:
		case TMR_Region::TMR_REGION_NA2:
		case TMR_Region::TMR_REGION_NONE:
		case TMR_Region::TMR_REGION_NZ:
		case TMR_Region::TMR_REGION_OPEN:
		case TMR_Region::TMR_REGION_OPEN_EXTENDED:
		case TMR_Region::TMR_REGION_PH:
		case TMR_Region::TMR_REGION_PRC:
		case TMR_Region::TMR_REGION_PRC2:
		case TMR_Region::TMR_REGION_RU:
		case TMR_Region::TMR_REGION_SG:
		case TMR_Region::TMR_REGION_TH:
		case TMR_Region::TMR_REGION_TW:
		case TMR_Region::TMR_REGION_VN:
			break;
		}
	}
	static TMR_Region getRegionFromName(char const * name)
	{
		return TMR_Region::TMR_REGION_NONE;
	}
};

class ILog
{
public:
	enum MsgType
	{
		ERROR_MSG,
		INFORMATION_MSG,
		WARNING_MSG
	};
	virtual void writeMsg(MsgType type, char const * msg) = 0;
};

class ITagInfo
{
public:
	enum TagType
	{
		GEN2
	};
	virtual void * getTagData() = 0;
	virtual int getTagDataSize() = 0;

};

class TagInfo : protected ITagInfo
{
public:

};

struct NetworkBytecode
{
public:
	static int const MAX_PAYLOAD_SIZE = 112;
	uint32_t cmd;
	uint32_t payloadSize;
	long long tickTime;
};

struct GenericNetworkBytecode : public NetworkBytecode
{
public:
	char payload[MAX_PAYLOAD_SIZE];
};

class IClientToHostCommunication
{
	//cmds of [0,max) are valid, (min,0) are invalid
public:
	virtual bool connect(char const * host) = 0;
	virtual bool disconnect(char const * reason) = 0;
	virtual bool send(NetworkBytecode * what) = 0;
	virtual bool peek(int & cmd) = 0;
	virtual bool read(GenericNetworkBytecode * where) = 0;
	virtual bool flush() = 0;
};

class RPCCommands
{
public:
	static int const UNLOCK = 1;
	static int const LOCK = 2;
	static int const START = 3;
	static int const UPDATE = 4;
	static int const STOP = 5;
	static int const TAG_ARRIVE = 6;
	static int const TAG_LEAVE = 7;
	static int const OBJECT_PRESENT = 8;
	static int const TAG_PRESENT_TOO_LONG = 9;
	static int const GET_DEVICE_TICK_COUNT = 10;
	static int const RESET_DEVICE_TICK_COUNT = 11;
private:
	RPCCommands();
};

struct TagArriveParam : public NetworkBytecode
{
public:
	char epc[12];
	
	TagArriveParam()
	{
		cmd = RPCCommands::TAG_ARRIVE;
		payloadSize = sizeof(TagArriveParam) - sizeof(NetworkBytecode);
	}
};

struct TagLeaveParam : public TagArriveParam
{
public:
	TagLeaveParam()
	{
		cmd = RPCCommands::TAG_LEAVE;
	}
};


class WindowsClientToHostCommunication : public IClientToHostCommunication
{
	//Use winsocks
public:
};

class LinuxClientToHostCommunication : public IClientToHostCommunication
{
	//Use socket.h with linux (may be same as winsock lib)
public:
};



class RFIDAntenna
{
public:
	enum AntennaState
	{
		OFF,
		ON
	};

public:

	uint8_t antennaPort;
	AntennaState state;
	bool isPortOkay;						//Corresponds to TMR_PARAM_ANTENNA_CHECKPORT
	int32_t returnLoss = 0;
	int32_t readPower = -1;
	int32_t writePower = -1;
	bool isReadPwrDefault = false;
	bool isWritePwrDefault = false;

	RFIDAntenna()
	{

	}

	RFIDAntenna(TMR_Reader * rdr, uint8_t port)
	{
		reader = rdr;
		antennaPort = port;
	}

	bool disableAntenna();

	bool enableAntenna();

private:
	TMR_Reader * reader;
};

class RFIDReader
{
	const static int MAX_REGIONS = 32;
	const static int MAX_ANTENNAS = 16;
	const static int MAX_STRING_SIZE = 32;
public:
	struct AntennaSettings
	{
	public:
		int32_t readPower;
		int32_t writePower;
		uint8_t port;
		bool useToRead;
		bool useToWrite;
	};

	struct ReaderSettings
	{
	public:
		char const * uriConnectionString = nullptr;
		int32_t defaultReadPower = -1;
		int32_t defaultWritePower = -1;
		TMR_SR_PowerMode powerMode = TMR_SR_PowerMode::TMR_SR_POWER_MODE_INVALID;
		TMR_Region regionToUse = TMR_REGION_NONE;
		AntennaSettings antennasParams[MAX_ANTENNAS];
		int numOfAntennas = -1;
	};

	RFIDReader(ILog * logAssistant)
	{
		log = logAssistant;
	}

	virtual bool initialize(ReaderSettings * devSettings)
	{
		//Create the reader
		TMR_Status startupError = TMR_create(&reader,devSettings->uriConnectionString);
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
		
		//Initialize the antennas
		if (!initAntennas(devSettings))
			return false;

		return true;
	}

	bool setReadPower(int32_t pwr)
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
	
	bool setWritePower(int32_t pwr)
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
	bool readRemainingTags(TMR_TagReadData * tags, int & totalTags)
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

	bool readTags(TMR_TagReadData * tags, int timeoutMs, int & totalTags)
	{
		int tagLimit = totalTags;
		TMR_Status status = TMR_read(&reader, 100, &totalTags);
		if (status != TMR_SUCCESS)
			return false;
		for (int i = 0; i < totalTags && i < tagLimit; i++)
		{
			status = TMR_getNextTag(&reader, tags + i);
			if (status != TMR_SUCCESS)
			{
				totalTags = i;
				return false;
			}
		}
		return true;
	}

	uint32_t getDefaultReadPower()
	{
		return readPower;
	}
	
	uint32_t getDefaultWritePower()
	{
		return writePower;
	}

	bool setCommandTimeout(uint32_t timeoutMs);

	bool setTransportTimeout(uint32_t timeoutMs);

	int getTotalAntennas() const
	{
		return totalAntennas;
	}

	RFIDAntenna const * getAntenna(int index) const
	{
		if (index >= totalAntennas)
			return nullptr;
		return antennas + index;
	}

	bool isTagProtocolSupported(TMR_TagProtocol protocol)const;

	bool setCurrentRegion(TMR_Region region)
	{
		TMR_Status status = TMR_paramSet(&reader, TMR_PARAM_REGION_ID, &region);

		//Apply the region if the parameter was successfully set
		if (status == TMR_SUCCESS)
			currentRegion = region;
		return status == TMR_SUCCESS;
	}

	bool isRegionSupported(TMR_Region region)
	{
		//Select the maximum index to iterate, go through each region and determine if the specified region is in the list
		int maxIndex = regionList.len < regionList.max ? regionList.len : regionList.max;
		for (int i = 0; i < maxIndex; i++)
			if (supportedRegions[i] == region)
				return true;
		return false;
	}

	bool setBaudRate(uint32_t newBaud);

	bool setReaderPowerMode(TMR_SR_PowerMode mode);

	bool saveCurrentConfig();

	bool setGen2Password(uint32_t value);

	bool setGen2BlockWrite(bool enable, bool useFallback);

	bool setGen2BackscatterLinkFreq(uint32_t freqkHz);
	
	bool setGen2TagEncoding(TMR_GEN2_TagEncoding encoding);
	
	bool setGen2Session(TMR_GEN2_Session session);
	
	bool setGen2Target(TMR_GEN2_Target target);
	
	bool setGen2Tari(TMR_GEN2_Tari tari);
	
	bool setGen2WriteReplayTimeout(uint32_t timeoutMs);
	
	bool setGen2WriteEarly(bool writeEarly);

	bool setDevicePowerMode(TMR_SR_PowerMode mode)
	{
		TMR_Status status = TMR_paramSet(&reader, TMR_Param::TMR_PARAM_POWERMODE, &mode);
		if (status != TMR_SUCCESS)
			return false;
		powerMode = mode;
		return true;
	}

protected:

	//Initializes the version info contained by this object by getting the information from the rfid device
	//Override to allow more information to be obtained
	virtual bool initVersionInfo()
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
	virtual bool initRegionSettings(ReaderSettings *  devSettings)
	{
		//Get the current region
		currentRegion = TMR_REGION_NONE;
		TMR_Status startupError = TMR_paramGet(&reader, TMR_PARAM_REGION_ID, &currentRegion);
		if (startupError != TMR_SUCCESS)
			return false;

		//Get the list of supported regions
		regionList.list = supportedRegions;
		regionList.max = MAX_REGIONS;
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
	virtual bool initAntennas(ReaderSettings * devSettings)
	{
		//Get list of all antenna ports
		uint8_t antennaPorts[MAX_ANTENNAS];
		TMR_uint8List antennaList;
		antennaList.list = antennaPorts;
		antennaList.max = MAX_ANTENNAS;
		TMR_Status startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_ANTENNA_PORTLIST, &antennaList);
		if (startupError != TMR_SUCCESS)
			return false;
		totalAntennas = antennaList.len < antennaList.max ? antennaList.len : antennaList.max;

		//Get list of all connected antenna ports
		TMR_uint8List connectedAntennaList;
		uint8_t connectedAntennas[MAX_ANTENNAS];
		connectedAntennaList.list = connectedAntennas;
		connectedAntennaList.max = MAX_ANTENNAS;
		startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_ANTENNA_CONNECTEDPORTLIST, &connectedAntennaList);
		if (startupError != TMR_SUCCESS)
			return false;

		//Measure the return losses of each antenna
		TMR_PortValueList returnLossesList;
		TMR_PortValue portReturnLosses[MAX_ANTENNAS];
		returnLossesList.list = portReturnLosses;
		returnLossesList.max = MAX_ANTENNAS;
		startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_ANTENNA_RETURNLOSS, &returnLossesList);
		if (startupError != TMR_SUCCESS)
			return false;

		//Get the read power of each antenna
		TMR_PortValueList readPowerList;
		TMR_PortValue readPowers[MAX_ANTENNAS];
		readPowerList.list = readPowers;
		readPowerList.max = MAX_ANTENNAS;
		startupError = TMR_paramGet(&reader, TMR_Param::TMR_PARAM_RADIO_PORTREADPOWERLIST, &readPowerList);
		if (startupError != TMR_SUCCESS)
			return false;

		//Get the write power of each antenna
		TMR_PortValueList writePowerList;
		TMR_PortValue writePowers[MAX_ANTENNAS];
		writePowerList.list = writePowers;
		writePowerList.max = MAX_ANTENNAS;
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
	virtual bool initPowerInfo(ReaderSettings * settings)
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
		if (settings->defaultReadPower > 0 && !setReadPower(settings->defaultReadPower))
			return false;

		//Set the default write power
		if (settings->defaultWritePower > 0 && !setWritePower(settings->defaultWritePower))
			return  false;

		
		return true;
	}

private:
	mutable TMR_Reader reader;
	uint32_t commandTimeout;
	uint32_t transportTimeout;
	uint32_t baudRate;

	TMR_Region currentRegion;						//The region that is set by the reader
	TMR_RegionList regionList;						//Info about list of supported regions
	TMR_Region supportedRegions[MAX_STRING_SIZE];	//List of regions supported by the reader
	char serialNumber[MAX_STRING_SIZE];				//Serial Number of the reader
	char softwareVersion[MAX_STRING_SIZE];			//Software version of the reader
	char model[MAX_STRING_SIZE];					//Model of the reader
	char hardwareVersion[MAX_STRING_SIZE];			//Hardware version of the reader
	int totalAntennas;
	RFIDAntenna antennas[MAX_ANTENNAS];
	uint32_t readPower;
	uint32_t writePower;
	int16_t maxPower;
	int16_t minPower;
	ILog * log;
	TMR_SR_PowerMode powerMode;
};

class DefaultReaderDeviceSettings : public RFIDReader::ReaderSettings
{
public:
	DefaultReaderDeviceSettings()
	{
		regionToUse = TMR_REGION_NA;
		uriConnectionString = "tmr:///COM5";
		defaultReadPower = 3000;
		defaultWritePower = 500;
		powerMode = TMR_SR_PowerMode::TMR_SR_POWER_MODE_FULL;
	}
};

struct HostSettings
{
public:
	int tagRememberanceTime = 7000;  //How long to remember a tag for. If the tag arrive cmd occurs how long till a tag leave cmd  should occur
	int doorUnlockTime = 2500;

};

struct ClientSettings
{
public:
	char const * hostConnectionString;
	int networkTickRate;		//Corresponds to the read timeout. The host should receive messages at least this often. In ms
	
};

class DeviceSettings
{
public:
	RFIDReader::ReaderSettings * rdrSettings;
	ClientSettings * clientSettings;
};

struct DefaultClientSettings : public ClientSettings
{
public:
	DefaultClientSettings()
	{
		hostConnectionString = "127.0.0.1:30563";
		networkTickRate = 1000;
	}
};

class DefaultDeviceSettings : public DeviceSettings
{
private:
	DefaultReaderDeviceSettings defReaderSettings;
	DefaultClientSettings defClientSettings;
public:
	DefaultDeviceSettings()
	{
		rdrSettings = &defReaderSettings;
		clientSettings = &defClientSettings;
	}

};

IClientToHostCommunication * getCommunicationMethod();
int main()
{
	DefaultDeviceSettings settings;
	
	//Create reader and get it ready
	RFIDReader reader(nullptr);
	if (!reader.initialize(settings.rdrSettings))
		return -1;

	//Load host. Chat w/ host about getting the party started. Send reader init info
	IClientToHostCommunication * comm = getCommunicationMethod();
	if (!comm->connect(settings.clientSettings->hostConnectionString))
		return -1;
	uint64_t tick = 0;
	do
	{
		//Process all received rpcs
			//Interpret the command and act accordingly
		//Perform a read operation
		//Tell the host who was present during the read operations
	} while (true);


	TMR_TagReadData readTags[25];
	int totalTags = 25;
	do
	{
		if (!reader.readTags(readTags,1000, totalTags))
			return -1;
		for (int i = 0; i < totalTags && i < 25; i++)
		{
			TMR_TagData data = readTags[i].tag;
			std::cout << "Read tag: ";
			for (int j= 0; j < data.epcByteCount; j++)
				std::cout << std::hex << (int)data.epc[j];
			std::cout << '\n';
		}
	} while (true);
	
	return 0;
}
