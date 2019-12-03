using RFIDCommandCenter.Logic;
using SharedLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RFIDCommandCenter
{
    internal class ServiceThread
    {
        List<RFIDDeviceClient> deviceClients = new List<RFIDDeviceClient>();
        List<UIClient> uiClients = new List<UIClient>();
        SortedList<string,RFIDDeviceClient> systemDevices = new SortedList<string,RFIDDeviceClient>();                    //Device serials that are in the DB as well and connected

        SortedList<string,RFIDDeviceClient> nonSystemDevices = new SortedList<string, RFIDDeviceClient>();   //Device serials that are not in the DB and the tags that have been read from them
        
        bool handleRFIDClient(RFIDDeviceClient client, out string deviceError, out string serverError)
        {
            deviceError = null;
            serverError = null;
            if (client.exit)
                return true;
            if (!client.pauseExecution)
                return false;
            if (client.serverErrorMessage != null)
            {
                serverError = "A server error occurred:\n" + client.serverErrorMessage;
                client.serverErrorMessage = null;
                client.continueAfterDeviceError = false;
                client.exit = true;
                client.pauseExecution = false;
                return true;
            }
            switch ((RFIDDeviceClient.ErrorCodes)client.deviceError)
            {
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_CONNECT:
                    client.continueAfterDeviceError = false;
                    deviceError = "A connected device client could not find it's RFID reader";
                    client.deviceError = 0;
                    break;
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_READ:
                    client.continueAfterDeviceError = true;
                    deviceError = "A connected client failed to perform a tag read operation";
                    client.deviceError = 0;
                    break;
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_START:
                    client.continueAfterDeviceError = false;
                    deviceError = "A connected client failed to start it's RFID reader";
                    client.deviceError = 0;
                    break;
                case RFIDDeviceClient.ErrorCodes.TAG_MEMORY_BUFFER_FULL:
                    client.continueAfterDeviceError = false;
                    deviceError = "A connected client failed ran out of memory for remembered tags";
                    client.deviceError = 0;
                    break;
                case RFIDDeviceClient.ErrorCodes.TAG_TOO_LONG:
                    client.continueAfterDeviceError = true;
                    deviceError = "A connected client encountered a tag whose EPC was greater than 12 bytes";
                    client.deviceError = 0;
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
                        systemDevices.Add(client.deviceSerialNumber, client);
                        client.isSystemDevice = true;
                    }  
                    else
                    {
                        nonSystemDevices.Add(client.deviceSerialNumber, client);
                        client.isSystemDevice = false;
                    }
                    client.pauseExecution = false;
                    break;
            }
            return false;
        }

        void changeToSystemDevice(string serial)
        {
            if (serial == null)
                return;
            if (nonSystemDevices.ContainsKey(serial))
            {
                RFIDDeviceClient device = nonSystemDevices[serial];
                device.isSystemDevice = true;
                nonSystemDevices.Remove(serial);
                systemDevices.Add(serial, device);
            }
        }

        void changeToNonSystemDevice(string serial)
        {
            if (serial == null)
                return;
            if (systemDevices.ContainsKey(serial))
            {
                RFIDDeviceClient device = systemDevices[serial];
                device.isSystemDevice = false;
                systemDevices.Remove(serial);
                nonSystemDevices.Add(serial, device);
            }
        }

        void handleClientRequest(UIClient client)
        {
            if (client.request.GetType() == typeof(GetUnconnectedDevicesRPC))
                getUnconnectedDevices(client);
            else if (client.request.GetType() == typeof(GetAllConnectedDevicesRPC))
                getAllConnectedDevices(client);
            else if (client.request.GetType() == typeof(GetAllDevicesRPC))
                getAllDevices(client);
            else if (client.request.GetType() == typeof(WriteTagRPC))
                writeTag(client);
            else if (client.request.GetType() == typeof(ViewTagsRPC))
                viewNonsystemTags(client);
            else if (client.request.GetType() == typeof(SaveLocationRPC))
                updateDevicesOnSaveLocation(client);
            else if (client.request.GetType() == typeof(EditLocationRPC))
                updateDevicesOnEditLocation(client);
            else if (client.request.GetType() == typeof(DeleteLocationRPC))
                updateDevicesOnDeleteLocation(client);
            else if (client.request.GetType() == typeof(ChangeDeviceModeRPC))
                changeDeviceMode(client);
            client.request = null;
        }

        private void changeDeviceMode(UIClient client)
        {
            ChangeDeviceModeRPC rpc = (ChangeDeviceModeRPC)client.request;
            RFIDDeviceClient device;
            if (nonSystemDevices.ContainsKey(rpc.deviceSerial))
                device = nonSystemDevices[rpc.deviceSerial];
            else if (systemDevices.ContainsKey(rpc.deviceSerial))
                device = systemDevices[rpc.deviceSerial];
            else
                return;
            device.inVirtualMode = rpc.virtualMode;
            device.pendingDeviceCommand = RFIDDeviceClient.CommandCodes.CHANGE_MODE;
        }

        private void updateDevicesOnDeleteLocation(UIClient client)
        {
            DeleteLocationRPC rpc = (DeleteLocationRPC)client.request;
            foreach (var x in rpc.removedSerials)
                changeToNonSystemDevice(x);
        }

        private void updateDevicesOnEditLocation(UIClient client)
        {
            EditLocationRPC rpc = (EditLocationRPC)client.request;
            changeToNonSystemDevice(rpc.oldReaderSerialIn);
            changeToNonSystemDevice(rpc.oldReaderSerialOut);
            changeToSystemDevice(rpc.readerSerialIn);
            changeToSystemDevice(rpc.readerSerialOut);
        }

        private void updateDevicesOnSaveLocation(UIClient client)
        {
            SaveLocationRPC rpc = (SaveLocationRPC)client.request;
            changeToSystemDevice(rpc.readerSerialIn);
            changeToSystemDevice(rpc.readerSerialOut);
        }

        //Return true if thread should be removed
        bool handleUIClient(UIClient client, SortedList<string,object> deviceResponses)
        {
            if (client.threadExited)
                return true;
            if (client.request == null)
                return false;
            handleClientRequest(client);
            lock(client.messagesRcvd)
            {
                if (client.messagesRcvd.Count == 0)
                    return false;
                foreach(var x in client.messagesRcvd)
                    if (!deviceResponses.ContainsKey(x.Item1))
                        deviceResponses.Add(x.Item1, x.Item2);
                client.messagesRcvd.Clear();
            }
            return false;
        }

        private void viewNonsystemTags(UIClient client)
        {
            ViewTagsRPC rpc = new ViewTagsRPC()
            {
                tagList = new List<SharedLib.SharedModels.ViewTagModel>()
            };
            
            foreach(var deviceKeyPair in nonSystemDevices)
            {
                rpc.tagList = rpc.tagList.Union(deviceKeyPair.Value.readTags.ConvertAll(
                    bytesIn =>
                    {
                        return new SharedLib.SharedModels.ViewTagModel()
                        {
                            TagNumber = bytesIn
                        };
                    })).ToList();
            }
            lock (client.responses)
            {
                client.responses.Add(rpc);
            }
        }

        private void writeTag(UIClient client)
        {
            WriteTagRPC rpc = (WriteTagRPC)client.request;
            if(!nonSystemDevices.ContainsKey(rpc.targetSerialNumber))
                return;
            nonSystemDevices[rpc.targetSerialNumber].tagToWrite = rpc.newTagBytes;
        }

        private void getAllDevices(UIClient client)
        {
            GetAllDevicesRPC rpc = (GetAllDevicesRPC)client.request;
            foreach (var x in rpc.devices)
            {
                if (x.serialNumber == null)
                    continue;
                x.inDB = true;
                x.connected = systemDevices.ContainsKey(x.serialNumber);
                if (x.connected)
                    x.isVirtual = systemDevices[x.serialNumber].inVirtualMode;
                else
                    x.isVirtual = false;

            }
            lock (nonSystemDevices)
            {
                foreach (var x in nonSystemDevices)
                {
                    rpc.devices.Add(
                        new DeviceStatus()
                        {
                            connected = true,
                            inDB = false,
                            isVirtual = x.Value.inVirtualMode,
                            serialNumber = x.Key
                        });
                }
            }
            lock (client.responses)
            {
                client.responses.Add(rpc);
            }
        }

        private void getAllConnectedDevices(UIClient client)
        {
            GetAllConnectedDevicesRPC rpc = new GetAllConnectedDevicesRPC();
            rpc.serialNumbers = systemDevices.Keys.Union(nonSystemDevices.Keys).ToList();
            lock (client.responses)
            {
                client.responses.Add(rpc);
            }
        }

        private void getUnconnectedDevices(UIClient client)
        {
            GetUnconnectedDevicesRPC rpc = new GetUnconnectedDevicesRPC()
            {
                serialNumbers = nonSystemDevices.Keys.ToList()
            };
            lock (client.responses)
            {
                client.responses.Add(rpc);
            }
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
            SortedList<string, object> deviceResponses = new SortedList<string, object>();
            for(int i = 0; i < deviceClients.Count;i ++)
            {
                var dev = deviceClients[i];
                if(handleRFIDClient(dev, out devError, out serverError))
                {
                    deviceClients.RemoveAt(i);

                    if (dev.deviceSerialNumber != null && nonSystemDevices.ContainsKey(dev.deviceSerialNumber))
                        nonSystemDevices.Remove(dev.deviceSerialNumber);
                    else if (dev.deviceSerialNumber != null && systemDevices.ContainsKey(dev.deviceSerialNumber))
                        systemDevices.Remove(dev.deviceSerialNumber);
                    try
                    {
                        dev.destroy();
                    }
                    catch
                    {

                    }
                    i--;
                    continue;
                }
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
            {
                if(handleUIClient(uiClients[i],deviceResponses))
                {
                    uiClients.RemoveAt(i);
                    i--;
                }
            }
            foreach(var x in deviceResponses)
            {
                for (int i = 0; i < deviceClients.Count; i++)
                {
                    if (deviceClients[i].deviceSerialNumber != x.Key)
                        continue;
                    if ((bool)x.Value)
                    {
                        deviceClients[i].continueAfterDeviceError = true;
                        deviceClients[i].pauseExecution = false;
                    }
                    else
                    {
                        deviceClients[i].continueAfterDeviceError = false;
                        deviceClients[i].exit = true;
                        deviceClients[i].pauseExecution = false;
                    }
                }
            }
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
}
