using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFIDCommandCenter
{
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
