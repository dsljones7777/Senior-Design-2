using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
           5 * 1000;   //10 seconds
#else
            10000;           //2 seconds
#endif

        long tickRateOffset;
        internal volatile bool inVirtualMode = false;
        internal volatile CommandCodes pendingDeviceCommand;
        internal volatile bool isSystemDevice;              //Does the device exist in the database
        internal volatile bool exit;
        internal volatile bool pauseExecution;
        internal volatile int deviceError;
        internal volatile bool continueAfterDeviceError;
        internal volatile string serverErrorMessage;
        internal string deviceSerialNumber;
        internal List<byte[]> readTags = new List<byte[]>();
        internal volatile byte[] tagToWrite = null;
        
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
            TAG_PRESENT_TOO_LONG = 9,
            WRITE_TAG = 12,
            ALIVE = 13,
            PING = 14,
            DEVICE_ERROR = 15,
            CONFIRMATION_SYNC_TICK_COUNT = 16,
            REBOOT_READER = 17,
            WAIT = 18,					//Wait for a response, pump alive packets
            SERIAL_NUMBER = 19,          //Request device serial
            START_READER = 20,
            CHANGE_MODE = 21
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
            byte [] buffer= new byte[1];
            comObj.readFrom(who, buffer, 1);
            if (buffer[0] != 0)
                this.inVirtualMode = true;
        }

        public void destroy()
        {
            this.clientSocket.Close();
            this.clientSocket.Dispose();
        }
        
        public override void serverThreadRoutine(Object state)
        {
            //Create packet to tell client to start, start it's reader, lock the door and give up it's serial number
            
            NetworkCode bufferPacket = new NetworkCode();
            sendCommand(bufferPacket, CommandCodes.START, NETWORK_TIMEOUT * 1000, true, true);
            sendCommand(bufferPacket, CommandCodes.START_READER, NETWORK_TIMEOUT * 1000, true, true);
            sendCommand(bufferPacket, CommandCodes.LOCK, NETWORK_TIMEOUT * 1000, false, true);
            sendCommand(bufferPacket, CommandCodes.SERIAL_NUMBER, NETWORK_TIMEOUT * 1000, true, true,new byte[65]);
            
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
#if DEBUG
                    reportCommandInfo(bufferPacket);
#endif
                    provideResponse(bufferPacket);
                    decipherAndSendPendingDeviceCommand(bufferPacket);
                    
                }
                catch (Exception e)
                {
                    reportError(e.Message);
                    continue;
                }
            }
        }

        private void decipherAndSendPendingDeviceCommand(NetworkCode packet)
        {
            if (pendingDeviceCommand == CommandCodes.NONE)
                return;
            if (pendingDeviceCommand == CommandCodes.CHANGE_MODE)
            {
                sendCommand(packet, pendingDeviceCommand, NETWORK_TIMEOUT * 1000,false, true, inVirtualMode);
                pendingDeviceCommand = CommandCodes.NONE;
                exit = true;
            }

        }

        private bool isByteArrayEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        private void executePacketRequest(NetworkCode cmdPacket)
        {
            byte[] tagNumber = new byte[12];
            using (var context = new DataContext())
            {
                switch (cmdPacket.command)
                {
                    case (int)CommandCodes.TAG_ARRIVE:
                        Array.Copy(cmdPacket.payload, tagNumber, 12);
                        cmdPacket.command = (int)CommandCodes.LOCK;
                        if (isSystemDevice)
                        {
                            var tagArrive = new Logic.TagArrive();
                            if(tagArrive.Execute(tagNumber, deviceSerialNumber, context))
                                cmdPacket.command = (int)CommandCodes.UNLOCK;
                        }
                        else
                        {
                            
                            lock(readTags)
                            {
                                readTags.Add(tagNumber);
                            }
                        }
                        break;
                    case (int)CommandCodes.TAG_LEAVE:
                        cmdPacket.command = (int)CommandCodes.LOCK;
                        Array.Copy(cmdPacket.payload, tagNumber, 12);
                        if(!isSystemDevice)
                        {
                            lock(readTags)
                            {
                                for(int i = 0; i < readTags.Count; i ++)
                                {
                                    if (!isByteArrayEqual(readTags[i], tagNumber))
                                        continue;
                                    readTags.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        break;
                    case (int)CommandCodes.ALIVE:
                        
                        break;
                    case (int)CommandCodes.TAG_PRESENT_TOO_LONG:
                        //Write to db
                        break;
                    case (int)CommandCodes.DEVICE_ERROR:
                        handleDeviceError(cmdPacket);
                        break;
                    case (int)CommandCodes.SERIAL_NUMBER:
                        StringBuilder builder = new StringBuilder();
                        for(int i = 0; cmdPacket.payload[i] != 0; i ++)
                            builder.Append((char)cmdPacket.payload[i]);
                        deviceSerialNumber = builder.ToString();
                        pauseExecution = true;
                        while (pauseExecution)
                            Thread.Yield();
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
                    else if(param.GetType() == typeof(byte[]))
                    {
                        Array.Copy((byte[])param, 0, cmdPacket.payload, cmdPacket.payloadSize, ((byte[])param).Length);
                        cmdPacket.payloadSize += ((byte[])param).Length;
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
                    sendCommand(cmdPacket, CommandCodes.CONFIRMATION_SYNC_TICK_COUNT, NETWORK_TIMEOUT * 1000, true, true, (int)CommandCodes.NONE);
                    sendCommand(cmdPacket, CommandCodes.UNLOCK, NETWORK_TIMEOUT * 1000,true,true);
                    break;
                case (int)CommandCodes.LOCK:
                    sendCommand(cmdPacket, CommandCodes.CONFIRMATION_SYNC_TICK_COUNT, NETWORK_TIMEOUT * 1000, true, true, (int)CommandCodes.NONE);
                    sendCommand(cmdPacket, CommandCodes.LOCK, NETWORK_TIMEOUT * 1000, true, true);
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
            if (cmdPacket.command != (int)CommandCodes.NONE)
                sendCommand(cmdPacket, CommandCodes.CONFIRMATION_SYNC_TICK_COUNT, NETWORK_TIMEOUT * 1000, true, true, (int)CommandCodes.NONE);
            //Check a queue for cmds such as write /etc. 
            if (tagToWrite == null)
                return;
            sendCommand(cmdPacket, CommandCodes.WRITE_TAG, NETWORK_TIMEOUT * 1000, false, true, 1, 1000, 3000, 1, tagToWrite);
            tagToWrite = null;
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

            byte[] epcBytes;
            switch (cmdPacket.command)
            {
                case (int)CommandCodes.START:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Device Connecting @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
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
                    epcBytes = new byte[12];
                    Array.Copy(cmdPacket.payload, 0, epcBytes,0, 12);
                    using (DataContext context = new DataContext())
                    {
                        var name = context.Tags.Where(t => t.TagNumber == epcBytes).SingleOrDefault()?.Name;
                        Console.WriteLine("Tag Name = " + name ?? "[Unknown]");
                    }
                    break;
                case (int)CommandCodes.TAG_LEAVE:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Tag Leave @ " + DateTime.Now.ToString("hh:mm:ss.FFF"));
                    Console.WriteLine("Device Tick = " + cmdPacket.tickTime + "ms");
                    Console.Write("EPC: ");
                    for (int i = 0; i < 12; i++)
                        Console.Write(String.Format("{0:x2}", cmdPacket.payload[i]));
                    epcBytes = new byte[12];
                    Array.Copy(cmdPacket.payload, 0, epcBytes, 0, 12);
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
                case (int)CommandCodes.LOCK:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Door Locked");
                    break;
                case (int)CommandCodes.UNLOCK:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Door Unlocked");
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
