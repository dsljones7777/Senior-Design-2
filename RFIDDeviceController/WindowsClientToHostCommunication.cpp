#include "pch.h"
#include "WindowsClientToHostCommunication.h"

#ifdef _WIN32
#include <WS2tcpip.h>
Identification::Communication::WindowsClientToHostCommunication::WindowsClientToHostCommunication()
{
}
Identification::Communication::WindowsClientToHostCommunication::~WindowsClientToHostCommunication()
{
}
bool Identification::Communication::WindowsClientToHostCommunication::connectTo(char const * host)
{
	addrinfo addrHint,* pAddr, *ptr;
	memset(&addrHint, 0, sizeof(addrinfo));
	addrHint.ai_family = AF_INET;
	addrHint.ai_socktype = SOCK_STREAM;
	addrHint.ai_protocol = IPPROTO_TCP;
	char const * portStr = strchr(host, (int)' ');

	//is port null or next character a null-terminator
	if (!portStr || !*(++portStr))
		return false;
	portStr++;
	if (!GetAddrInfoA(host, portStr, &addrHint, &pAddr))
		return false;
	for ( ptr = pAddr; ptr; ptr = ptr->ai_next)
	{
		clientSocket = socket(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol);
		if (clientSocket != INVALID_SOCKET)
			break;
	}
	if (clientSocket == INVALID_SOCKET)
	{
		freeaddrinfo(pAddr);
		return false;
	}
	if (!connect(clientSocket, ptr->ai_addr, (int)ptr->ai_addrlen))
	{
		closesocket(clientSocket);
		clientSocket = INVALID_SOCKET;
		freeaddrinfo(pAddr);
		return false;
	}
	return true;
}

bool Identification::Communication::WindowsClientToHostCommunication::disconnect(char const * reason)
{
	closesocket(clientSocket);
	return true;
}

bool Identification::Communication::WindowsClientToHostCommunication::write(NetworkBytecode * what)
{
	if (!send(clientSocket, (const char *)what, what->payloadSize + sizeof(NetworkBytecode), 0))
		return false;
	return true;
}

bool Identification::Communication::WindowsClientToHostCommunication::peek(int & totalBytes)
{
	if(!ioctlsocket(clientSocket,FIONREAD,(u_long *)&totalBytes))
		return false;
	return true;
}

bool Identification::Communication::WindowsClientToHostCommunication::read(GenericNetworkBytecode * where)
{
	int offset = 0, ttlBytesReceived = 0;
	while (ttlBytesReceived < sizeof(NetworkBytecode))
	{
		int bytesRead = recv(clientSocket, ((char*)where) + offset, sizeof(NetworkBytecode) - ttlBytesReceived, 0);
		if (!bytesRead)
			return false;
		offset += bytesRead;
		ttlBytesReceived += bytesRead;
	}
	if (where->payloadSize > NetworkBytecode::MAX_PAYLOAD_SIZE)
		return false;
	int ttlNeeded = sizeof(NetworkBytecode) + where->payloadSize;
	while (ttlBytesReceived < ttlNeeded)
	{
		int bytesRead = recv(clientSocket, ((char*)where) + offset, ttlNeeded  - ttlBytesReceived, 0);
		if (!bytesRead)
			return false;
		offset += bytesRead;
		ttlBytesReceived += bytesRead;
	}
	return where->isValid();
}

bool Identification::Communication::WindowsClientToHostCommunication::flush()
{
	return true;
}

bool Identification::Communication::WindowsClientToHostCommunication::init()
{
	
	if(!WSAStartup(MAKEWORD(2, 2), &startupData))
		return false;
	return true;
}
#endif