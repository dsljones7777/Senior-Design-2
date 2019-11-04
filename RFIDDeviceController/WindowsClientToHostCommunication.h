#pragma once
#include "IClientToHostCommunication.h"
namespace Identification
{
	namespace Communication
	{
		class WindowsClientToHostCommunication : public IClientToHostCommunication
		{
		public:
			WindowsClientToHostCommunication();
			virtual ~WindowsClientToHostCommunication();
		};
	}
}


