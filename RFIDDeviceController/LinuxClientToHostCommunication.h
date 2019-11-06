#pragma once
#include "IClientToHostCommunication.h"
namespace Identification
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


