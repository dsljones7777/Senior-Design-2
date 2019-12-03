// RFIDDeviceController.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include "pch.h"
#include "DeviceController.h"
using namespace RFIDDeviceController;
SimulatedDeviceController simulatedController;
DeviceController deviceController;
int programLoop(int argc, char const * args[])
{
	//Create the device controllers (virtual and actual) - no init
	simulatedController = SimulatedDeviceController();
	deviceController = DeviceController();
	DeviceController * controller;

	//Check args
	if (argc <= 1)
		return -1;

	//Check to see if simulated or real device controller
	if (argc == 3)										//ip port	: real device
		controller = &deviceController;
	else if (argc == 4 && !strcmp(args[3], "-s"))		//ip port -s : simulated device, random serial
	{
		controller = &simulatedController;
		controller->setupDeviceSerial(nullptr);
	}
	else if (argc == 5 && !strcmp(args[3],"-serial"))	//ip port -serial 'serial number' : real device specified serial
	{
		controller = &deviceController;
		controller->setupDeviceSerial(args[4]);
	}
	else if (argc == 6 && !strcmp(args[3], "-serial") && !strcmp(args[5], "-s")) //ip port -serial 'serial number' -s : simulated device specified serial
	{
		controller = &simulatedController;
		((SimulatedDeviceController *)controller)->setupDeviceSerial(args[4]);
	}
	else
		return -1;

	//Setup the ip and port string to the host
	char const * ip = args[1];
	char const * port = args[2];
	char buffer[256];
	strcpy_s(buffer, 256, ip);
	size_t len = strnlen_s(buffer, 256);
	buffer[len] = ' ';
	strcpy_s(buffer + len + 1, 256 - len - 1, port);
	controller->settings.clientSettings->hostConnectionString = buffer;

	//Init and run the device controller program
	return controller->run();

}
int main(int argc,char const * args[])
{
	int loopResult;
	int currentArgc = argc;
	do
	{
		loopResult = programLoop(currentArgc, args);
#ifdef _WIN32
		system("cls");
#endif
		if (loopResult == 1)			//Reboot to virtual ?
			currentArgc = argc;				//Expand args for virtual mode
		else if (loopResult == 2)		//Reboot to real mode?
			currentArgc = argc - 1;			//Cut off args for virtual mode
	} while (loopResult > 0);
	return loopResult;
}
