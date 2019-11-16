#pragma once
#include <tm_reader.h>
#include "ReaderSettings.h"
#include "RFIDAntenna.h"
#include "ILog.h"
#include "Settings.h"
namespace RFIDDeviceController
{
	class RFIDReader
	{
	public:
		enum class ReaderErrors : int
		{
			NONE = 0,
			EPC_TOO_LONG,
			GET_NEXT_TAG,
		};
		ReaderErrors lastError = ReaderErrors::NONE;

		RFIDReader(ILog * logAssistant);
		virtual ~RFIDReader();
		virtual bool initialize(Settings::ReaderSettings * devSettings);

		bool setReadPower(int32_t pwr);

		bool setWritePower(int32_t pwr);

		//Total tags must be <= total tags in TMR_getNextTag
		bool readRemainingTags(TMR_TagReadData * tags, int & totalTags);

		bool readTags(char * epcBufferArray, int timeoutMs, int & totalTags);

		uint32_t getDefaultReadPower();

		uint32_t getDefaultWritePower();

		bool setCommandTimeout(uint32_t timeoutMs);

		bool setTransportTimeout(uint32_t timeoutMs);

		int getTotalAntennas() const;

		RFIDAntenna const * getAntenna(int index) const;

		bool isTagProtocolSupported(TMR_TagProtocol protocol)const;

		bool setCurrentRegion(TMR_Region region);

		bool isRegionSupported(TMR_Region region);

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

		bool setDevicePowerMode(TMR_SR_PowerMode mode);

		bool turnOnLed1();

		bool turnOnLed2();

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
		virtual bool initRegionSettings(Settings::ReaderSettings *  devSettings)
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
		virtual bool initAntennas(Settings::ReaderSettings * devSettings)
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
		virtual bool initPowerInfo(Settings::ReaderSettings * settings)
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

		virtual bool initOperationSettings(Settings::ReaderSettings * settings)
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

	private:
		mutable TMR_Reader reader;
		TMR_Region currentRegion;						//The region that is set by the reader
		TMR_RegionList regionList;						//Info about list of supported regions
		TMR_Region supportedRegions[Settings::MAX_STRING_SIZE];	//List of regions supported by the reader
		char serialNumber[Settings::MAX_STRING_SIZE];				//Serial Number of the reader
		char softwareVersion[Settings::MAX_STRING_SIZE];			//Software version of the reader
		char model[Settings::MAX_STRING_SIZE];					//Model of the reader
		char hardwareVersion[Settings::MAX_STRING_SIZE];			//Hardware version of the reader
		int32_t totalAntennas;
		RFIDAntenna antennas[Settings::MAX_ANTENNAS];
		uint32_t commandTimeout;
		uint32_t transportTimeout;
		uint32_t baudRate;
		uint32_t readPower;
		uint32_t writePower;
		int16_t maxPower;
		int16_t minPower;
		ILog * log;
		TMR_SR_PowerMode powerMode;
	};
}

