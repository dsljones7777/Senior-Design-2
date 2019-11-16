#include "pch.h"
#include "ReaderSettings.h"

#ifdef _WIN32
#include <Windows.h>
#include <SetupAPI.h>
static const char * EXPECTED_HW_ID = "vid_2008&pid_1004";
#endif

using namespace RFIDDeviceController::Settings;
ReaderSettings::ReaderSettings()
{
	regionToUse = TMR_REGION_NA;
	uriConnectionString = "tmr::///COM1";
	defaultReadPower = 3000;
	defaultWritePower = 250;
	defaultBaudRate = 57600;
	defaultCommandTimeout = 0;
	defaultTransportTimeout = 0;
	powerMode = TMR_SR_PowerMode::TMR_SR_POWER_MODE_FULL;

#ifdef _WIN32
	if (!initWindows())
		uriConnectionString = nullptr;
#endif
}
#ifdef _WIN32
bool RFIDDeviceController::Settings::ReaderSettings::initWindows()
{
	//Override the URI ConnectionString
	const char uriCT[] = "tmr:///";

	//Find usb device for serial reader by pci hw id
	HDEVINFO deviceInfoHdl;
	SP_DEVINFO_DATA deviceInfo;
	DEVPROPTYPE propType;
	BYTE buffer[512];
	DWORD success = FALSE;
	deviceInfoHdl = SetupDiGetClassDevsA(NULL, "USB", NULL, DIGCF_ALLCLASSES | DIGCF_PRESENT);
	if (deviceInfoHdl == INVALID_HANDLE_VALUE)
		return false;

	//Clear to 0 (forget memset)
	memset((void*)&deviceInfo, 0, sizeof(SP_DEVINFO_DATA));
	deviceInfo.cbSize = sizeof(SP_DEVINFO_DATA);
	for (DWORD index = 0; true; index++)
	{
		BOOL failed = SetupDiEnumDeviceInfo(deviceInfoHdl, index, &deviceInfo);
		if (failed == FALSE)
		{
			DWORD err = GetLastError();
			return false;
		}
		DWORD size = 0, portSize = 16 - sizeof(uriCT);
		if (!SetupDiGetDeviceRegistryPropertyA(deviceInfoHdl, &deviceInfo, SPDRP_HARDWAREID, &propType, buffer, sizeof(buffer), &size))
			continue;
		DWORD type = 0;
		HKEY key = SetupDiOpenDevRegKey(deviceInfoHdl, &deviceInfo, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
		if (key == INVALID_HANDLE_VALUE)
			continue;

		size = 7;
		success = RegQueryValueExA(key, "PortName", NULL, &type, (LPBYTE)(uriBuffer + sizeof(uriCT) - 1), &portSize);
		RegCloseKey(key);
		if (success != ERROR_SUCCESS)
			continue;
		break;
	}
	SetupDiDestroyDeviceInfoList(deviceInfoHdl);
	if (success != ERROR_SUCCESS)
		return false;
	uriConnectionString = uriBuffer;
	return true;
}
#endif

ReaderSettings::~ReaderSettings()
{
}
