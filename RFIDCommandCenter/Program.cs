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
        static List<RFIDUIClient> uiClients = new List<RFIDUIClient>();
        static void Main(string[] args)
        {
            //using (var context = new DataContext())
            //{
            //    var component = new Logic.GetSystemUserByUsername();
            //    var test = component.Execute("cosimo", context);
            //}
            
            NetworkCommunication netObj = NetworkCommunication.createNetworkCommunicationObject();
        RESTART_CONNECTION:
            try
            {
                netObj.start();
            }
            catch(Exception e)
            {
                DialogResult result = MessageBox.Show(null, 
                    "The command center network could not be created:\n" + e.Message, 
                    "Network Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result != DialogResult.Retry)
                    return;
                goto RESTART_CONNECTION;
            }
            //Program loop
            while(true)
            {
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
                if(newClient != null)
                {
                    RFIDDeviceClient deviceClient = newClient as RFIDDeviceClient;
                    if (deviceClient != null)
                    {
                        lock (deviceClients)
                        {
                            deviceClients.Add(deviceClient);
                        }
                        if (!ThreadPool.QueueUserWorkItem(deviceClient.serverThreadRoutine, deviceClient))
                        {
                            DialogResult result = MessageBox.Show(null,
                           "A thread for the client could not be created",
                           "Server Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                            if (result != DialogResult.Retry)
                                return;
                            continue;
                        }
                    }
                    RFIDUIClient uiClient = newClient as RFIDUIClient;
                    if (uiClient != null)
                    {

                    }
                }


                
                
            }
           
        }
    }
}
