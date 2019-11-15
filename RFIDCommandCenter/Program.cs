using RFIDCommandCenter.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFIDCommandCenter
{
    public class Program
    {

        
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
            Client newClient = netObj.acceptPendingClient();
            NetworkCode startCode = new NetworkCode()
            {
                command = (int)DeviceNetworkCommands.START,
                payloadSize = 0
            };
            newClient.sendPacket(startCode, 1000);
            do
            {
                newClient.receivePacket(startCode, 5000);
            }while(true);

        }
    }
}
