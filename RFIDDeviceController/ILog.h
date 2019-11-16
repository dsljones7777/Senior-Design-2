#pragma once
namespace RFIDDeviceController
{
	class ILog
	{
	public:
		enum MsgType
		{
			ERROR_MSG,
			INFORMATION_MSG,
			WARNING_MSG
		};
		virtual void writeMsg(MsgType type, char const * msg) = 0;
	};
}


