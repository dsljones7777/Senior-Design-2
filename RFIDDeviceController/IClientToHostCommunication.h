#pragma once
#include "CommunicationObjects.h"
namespace Identification
{
	namespace Communication
	{
		class IClientToHostCommunication
		{
		public:
			virtual bool connect(char const * host) = 0;
			virtual bool disconnect(char const * reason) = 0;
			virtual bool send(NetworkBytecode * what) = 0;
			virtual bool peek(int & cmd) = 0;
			virtual bool read(GenericNetworkBytecode * where) = 0;
			virtual bool flush() = 0;
		protected:
			IClientToHostCommunication();
			virtual ~IClientToHostCommunication();
		};
	}
}


