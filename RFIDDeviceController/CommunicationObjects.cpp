#include "pch.h"
#include "CommunicationObjects.h"

bool Identification::Communication::NetworkBytecode::isValid()
{
	switch (cmd)
	{
	case (uint32_t)RPCCommands::UNLOCK:
		return payloadSize == sizeof(UnlockParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::START:
		return payloadSize == sizeof(StartParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::LOCK:
		return payloadSize == sizeof(LockParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::UPDATE:
		return payloadSize == sizeof(UpdateParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::STOP:
		return payloadSize == sizeof(StopParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::TAG_ARRIVE:
	case (uint32_t)RPCCommands::TAG_LEAVE:
	case (uint32_t)RPCCommands::OBJECT_PRESENT:
	case (uint32_t)RPCCommands::TAG_PRESENT_TOO_LONG:
	case (uint32_t)RPCCommands::GET_DEVICE_TICK_COUNT:
		return false;
	case (uint32_t)RPCCommands::RESET_DEVICE_TICK_COUNT:
		return payloadSize == sizeof(ResetTickCountParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::WRITE_TAG:
		return payloadSize == sizeof(WriteTagParam) - sizeof(NetworkBytecode);
	case (uint32_t)RPCCommands::ALIVE:
		return false;
	case (uint32_t)RPCCommands::PING:
		return payloadSize == sizeof(PingParam) - sizeof(NetworkBytecode);
	}
}
