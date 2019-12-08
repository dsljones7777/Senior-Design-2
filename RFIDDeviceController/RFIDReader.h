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
		char serialNumber[Settings::MAX_STRING_SIZE];				//Serial Number of the reader

		enum class ReaderErrors : int
		{
			NONE = 0,
			EPC_TOO_LONG,
			GET_NEXT_TAG,
		};
		ReaderErrors lastError = ReaderErrors::NONE;
		bool initialized = false;

		RFIDReader(ILog * logAssistant);
		virtual ~RFIDReader();
		virtual bool initialize(Settings::ReaderSettings * devSettings);

		bool setReadPower(int32_t pwr);

		bool setWritePower(int32_t pwr);

		//Total tags must be <= total tags in TMR_getNextTag
		bool readRemainingTags(TMR_TagReadData * tags, int & totalTags);

		bool readTags(char * epcBufferArray, int timeoutMs, int & totalTags);

		void shutdown()
		{
			TMR_destroy(&reader);
		}

		void writeTag(char  const * epcBufferArray, int readTimeout, int writeTimeout)
		{
			TMR_TagData data;
			int const* pSrc = (int const*)epcBufferArray;
			int * pDest = (int*)data.epc;
			for (int i = 0; i < 3; i++, pSrc++, pDest++)
				*pDest = *pSrc;
			data.epcByteCount = 12;
			data.protocol = TMR_TagProtocol::TMR_TAG_PROTOCOL_GEN2;
			data.crc = 0;
			TMR_TagOp_GEN2_WriteTag op;
			op.epcptr = &data;
			
			if (TMR_writeTag(&reader, nullptr, &data) != TMR_SUCCESS)
				return;
			op.epcptr = nullptr;
			return;
		}

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

		bool setLedState(int led, bool high);

		bool turnOnLed2();

	protected:

		//Initializes the version info contained by this object by getting the information from the rfid device
		//Override to allow more information to be obtained
		virtual bool initVersionInfo();

		//Initializes the region by getting the supported regions and trying to set the reader to the specified region if supported
		//Override to change the way regions are initialized
		virtual bool initRegionSettings(Settings::ReaderSettings *  devSettings);

		//Initializes the antennas and their respective states
		virtual bool initAntennas(Settings::ReaderSettings * devSettings);

		//Retreives all power information for the device and sets the desired power 
		virtual bool initPowerInfo(Settings::ReaderSettings * settings);

		virtual bool initOperationSettings(Settings::ReaderSettings * settings);

		virtual bool initGPIO(Settings::ReaderSettings * settings);

	private:
		mutable TMR_Reader reader;
		TMR_Region currentRegion;						//The region that is set by the reader
		TMR_RegionList regionList;						//Info about list of supported regions
		TMR_Region supportedRegions[Settings::MAX_STRING_SIZE];	//List of regions supported by the reader
		
		char softwareVersion[Settings::MAX_STRING_SIZE];			//Software version of the reader
		char model[Settings::MAX_STRING_SIZE];						//Model of the reader
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
		int led1GPIO;
		int led2GPIO;
	};
}

