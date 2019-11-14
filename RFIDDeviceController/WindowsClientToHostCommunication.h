#pragma once
#include "IClientToHostCommunication.h"

#ifdef _WIN32

#include <WinSock2.h>
namespace Identification
{
	namespace Communication
	{
		class WindowsClientToHostCommunication : public IClientToHostCommunication
		{
		private:
			SOCKET clientSocket = INVALID_SOCKET;
			WSADATA startupData;
			char buffer[NetworkBytecode::MAX_PAYLOAD_SIZE + sizeof(NetworkBytecode)];
		public:
			
			WindowsClientToHostCommunication();
			virtual ~WindowsClientToHostCommunication();
			
			// Inherited via IClientToHostCommunication
			virtual bool connectTo(char const * host) override;
			virtual bool disconnect(char const * reason) override;
			virtual bool write(NetworkBytecode * what) override;
			virtual bool peek(int & cmd) override;
			virtual bool read(GenericNetworkBytecode * where) override;
			virtual bool flush() override;

			// Inherited via IClientToHostCommunication
			virtual bool init() override;
		};
	}
}
#endif