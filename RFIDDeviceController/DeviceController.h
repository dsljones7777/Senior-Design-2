#pragma once

#include "RFIDReader.h"
#include "DeviceSettings.h"
#include "IClientToHostCommunication.h"
namespace RFIDDeviceController
{

	class DeviceController
	{
	public:
		DeviceController();
		virtual ~DeviceController();

		//Estimated tags per read (per 250ms)
		static const int TAG_BUFFER_SIZE = 5;											
		static const int REMEMBERANCE_TAG_BUFFER_SIZE = 10 * 4 * TAG_BUFFER_SIZE;		//10 seconds* 4 times a second (250ms reader tick rate) * Estimated tags per read (per read tick rate)

		//Runs until device is to shutdown
		int run();

	protected:
		static const int ERROR_LED_NUMBER = 1;
		static const int PING_LED_NUMBER = 2;

#pragma region HardwareControl


		void turnOnLed(int ledNumber);
		void turnOffLed(int ledNumber);
		bool isLedOn(int ledNumber);
		bool isLedOff(int ledNumber);
		void unlockDoor();
		void lockDoor();

		//Guarantees the reader starts and if it fails it contacts the server and handles. Return state is reader being able to read or the 
		void startReader();

		
		RFIDReader reader = RFIDReader(nullptr);
#pragma endregion

#pragma region NetworkControl

		bool startConnection(bool dueToFault);

		void connectToCommandCenter(bool onFault);

		//Send start command
		void sendStartToServer();

		//Tell server tag has arrived (make sure was not present in memory)
		void sendTagArriveToServer(char const * epc);

		//Send alive packet to server
		void tellServerAlive();

		//Tell server tag has left (make sure tag left for at least leave amount of time)
		void sendTagLeaveToServer(char const * epc);

		//Tell server the specified tag was detected for way too long
		void sendTagPresentTooLongToServer(char const * epc);

		//Tell server about a device error. If true is returned execution can be continued otherwise abort operation
		bool sendDeviceError(int errorCode);

		//Sends prepared packet(buffer) to server and waits for the server to acknowledge 
		void sendAndAssure();

		void sendWithoutAssurance();

		Communication::GenericNetworkBytecode buffer;

		Communication::IClientToHostCommunication * comm = nullptr;

#pragma endregion
		
#pragma region DeviceProgram

		/*
			Checks for a specified or any command on network and executes it
				-Set expectedCommand to a value to accept that command
			After call:
				-network is ok, client is connected to server, the socket has been polled
				-if a command was expected it has been received and guaranteed to be executed
				-if a command packet was present from server it was executed
					*unless it was received before an expected command packet (Exec not guaranteed)
		*/
		void checkAndExecuteCommand(int expectedCommand = 0);

		//Call check and execute instead for command to be received from server.
		//Executes command only if expected command is non-zero or command matches. 
		bool executeCommand(int expectedCommand);

		//Handles response from server for device error. 
		bool handleDeviceError();

		//Resets how long till Alive command needs to be sent
		void resetTicksTillDead();
		
		//Reading logic. Has the reader read some tags and determines if they should be reported to 
		//the server as arriving, leaving
		void updateTagsWithServer();


		

		//Array for easier offsetting of epc buffer when reading
		struct EPC
		{
		public:
			char epcData[12];
		};

		//Mem struct for tag memory.
		struct EPCRememberance
		{
			uint64_t epcFront;	//First 8 bytes of epc
			uint32_t epcEnd;	//First 4 bytes of epc
			uint32_t stayTime;
			uint32_t leaveTime;
			bool valid;
			bool foundDuringCurrentRead;
			bool toldServerTagPresentTooLong;
			bool sentServerTagPresent;
			EPCRememberance()
			{
				valid = false;
			}
			EPCRememberance(char const * epcData)
			{
				stayTime = leaveTime = 0;
				epcFront = *(uint64_t const *)epcData;
				epcEnd = *((uint32_t const *)epcData + 2);
				valid = true;
				foundDuringCurrentRead = true;
				toldServerTagPresentTooLong = false;
				sentServerTagPresent = false;
			};
		};

		//Buffer for reading and writing tags
		EPC epcBuffer[TAG_BUFFER_SIZE];

		//Buffer for tag memory of the device
		EPCRememberance epcMemoryBuffer[REMEMBERANCE_TAG_BUFFER_SIZE];
		Settings::DeviceSettings settings;
		bool exitProgram = false;

		//Tick and statistics
		uint64_t currentTick = 0;
		int64_t ticksTillDead = 1;
		uint64_t readTagCount = 0;
		uint64_t sentTagCount = 0;
#pragma endregion
	};

}

