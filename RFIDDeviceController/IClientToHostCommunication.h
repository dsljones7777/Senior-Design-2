#pragma once
#include "CommunicationObjects.h"
namespace RFIDDeviceController
{
	namespace Communication
	{
		class IClientToHostCommunication
		{
		public:

			virtual bool init() = 0;
			virtual bool connectTo(char const * host) = 0;
			virtual bool disconnect(char const * reason) = 0;
			virtual bool write(NetworkBytecode * what) = 0;
			virtual bool peek(int & bytesAvailable) = 0;
			virtual bool read(GenericNetworkBytecode * where) = 0;
			virtual bool flush() = 0;
		protected:
			IClientToHostCommunication();
			virtual ~IClientToHostCommunication();
		};
		IClientToHostCommunication * getCommunicationObject();

	}
}


