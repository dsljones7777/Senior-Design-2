#include "pch.h"
#include "CommunicationObjects.h"

bool Identification::Communication::NetworkBytecode::isValid()
{
	switch (cmd)
	{
	case (uint32_t)RPCCommands::UNLOCK:
		return payloadSize == sizeof(UnlockNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::START:
		return payloadSize == sizeof(StartNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::LOCK:
		return payloadSize == sizeof(LockNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::UPDATE:
		return payloadSize == sizeof(UpdateNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::STOP:
		return payloadSize == sizeof(StopNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::TAG_ARRIVE:
	case (uint32_t)RPCCommands::TAG_LEAVE:
	case (uint32_t)RPCCommands::OBJECT_PRESENT:
	case (uint32_t)RPCCommands::TAG_PRESENT_TOO_LONG:
	case (uint32_t)RPCCommands::GET_DEVICE_TICK_COUNT:
		return false;
	case (uint32_t)RPCCommands::RESET_DEVICE_TICK_COUNT:
		return payloadSize == sizeof(ResetTickCountNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::WRITE_TAG:
		return payloadSize == sizeof(WriteTagNetParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::ALIVE:
		return false;
	case (uint32_t)RPCCommands::PING:
		return payloadSize == sizeof(PingNetParam) - sizeof(NetworkBytecode);
	}
}
