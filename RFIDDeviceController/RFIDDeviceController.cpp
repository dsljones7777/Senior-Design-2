// RFIDDeviceController.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include "pch.h"
#include "DeviceController.h"
using namespace RFIDDeviceController;
int main()
{
	DeviceController handler;
	int returnval = handler.run();
	return returnval;
}
