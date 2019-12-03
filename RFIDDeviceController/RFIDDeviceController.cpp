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
	char const * serial = nullptr;

	//Check args
	if (argc <= 1)
		return -1;

	//Check to see if simulated or real device controller
	int ipArgIndex = 1;
	if (argc == 3)
		controller = &deviceController;
	else if (argc == 4 && !strcmp(args[3], "-s"))
	{
		controller = &simulatedController;
		((SimulatedDeviceController *)controller)->setupDeviceSerial(nullptr);
	}
	else if (argc == 6 && !strcmp(args[3], "-s") && !strcmp(args[4], "-serial"))
	{
		controller = &simulatedController;
		serial = args[5];
		((SimulatedDeviceController *)controller)->setupDeviceSerial(serial);
	}
	else
		return -1;

	//Setup the ip and port string to the host
	char const * ip = args[ipArgIndex];
	char const * port = args[ipArgIndex + 1];
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
		if (loopResult == 1)			//Reboot to virtual ?
			currentArgc = argc;				//Expand args for virtual mode
		else if (loopResult == 2)		//Reboot to real mode?
			currentArgc = 3;				//Cut off args for virtual mode
	} while (loopResult > 0);
	return loopResult;
}
