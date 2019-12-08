#pragma once
#include "IClientToHostCommunication.h"
namespace RFIDDeviceController
{
	namespace Communication
	{
		class LinuxClientToHostCommunication : public IClientToHostCommunication
		{
		public:
			LinuxClientToHostCommunication();
			virtual ~LinuxClientToHostCommunication();
		};
	}
}


