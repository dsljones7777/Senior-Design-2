using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace RFIDCommandCenter
{
    class RFIDDeviceClient : Client
    {
        const int NETWORK_TIMEOUT =
#if TEST
            2000;           //2 seconds
#elif DEBUG
           15 * 60 * 1000;   //15 minutes
#else
            2000;           //2 seconds
#endif



        long tickRateOffset;
        internal volatile bool exit;
        internal volatile bool pauseExecution;
        internal volatile int deviceError;

        bool isDeviceWaiting;
        
        public enum CommandCodes
        {
            NONE = 0,
            UNLOCK = 1,
            START = 2,
            LOCK = 3,
            UPDATE = 4,
            STOP = 5,
            TAG_ARRIVE = 6,
            TAG_LEAVE = 7,
            OBJECT_PRESENT = 8,
            TAG_PRESENT_TOO_LONG = 9,
            GET_DEVICE_TICK_COUNT = 10,
            RESET_DEVICE_TICK_COUNT = 11,
            WRITE_TAG = 12,
            ALIVE = 13,
            PING = 14,
            DEVICE_ERROR = 15,
            CONFIRMATION_SYNC_TICK_COUNT = 16
        }

        enum ErrorCodes
        {
            NONE = 0,
			DEVICE_FAILED_TO_READ,
			TAG_TOO_LONG,
			DEVICE_FAILED_TO_CONNECT,
			DEVICE_FAILED_TO_START,
			TAG_MEMORY_BUFFER_FULL
        };

        enum DeviceErrorCodes
        {
            NONE = 0,
            DEVICE_FAILED_TO_READ = 1,
            TAG_TOO_LONG = 2,
        };

        public RFIDDeviceClient(Socket who, NetworkCommunication comObj) : base(who, comObj)
        {
            tickRateOffset = DateTime.Now.Ticks;
        }
        
        public override void serverThreadRoutine(Object state)
        {
            //Create packet to tell client to start
            NetworkCode bufferPacket = new NetworkCode();
            bufferPacket.command = (int)CommandCodes.START;
#if DEBUG
            reportCommandInfo(bufferPacket);
#endif
            provideResponse(bufferPacket);

            //program loop, run until we are told to exit
            while (!exit)
            {
                try
                {
                    //get a packet from the client. An alive packet should come
                    if (!receivePacket(bufferPacket, NETWORK_TIMEOUT * 1000))
                        if (!isDeviceWaiting)
                            reportError("Device Timeout: Alive packet not sent");
                }
                catch (Exception e)
                {
                    reportError(e.Message);
                    continue;
                }

#if DEBUG
                reportCommandInfo(bufferPacket);
#endif
                executePacketRequest(bufferPacket);
                provideResponse(bufferPacket);
            }
        }


        private void runDeviceProgram()
        {
           
        }
        
        private void executePacketRequest(NetworkCode cmdPacket)
        {
            switch (cmdPacket.command)
            {
                case (int)CommandCodes.TAG_ARRIVE:
                    //Write to db
                    break;
                case (int)CommandCodes.TAG_LEAVE:
                    //Write to db
                    break;
                case (int)CommandCodes.ALIVE:
                    //Write to db
                    break;
                case (int)CommandCodes.TAG_PRESENT_TOO_LONG:
                    //Write to db
                    break;
                case (int)CommandCodes.DEVICE_ERROR:
                    handleDeviceError(cmdPacket);
                    break;
            }
        }

        private void provideResponse(NetworkCode cmdPacket)
        {
            switch (cmdPacket.command)
            {
                case (int)CommandCodes.TAG_ARRIVE:
                    //Write to db
                    break;
                case (int)CommandCodes.TAG_LEAVE:
                    //Write to db
                    break;
                case (int)CommandCodes.ALIVE:
                    //Write to db
                    break;
                case (int)CommandCodes.TAG_PRESENT_TOO_LONG:
                    //Write to db
                    break;
                case (int)CommandCodes.START:
                    cmdPacket.command = (int)CommandCodes.START;
                    cmdPacket.tickTime = (ulong)tickRateOffset;
                    cmdPacket.payloadSize = 0;
                    while(!exit)
                        try
                        {
                            if (sendPacket(cmdPacket, NETWORK_TIMEOUT  * 1000))
                                break;
                            reportError("Could not send packet to tell device client to start");
                        }
                        catch(Exception e)
                        {
                            reportError(e.Message);
                        }
                    return;
                case (int)CommandCodes.DEVICE_ERROR:
                    break;
                default:
                    break;
            }
            if (cmdPacket.command == (int)CommandCodes.NONE)
                return;
            cmdPacket.command = (int)CommandCodes.CONFIRMATION_SYNC_TICK_COUNT;
            cmdPacket.tickTime = (ulong)(DateTime.Now.Ticks - tickRateOffset) / TimeSpan.TicksPerMillisecond;
            Array.Copy(BitConverter.GetBytes((int)CommandCodes.NONE),cmdPacket.payload,4);
            cmdPacket.payloadSize = 4;
            while (!exit)
            {
                try
                {
                    while (!exit)
                    {
                        if (sendPacket(cmdPacket, NETWORK_TIMEOUT * 1000))
                            return;
                        reportError("Failed to provide response to client");
                    }
                }
                catch (Exception e)
                {
                    reportError(e.Message);
                }
            }
        }

        private void handleDeviceError(NetworkCode errorCode)
        {
            string msg;
            switch(BitConverter.ToInt32(errorCode.payload,0))
            {
                case (int)ErrorCodes.DEVICE_FAILED_TO_CONNECT:
                    msg = "Device is not connected to the PC";
                    break;
                case (int)ErrorCodes.DEVICE_FAILED_TO_READ:
                    msg = "Device failed to complete a read operation";
                    break;
                case (int)ErrorCodes.DEVICE_FAILED_TO_START:
                    msg = "Device failed to start rfid reader module";
                    break;
                case (int)ErrorCodes.TAG_MEMORY_BUFFER_FULL:
                    msg = "Device out of tag memory, buffer full";
                    break;
                default:
                    msg = "Unknown device error";
                    break;
            }
            DialogResult result = MessageBox.Show(null, "An error occurred with the device:\nError Code: "
                               + BitConverter.ToInt32(errorCode.payload, 0) +
                               "\nReason: " + msg +
                               "\n Would you like to continue?",
                               "Device Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (result != DialogResult.Retry)
                exit = true;
        }
        private void reportCommandInfo(NetworkCode cmdPacket,bool showAlive = false)
        {
            switch (cmdPacket.command)
            {
                case (int)CommandCodes.START:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Device Connecting @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    Console.Write("EPC: ");
                    for (int i = 0; i < 12; i++)
                        Console.Write(String.Format("{0:x2}", cmdPacket.payload[i]));
                    Console.WriteLine();
                    break;
                case (int)CommandCodes.TAG_ARRIVE:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Tag Arrive @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    Console.Write("EPC: ");
                    for (int i = 0; i < 12; i++)
                        Console.Write(String.Format("{0:x2}", cmdPacket.payload[i]));
                    Console.WriteLine();
                    break;
                case (int)CommandCodes.TAG_LEAVE:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Tag Leave @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    Console.Write("EPC: ");
                    for (int i = 0; i < 12; i++)
                        Console.Write(String.Format("{0:x2}", cmdPacket.payload[i]));
                    Console.WriteLine();
                    break;
                case (int)CommandCodes.ALIVE:
                    if (!showAlive)
                        return;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Reader Alive Packet: " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    break;
                case (int)CommandCodes.TAG_PRESENT_TOO_LONG:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Tag Present Too Long @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    Console.Write("EPC: ");
                    for (int i = 0; i < 12; i++)
                        Console.Write(String.Format("{0:x2}", cmdPacket.payload[i]));
                    Console.WriteLine();
                    break;
                case (int)CommandCodes.DEVICE_ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Device Error @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    Console.WriteLine("ErrCode: " + BitConverter.ToInt32(cmdPacket.payload, 0).ToString());
                    break;

            }
            long serverTicks = (DateTime.Now.Ticks - tickRateOffset) / TimeSpan.TicksPerMillisecond;
            Console.WriteLine("Server Tick = " + serverTicks + "ms");
            Console.WriteLine("Tick Diff(Server) = " + (serverTicks - (long)cmdPacket.tickTime).ToString() + "ms");
            Console.WriteLine();
        }

        private void reportError(string error)
        {
            pauseExecution = true;
#if DEBUG
            DialogResult result = MessageBox.Show(null, "An error occurred with the server:\n"
                                + error +
                                "\n Would you like to continue?",
                                "Server Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (result == DialogResult.Retry)
                pauseExecution = false;
            else
                Application.Exit();
#endif
            //TODO: Log or determine how to handle

            while (pauseExecution)
                Thread.Yield();
        }
    }
}
