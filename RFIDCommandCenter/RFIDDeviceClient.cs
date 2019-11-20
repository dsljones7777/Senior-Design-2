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
        internal volatile bool continueAfterDeviceError;
        internal volatile string serverErrorMessage;
        
        
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
            CONFIRMATION_SYNC_TICK_COUNT = 16,
            REBOOT_READER = 17,
            WAIT = 18					//Wait for a response, pump alive packets
        }

        public enum ErrorCodes
        {
            NONE = 0,
			DEVICE_FAILED_TO_READ,
			DEVICE_FAILED_TO_CONNECT,
			DEVICE_FAILED_TO_START,
			TAG_MEMORY_BUFFER_FULL,

            TAG_TOO_LONG
        };


        public RFIDDeviceClient(Socket who, NetworkCommunication comObj) : base(who, comObj)
        {
            tickRateOffset = DateTime.Now.Ticks;
        }
        
        public override void serverThreadRoutine(Object state)
        {
            //Create packet to tell client to start
            NetworkCode bufferPacket = new NetworkCode();
            sendCommand(bufferPacket, CommandCodes.START, NETWORK_TIMEOUT * 1000, true, true);
#if DEBUG
            reportCommandInfo(bufferPacket);
#endif

            //program loop, run until we are told to exit
            while (!exit)
            {
                try
                {
                    //get a packet from the client. An alive packet should come
                    if (!receivePacket(bufferPacket, NETWORK_TIMEOUT * 1000))
                           reportError("Device Timeout: Alive packet not sent");
#if DEBUG
                    reportCommandInfo(bufferPacket);
#endif
                    executePacketRequest(bufferPacket);
                    provideResponse(bufferPacket);
                }
                catch (Exception e)
                {
                    reportError(e.Message);
                    continue;
                }


            }
        }

        private void executePacketRequest(NetworkCode cmdPacket)
        {
            using (var context = new DataContext())
            {
                switch (cmdPacket.command)
                {
                    case (int)CommandCodes.TAG_ARRIVE:
                        var tagArriveNum = (byte[])cmdPacket.payload;
                        if (tagArriveNum == null)
                            throw new ApplicationException("Invalid payload data");
                        var tagArrive = new Logic.TagArrive();
                        tagArrive.Execute(tagArriveNum, context);
                        //If allowed location exits then
                        cmdPacket.command = (int)CommandCodes.UNLOCK;
                        //else
                            //break;
                        break;
                    case (int)CommandCodes.TAG_LEAVE:
                        var tagLeavingNum = (byte[])cmdPacket.payload;
                        if (tagLeavingNum == null)
                            throw new ApplicationException("Invalid payload data");
                        var tagLeave = new Logic.TagLeave();
                        tagLeave.Execute(tagLeavingNum, context);
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
                
        }

        private void sendCommand(NetworkCode cmdPacket, CommandCodes cmd, int timeoutUs, bool assured, bool shouldReportError,params object[] cmdParams)
        {
            //Set the cmd header
            cmdPacket.command = (int)cmd;
            cmdPacket.tickTime = (ulong)(DateTime.Now.Ticks - tickRateOffset) / TimeSpan.TicksPerMillisecond;
            cmdPacket.payloadSize = 0;

            //Serialize each parameter into the payload
            if(cmdParams != null)
                foreach(object param in cmdParams)
                {
                    if (param.GetType() == typeof(int))
                    {
                        byte[] bytes = BitConverter.GetBytes((int)param);
                        bytes.CopyTo(cmdPacket.payload, cmdPacket.payloadSize);
                        cmdPacket.payloadSize += bytes.Length;
                    }
                    else if(param.GetType() == typeof(bool))
                    {
                        cmdPacket.payload[cmdPacket.payloadSize] = (byte)( (bool)param ? 1 : 0);
                        cmdPacket.payloadSize++;
                    }
                    else
                        throw new CommandCenterException("Command packet parameter is an invalid type",null);
                }
            //Send the command to the device until the command is sent, exit is specifed and msg is not assurred 
            do
            {
                try
                {
                    if (sendPacket(cmdPacket, NETWORK_TIMEOUT * 1000))
                        return;
                    if(shouldReportError)
                        reportError("Failed to provide response to client");
                }
                catch (Exception e)
                {
                    if(shouldReportError)
                        reportError(e.Message);
                }
            } while (!exit && assured);
        }

        private void provideResponse(NetworkCode cmdPacket)
        {
            switch (cmdPacket.command)
            {
                case (int)CommandCodes.TAG_ARRIVE:
                    break;
                case (int)CommandCodes.UNLOCK:
                    sendCommand(cmdPacket, CommandCodes.UNLOCK, NETWORK_TIMEOUT * 1000,true,true);
                    break;
                case (int)CommandCodes.TAG_LEAVE:
                    break;
                case (int)CommandCodes.ALIVE:
                    break;
                case (int)CommandCodes.TAG_PRESENT_TOO_LONG:
                    break;
                case (int)CommandCodes.START:
                    sendCommand(cmdPacket, CommandCodes.START, NETWORK_TIMEOUT * 1000, true,true);
                    return;
                case (int)CommandCodes.DEVICE_ERROR:
                    return;
                default:
                    break;
            }
            if (cmdPacket.command == (int)CommandCodes.NONE)
                return;
            sendCommand(cmdPacket, CommandCodes.CONFIRMATION_SYNC_TICK_COUNT, NETWORK_TIMEOUT * 1000, true, true, (int)CommandCodes.NONE);
        }

        private void handleDeviceError(NetworkCode errorCode)
        {
            //Tell device to wait
            sendCommand(new NetworkCode(), CommandCodes.DEVICE_ERROR, NETWORK_TIMEOUT * 1000, true, false, (int)0, false, true);
            
            //Wait for server thread to handle
            deviceError = BitConverter.ToInt32(errorCode.payload, 0);
            pauseExecution = true;
            while (pauseExecution)
            {
                //tell device to keep waiting
                Thread.Sleep(NETWORK_TIMEOUT > 500 ? 500 : NETWORK_TIMEOUT);
                sendCommand(new NetworkCode(), CommandCodes.DEVICE_ERROR, NETWORK_TIMEOUT * 1000, true, false, (int)0, false, true);
            }
            //Tell device to continue and whether or not to continue execution
            sendCommand(new NetworkCode(), CommandCodes.DEVICE_ERROR, NETWORK_TIMEOUT * 1000, true,true, (int)0,continueAfterDeviceError,false);
        }

        private void reportCommandInfo(NetworkCode cmdPacket,bool showAlive = true)
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
            serverErrorMessage = error;
            pauseExecution = true;
            while (pauseExecution)
                Thread.Yield();
        }
    }
}
