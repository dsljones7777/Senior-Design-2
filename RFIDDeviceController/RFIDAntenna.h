#pragma once
#include <tm_reader.h>
namespace RFIDDeviceController
{
	class RFIDAntenna
	{
	public:
		enum AntennaState
		{
			OFF,
			ON
		};

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

}


