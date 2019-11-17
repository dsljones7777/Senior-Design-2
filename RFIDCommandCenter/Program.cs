using RFIDCommandCenter.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RFIDCommandCenter
{
    public class Program
    {

        static List<RFIDDeviceClient> deviceClients = new List<RFIDDeviceClient>();
        static List<UIClient> uiClients = new List<UIClient>();

        static void handleRFIDClient(RFIDDeviceClient client)
        {
            if (!client.pauseExecution)
                return;
            string msg;
            switch((RFIDDeviceClient.ErrorCodes)client.deviceError)
            {
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_CONNECT:
                    client.continueAfterDeviceError = false;
                    msg = "A connected device client could not find it's RFID reader";
                    break;
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_READ:
                    client.continueAfterDeviceError = true;
                    msg = "A connected client failed to perform a tag read operation";
                    break;
                case RFIDDeviceClient.ErrorCodes.DEVICE_FAILED_TO_START:
                    client.continueAfterDeviceError = false;
                    msg = "A connected client failed to start it's RFID reader";
                    break;
                case RFIDDeviceClient.ErrorCodes.TAG_MEMORY_BUFFER_FULL:
                    client.continueAfterDeviceError = false;
                    msg = "A connected client failed ran out of memory for remembered tags";
                    break;
                case RFIDDeviceClient.ErrorCodes.TAG_TOO_LONG:
                    client.continueAfterDeviceError = true;
                    msg = "A connected client encountered a tag whose EPC was greater than 12 bytes";
                    break;
                default:
                    client.continueAfterDeviceError = true;
                    msg = null;
                    break;
            }
            if(client.deviceError != 0)
            {
                DialogResult result = MessageBox.Show(null, "A device error occurred:\n" + msg,
                    "Device Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if(result != DialogResult.Retry)
                {
                    //Shut down client
                    client.exit = true;
                    client.continueAfterDeviceError = false;
                    client.pauseExecution = false;
                    return;
                }
            }
            client.deviceError = 0;
            if(client.serverErrorMessage != null)
            {
                DialogResult result = MessageBox.Show(null, "A server error occurred:\n" + client.serverErrorMessage,
                   "Server Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result != DialogResult.Retry)
                {
                    //Shut down client
                    client.exit = true;
                    client.continueAfterDeviceError = false;
                    client.pauseExecution = false;
                    return;
                }
                client.serverErrorMessage = null;
            }
            client.pauseExecution = false;
        }

        static void handleUIClient(UIClient client)
        {

        }

        static void addRFIDDeviceClient(RFIDDeviceClient deviceClient)
        {
            lock (deviceClients)
            {
                deviceClients.Add(deviceClient);
            }
            
            while(!ThreadPool.QueueUserWorkItem(deviceClient.serverThreadRoutine, deviceClient))
            {
                DialogResult result = MessageBox.Show(null,
               "A thread for the client could not be created",
               "Server Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result != DialogResult.Retry)
                    return;
            }
        }

        static void addUIClient(Client client)
        {

        }
        static void Main(string[] args)
        {
            //GET YOUR SHIT OUT OF MY CODE FILES ;)
            //using (var context = new DataContext())
            //{
            //    var component = new Logic.GetSystemUserByUsername();
            //    var test = component.Execute("cosimo", context);
            //}
            

            //Create an object to start accepting ui and device clients
            NetworkCommunication netObj = NetworkCommunication.createNetworkCommunicationObject();
            while(true)
            {
                try
                {
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
                catch(Exception e)
                {
                    DialogResult result = MessageBox.Show(null,
                      "The client failed to connect\nContinue?",
                      "Client Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result != DialogResult.Yes)
                        return;
                    continue;
                }

                //If new client was accepted then start
                if(newClient != null)
                {
                    UIClient uiClient = newClient as UIClient;
                    RFIDDeviceClient devClient = newClient as RFIDDeviceClient;
                    if (devClient != null)
                        addRFIDDeviceClient(devClient);
                    else if (uiClient != null)
                        addUIClient(devClient);
                    else
                        MessageBox.Show(null, "A client connected who is not a device or ui client", "Unknown Client", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //Service every device client and ui client
                lock(deviceClients)
                {
                    foreach(RFIDDeviceClient client in deviceClients)
                        handleRFIDClient(client);
                }
                lock(uiClients)
                {
                    foreach (UIClient client in uiClients)
                        handleUIClient(client);
                }
            }
           
        }
    }
}
