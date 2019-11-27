using RFIDCommandCenter.Logic;
using SharedLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RFIDCommandCenter
{
    internal class ServiceThread
    {
        List<RFIDDeviceClient> deviceClients = new List<RFIDDeviceClient>();
        List<UIClient> uiClients = new List<UIClient>();
        SortedList<string,string> systemDevices = new SortedList<string,string>();                    //Device serials that are in the DB as well and connected

        SortedList<string, List<byte[]>> nonSystemDevices = new SortedList<string, List<byte[]>>();   //Device serials that are not in the DB and the tags that have been read from them

        void handleRFIDClient(RFIDDeviceClient client, out string deviceError, out string serverError)
        {
            deviceError = null;
            serverError = null;
            if (!client.pauseExecution)
                return;
            if (client.serverErrorMessage != null)
            {
                serverError = "A server error occurred:\n" + client.serverErrorMessage;
                client.serverErrorMessage = null;
            }
            switch ((RFIDDeviceClient.ErrorCodes)client.deviceError)
            {
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_CONNECT:
                    client.continueAfterDeviceError = false;
                    deviceError = "A connected device client could not find it's RFID reader";
                    break;
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_READ:
                    client.continueAfterDeviceError = true;
                    deviceError = "A connected client failed to perform a tag read operation";
                    break;
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_START:
                    client.continueAfterDeviceError = false;
                    deviceError = "A connected client failed to start it's RFID reader";
                    break;
                case RFIDDeviceClient.ErrorCodes.TAG_MEMORY_BUFFER_FULL:
                    client.continueAfterDeviceError = false;
                    deviceError = "A connected client failed ran out of memory for remembered tags";
                    break;
                case RFIDDeviceClient.ErrorCodes.TAG_TOO_LONG:
                    client.continueAfterDeviceError = true;
                    deviceError = "A connected client encountered a tag whose EPC was greater than 12 bytes";
                    break;
                default:
                    client.continueAfterDeviceError = true;
                    if (client.deviceSerialNumber == null)
                        break;
                    if (systemDevices.ContainsKey(client.deviceSerialNumber) || nonSystemDevices.ContainsKey(client.deviceSerialNumber))
                        break;
                    List<string> dbSerials = new List<string>();
                    using (DataContext context = new DataContext())
                        dbSerials = (new GetAllUniqueSerialNumbers()).Execute(context);
                    if (dbSerials.Contains(client.deviceSerialNumber))
                    {
                        systemDevices.Add(client.deviceSerialNumber, null);
                        client.isSystemDevice = true;
                    }  
                    else
                    {
                        nonSystemDevices.Add(client.deviceSerialNumber, new List<byte[]>());
                        client.isSystemDevice = false;
                    }
                        
                    client.pauseExecution = false;
                    break;
            }
            
        }

        //Return true if thread should be removed
        bool handleUIClient(UIClient client)
        {
            if (client.threadExited)
                return true;
            if (client.request == null)
                return false;
            if(client.request.GetType()== typeof(GetUnconnectedDevicesRPC))
            {
                GetUnconnectedDevicesRPC rpc = new GetUnconnectedDevicesRPC()
                {
                    serialNumbers = nonSystemDevices.Keys.ToList()
                };
                lock(client.responses)
                {
                    client.responses.Add(rpc);
                }
            }
            else if(client.request.GetType() == typeof(GetAllConnectedDevicesRPC))
            {
                GetAllConnectedDevicesRPC rpc = new GetAllConnectedDevicesRPC();
                rpc.serialNumbers = systemDevices.Keys.Union(nonSystemDevices.Keys).ToList();
                lock(client.responses)
                {
                    client.responses.Add(rpc);
                }
            }
            else if(client.request.GetType() == typeof(GetAllDevicesRPC))
            {
                GetAllDevicesRPC rpc = (GetAllDevicesRPC)client.request;
                foreach(var x in rpc.devices)
                {
                    x.inDB = true;
                    x.connected = systemDevices.ContainsKey(x.serialNumber);
                }
                rpc.devices.Union(nonSystemDevices.ToList().ConvertAll(
                    a => new DeviceStatus()
                    {
                        connected = true,
                        inDB = false,
                        serialNumber = a.Key
                    }));
                lock(client.responses)
                {
                    client.responses.Add(rpc);
                }
            }
            client.request = null;
            return false;
        }

        public void AddClient(Client who)
        {
            if (who.GetType() == typeof(RFIDDeviceClient))
                addRFIDDeviceClient((RFIDDeviceClient)who);
            else
                addUIClient((UIClient)who);
        }

        public void HandleClients()
        {
            string devError, serverError;
            for(int i = 0; i < deviceClients.Count;i ++)
            {
                var dev = deviceClients[i];
                handleRFIDClient(dev, out devError, out serverError);
                if(devError != null)
                {
                    foreach (var ui in uiClients)
                        ui.addErrorMessage(dev.deviceSerialNumber, devError);
                }
                if(serverError != null)
                {
                    foreach (var ui in uiClients)
                        ui.addErrorMessage(dev.deviceSerialNumber, serverError);
                }
            }  
            for(int i = 0; i < uiClients.Count; i ++)
                handleUIClient(uiClients[i]);
        }

        private void addRFIDDeviceClient(RFIDDeviceClient deviceClient)
        {
            lock (deviceClients)
            {
                deviceClients.Add(deviceClient);
            }

            while (!ThreadPool.QueueUserWorkItem(deviceClient.serverThreadRoutine, deviceClient))
            {
                DialogResult result = MessageBox.Show(null,
               "A thread for the client could not be created",
               "Server Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result != DialogResult.Retry)
                    return;
            }
        }

        private void addUIClient(UIClient client)
        {
            lock(uiClients)
            {
                uiClients.Add(client);
            }
            while (!ThreadPool.QueueUserWorkItem(client.serverThreadRoutine, client));
        }

       
    };



    public class Program
    {
        
        static void Main(string[] args)
        {
            ServiceThread serverThread = new ServiceThread();
            //Create an object to start accepting ui and device clients
            NetworkCommunication netObj = NetworkCommunication.createNetworkCommunicationObject();
            while(true)
            {
                try
                {
                    //try to improve performance of the first db connection
                    using (var context = new DataContext())
                    {
                        var dummyQuery = context.Tags.Select(x => x.ID == 0);
                    }
                    netObj.start();
                }
                catch (Exception e)
                {
                    DialogResult result = MessageBox.Show(null,
                        "The command center network could not be created:\n" + e.Message,
                        "Network Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result != DialogResult.Retry)
                        return;
                    continue;
                }
                break;
            }
            
            //Program loop
            while(true)
            {
                //Accept a new client block for approx 100ms
                Client newClient;
                try
                {
                    newClient = netObj.acceptPendingClient(100000);
                }
                catch
                {
                    DialogResult result = MessageBox.Show(null,
                      "The client failed to connect\nContinue?",
                      "Client Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result != DialogResult.Yes)
                        return;
                    continue;
                }
                
                if(newClient != null)
                    serverThread.AddClient(newClient);
                serverThread.HandleClients();
            }
           
        }
    }
}
