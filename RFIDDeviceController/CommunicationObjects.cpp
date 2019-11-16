#include "pch.h"
#include "CommunicationObjects.h"

bool RFIDDeviceController::Communication::NetworkBytecode::isValid()
{
	switch (cmd)
	{
	case (uint32_t)CommandCodes::UNLOCK:
		return payloadSize == sizeof(UnlockNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::START:
		return payloadSize == sizeof(StartNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::LOCK:
		return payloadSize == sizeof(LockNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::UPDATE:
		return payloadSize == sizeof(UpdateNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::STOP:
		return payloadSize == sizeof(StopNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::TAG_ARRIVE:
	case (uint32_t)CommandCodes::TAG_LEAVE:
	case (uint32_t)CommandCodes::OBJECT_PRESENT:
	case (uint32_t)CommandCodes::TAG_PRESENT_TOO_LONG:
	case (uint32_t)CommandCodes::GET_DEVICE_TICK_COUNT:
		return false;
	case (uint32_t)CommandCodes::RESET_DEVICE_TICK_COUNT:
		return payloadSize == sizeof(ResetTickCountNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::WRITE_TAG:
		return payloadSize == sizeof(WriteTagNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::ALIVE:
		return false;
	case (uint32_t)CommandCodes::DEVICE_ERROR:
		return payloadSize == sizeof(DeviceErrorNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::PING:
		return payloadSize == sizeof(PingNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::CONFIRMATION_SYNC_TICK_COUNT:
		return payloadSize == sizeof(ConfirmationNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)CommandCodes::REBOOT_READER:
		return payloadSize == sizeof(NetworkBytecode);
	}
	return false;
}
