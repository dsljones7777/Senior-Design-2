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
	static std::string const & getRegionName(TMR_Region region);
	static TMR_Region getRegionFromName(std::string const & name);
};

class RFIDAntenna
{
private:
	TMR_Reader * reader;

public:
	enum AntennaState
	{
		OFF,
		ON
	};
	mutable uint8_t antennaPort;
	mutable bool isPortOkay; //Corresponds to TMR_PARAM_ANTENNA_CHECKPORT
	mutable uint32_t antennaReadPower;
	mutable uint32_t antennaWritePower;
	bool setAntennaReadPower(uint32_t pwr);
	bool setAntennaWritePower(uint32_t pwr);
	bool disableAntenna();
	bool enableAntenna();

};

class RFIDReader
{
private:
	mutable TMR_Reader reader;
	std::list<RFIDAntenna *> antennas;

public:
	mutable uint32_t commandTimeout;
	mutable uint32_t transportTimeout;
	mutable uint32_t baudRate;
	mutable TMR_Region currentRegion;
	const std::string uriConnectionString;
	const std::string softwareVersion;
	const std::string model;
	const std::string hardwareVersion;
	const std::string readerSerialNumber;

	RFIDReader(std::string const & uri);
	bool setCommandTimeout(uint32_t timeoutMs);
	bool setTransportTimeout(uint32_t timeoutMs);
	int getTotalAntennas() const;			
	RFIDAntenna * getAntenna(int index) const;
	int getReaderTemperature()const;	
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

};

int main()
{
	TMR_Reader reader, * pReader;
	TMR_Status readerStatus;
	TMR_Region readerRegion;
	pReader = &reader;
	readerStatus = TMR_create(pReader, "tmr:///COM5");
	if (readerStatus != TMR_SUCCESS)
	{
		std::cout << "Failed to create the reader\n";
		return -1;
	}
	readerStatus = TMR_connect(pReader);
	if (readerStatus != TMR_SUCCESS)
	{
		std::cout << "Failed to connect to reader\n";
		return -1;
	}
	readerStatus = TMR_paramGet(pReader, TMR_PARAM_REGION_ID, (void*)&readerRegion);
	if (readerStatus != TMR_SUCCESS)
	{
		std::cout << "Failed to get reader region\n";
		return -1;
		
	}
	std::cout << "Current Region: " << readerRegion << '\n';
	int8_t temp;
	readerStatus = TMR_paramGet(pReader, TMR_PARAM_RADIO_TEMPERATURE, (void*)&temp);
	if (readerStatus != TMR_SUCCESS)
	{
		std::cout << "Failed to get reader radio temperature\n";
		return -1;
	}
	std::cout << "Current Temperature: " << std::to_string(temp) << " *C\n";
	return 0;
}
