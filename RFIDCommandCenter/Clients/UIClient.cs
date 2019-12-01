using Network;
using RFIDCommandCenter.Logic;
using SharedLib;
using SharedLib.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using static SharedLib.Network.UIClientConnection;

namespace RFIDCommandCenter
{
    class UIClient : Client
    {
        public volatile bool exit = false;                          //Signals to the client to exit the program
        public volatile bool threadExited = false;                  //signals to main server that the client thread has exited
        public volatile UINetworkPacket request;
        public List<UINetworkPacket> responses = new List<UINetworkPacket>(); 
        public Exception lastException;                             //The last exception that occurred on the thread that caused it to exit
        public string clientUsername = null;                        //Null when no one is logged in
        public int role;                                            //Role of the client. Used to determine access to RPCS

        //First obj in tuple is the device serial number, the second object is a string for now but it is what to send to the client
        List<Tuple<string, object>> messagesToSend = new List<Tuple<string, object>>();

        //First obj in tuple is the device serial number, the second object is a bool for now but it is what client sends back to us
        public List<Tuple<string, object>> messagesRcvd = new List<Tuple<string, object>>();

        NetworkStream clientStream;

        public UIClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
        {
            clientStream = new NetworkStream(who);
        }
        
        bool tendToClientRequests()
        {
            lock(responses)
            {
                while(responses.Count > 0)
                {
                    sendRPC(responses[0]);
                    sendRPC(new FunctionCallStatusRPC() { });
                    responses.Remove(responses[0]);
                }
            }
            if (!clientStream.DataAvailable)
                return false;
            BinaryFormatter formatter = new BinaryFormatter();
            object cmd = formatter.Deserialize(clientStream);
            try
            {
                bool needsDelay = executeRPC(cmd);
                sendRPC(new FunctionCallStatusRPC() { waitForResponse = needsDelay});
            }
            catch(UIClientException e)
            {
                sendRPC(new FunctionCallStatusRPC() { error = e.Message});
            }
            return true;
        }

        bool sendMessagesToClient()
        {
            lock (messagesToSend)
            {
                if (messagesToSend.Count <= 0)
                    return false;
                foreach (var serialMsgTup in messagesToSend)
                    tellClient(serialMsgTup.Item1, (string)serialMsgTup.Item2);
                messagesToSend.Clear();
            }
            return true;
        }

        public override void serverThreadRoutine(object state)
        {
            while (!exit)
            {
                try
                {
                    bool didClientHaveRequest = tendToClientRequests();
                    bool wasMessageSent = sendMessagesToClient();
                    if (!didClientHaveRequest && !wasMessageSent)
                        Thread.Sleep(1);
                }
                catch (Exception e)
                {
                    lastException = e;
                    return;
                }
            }
        }
        
        //Returns true if delayed response is needed
        bool executeRPC(object cmd)
        {
            bool needsDelayedResponse = false;
            using (var context = new DataContext())
            {
                if (cmd.GetType() == typeof(LoginUserRPC))
                    loginUser((LoginUserRPC)cmd, context);
                else if (clientUsername == null)
                    throw new UIClientException("You must be logged in to do that");
                else if (cmd.GetType() == typeof(SaveTagRPC))
                    saveTag(cmd, context);
                else if (cmd.GetType() == typeof(DeleteTagRPC))
                    deleteTag(cmd, context);
                else if (cmd.GetType() == typeof(SaveSystemUserRPC))
                    saveSystemUser(cmd, context);
                else if (cmd.GetType() == typeof(DeleteSystemUserRPC))
                    deleteSystemUser(cmd, context);
                else if (cmd.GetType() == typeof(SaveLocationRPC))
                    saveLocation(cmd, context);
                else if (cmd.GetType() == typeof(DeleteLocationRPC))
                    deleteLocation(cmd, context);
                else if (cmd.GetType() == typeof(EditLocationRPC))
                    editLocation(cmd, context);
                else if (cmd.GetType() == typeof(EditTagRPC))
                    editTag(cmd, context);
                else if (cmd.GetType() == typeof(RemoveConnectedDevicesRPC))
                    removeConnectedDevices(context);
                else if (cmd.GetType() == typeof(SaveAllowedLocationsRPC))
                    saveAllowedTagLocation(cmd, context);
                else if (cmd.GetType() == typeof(SaveConnectedDeviceRPC))
                    saveConnectedDevice(cmd, context);
                else if (cmd.GetType() == typeof(TagArriveRPC))
                    tagArrive(cmd, context);
                else if (cmd.GetType() == typeof(TagLeaveRPC))
                    tagLeave(cmd, context);
                else if (cmd.GetType() == typeof(ViewAllowedLocationsRPC))
                    viewAllowedLocations(cmd, context);
                else if (cmd.GetType() == typeof(ViewUserRPC))
                    viewUsers(cmd, context);
                else if (cmd.GetType() == typeof(GetUniqueSerialNumbersRPC))
                    getUniqueSerialNumbers(cmd, context);
                else if (cmd.GetType() == typeof(ViewLocationsRPC))
                    viewLocations(context);
                else if (cmd.GetType() == typeof(ViewTagsRPC))
                    needsDelayedResponse = viewTags((ViewTagsRPC)cmd, context);
                else if (cmd.GetType() == typeof(EditUserRPC))
                    editUser((EditUserRPC)cmd, context);
                else if (cmd.GetType() == typeof(GetUnconnectedDevicesRPC))
                    needsDelayedResponse = getUnconnectedDevices(context);
                else if (cmd.GetType() == typeof(GetAllConnectedDevicesRPC))
                    needsDelayedResponse = getConnectedDevices(context);
                else if (cmd.GetType() == typeof(GetAllDevicesRPC))
                    needsDelayedResponse = getAllDevices(context);
                else if (cmd.GetType() == typeof(ErrorReplyRPC))
                    processErrorReply(cmd);
                else if(cmd.GetType() == typeof(DeleteAllowedLocationsRPC))
                    deleteAllowedLocations(cmd, context);
                else if(cmd.GetType() == typeof(WriteTagRPC))
                    writeTag(cmd);
                else
                    throw new Exception("Client sent an invalid RPC");
            }
            return needsDelayedResponse;
        }

        private void processErrorReply(object cmd)
        {
            ErrorReplyRPC op = (ErrorReplyRPC)cmd;
            if (op.serialNumber != null)
                receiveMessageFromClient(op.serialNumber, op.retry);
        }

        private void writeTag(object cmd)
        {
            verifyAdminAccess(role);
            WriteTagRPC rpc = (WriteTagRPC)cmd;
            while (this.request != null)
                Thread.Yield();
            this.request = rpc;
        }

        private bool getConnectedDevices(DataContext context)
        {
            while (request != null)
                Thread.Yield();
            request = new GetAllConnectedDevicesRPC();
            return true;
        }

        public void addErrorMessage(string serial, string message)
        {
            lock (messagesToSend)
            {
                messagesToSend.Add(new Tuple<string, object>(serial, message));
            }
        }

        void receiveMessageFromClient(string serial, bool didUsrSayYes)
        {
            lock (messagesRcvd)
            {
                messagesRcvd.Add(new Tuple<string, object>(serial, didUsrSayYes));
            }
        }

        #region RPCLogic
        void viewLocations(DataContext context)
        {
            var allLocations = new Logic.ViewLocations();
            ViewLocationsRPC rpc = new ViewLocationsRPC();
            rpc.locationList = allLocations.Execute(context);
            sendRPC(rpc);
        }

        //Returns true if a delayed response is needed otherwise false
        bool viewTags(ViewTagsRPC cmd, DataContext context)
        {
            if(cmd.nonSystemTagsOnly)
            {
                while (request != null)
                    Thread.Yield();
                request = cmd;
                return true;
            }
            else
            {
                var allTags = new Logic.ViewTags();
                cmd.tagList = allTags.Execute(context);
                sendRPC(cmd);
                return false;
            }
        }
            
       
        private bool getAllDevices(DataContext context)
        {
            while (request != null)
                Thread.Yield();
            GetAllDevicesRPC rpc = new GetAllDevicesRPC()
            {
                devices = (new Logic.GetAllUniqueSerialNumbers()).Execute(context).ConvertAll(s => new DeviceStatus { serialNumber = s })
            };
            request = rpc;
            return true;
        }

        private bool getUnconnectedDevices(DataContext context)
        {
            while (request != null)
                Thread.Yield();
            request = new GetUnconnectedDevicesRPC();
            return true;
        }

        private void editUser(EditUserRPC cmd, DataContext context)
        {
            byte[] pwdBytes = null;
            if (cmd.pass != null)
                pwdBytes = Encoding.ASCII.GetBytes(cmd.pass);
            try
            {
                new Logic.EditUser().Execute(cmd.username, pwdBytes, cmd.userRole, context);
            }
            catch(Exception x)
            {

            }
            
        }


        void loginUser(LoginUserRPC loginData, DataContext context)
        {
            var login = new Logic.Login();
            var pwdBytes = Encoding.ASCII.GetBytes(loginData.password);
            login.Execute(loginData.username, pwdBytes, out role, context);
            clientUsername = loginData.username;
            loginData.isAdmin = (role == (int)NetworkLib.Role.Admin);
            sendRPC(loginData);
        }

        private void deleteLocation(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            DeleteLocationRPC op = (DeleteLocationRPC)cmd;
            var delLoc = new Logic.DeleteLocation();
            //TODO: Delete location should not include reader serial in
            delLoc.Execute(op.locationName, context);
        }

        private void saveLocation(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            SaveLocationRPC op = (SaveLocationRPC)cmd;
            var saveLoc = new Logic.SaveLocation();
            saveLoc.Execute(op.locationName, op.readerSerialIn, op.readerSerialOut, context);
        }

        private void deleteSystemUser(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            DeleteSystemUserRPC op = (DeleteSystemUserRPC)cmd;
            var delSysUser = new Logic.DeleteSystemUser();
            delSysUser.Execute(op.username, context);
        }

        private void saveSystemUser(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            SaveSystemUserRPC op = (SaveSystemUserRPC)cmd;
            var saveSysUser = new Logic.SaveSystemUser();
            //TODO: Implement hashing
            byte[] passBytes = Encoding.ASCII.GetBytes(op.password);
            saveSysUser.Execute(op.username, passBytes, op.role, context);
        }

        private void deleteTag(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            DeleteTagRPC op = (DeleteTagRPC)cmd;
            var deleteTag = new Logic.DeleteTag();
            deleteTag.Execute(op.name, context);
        }

        private void saveTag(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            SaveTagRPC op = (SaveTagRPC)cmd;
            var saveTag = new Logic.SaveTag();
            saveTag.Execute(op.tagNumber, op.name, op.guest, context);
        }

        internal void sendRPC(UINetworkPacket packet)
        {
            BinaryFormatter formatSerializer = new BinaryFormatter();
            formatSerializer.Serialize(clientStream, packet);
        }

        void tellClient(string serialNum, string clientMsg)
        {
            ErrorReplyRPC errorRpc = new ErrorReplyRPC()
            {
                msg = clientMsg,
                serialNumber = serialNum
            };
            BinaryFormatter formatSerializer = new BinaryFormatter();
            formatSerializer.Serialize(clientStream, errorRpc);
        }

        private void editLocation(object cmd, DataContext context)
        {
            EditLocationRPC op = (EditLocationRPC)cmd;
            var editLocation = new Logic.EditLocation();
            editLocation.Execute(op.currentLocationName, op.newLocationName, op.readerSerialIn, op.readerSerialOut, context);
            while (request != null)
                Thread.Yield();
            request = op;

        }

        private void editTag(object cmd, DataContext context)
        {
            EditTagRPC op = (EditTagRPC)cmd;
            var editTag = new Logic.EditTag();
            editTag.Execute(op.tagNumber, op.name, op.lost,op.guest, context);
        }

        private void removeConnectedDevices(DataContext context)
        {
            var removeDevices = new Logic.RemoveAllConnectedDevices();
            removeDevices.Execute(context);
        }

        private void saveAllowedTagLocation(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            SaveAllowedLocationsRPC op = (SaveAllowedLocationsRPC) cmd;
            var saveAllowedLocations = new Logic.SaveAllowedTagLocation();
            saveAllowedLocations.Execute(op.tagName, op.locationNames, context);
        }

        private static void saveConnectedDevice(object cmd, DataContext context)
        {
            SaveConnectedDeviceRPC op = (SaveConnectedDeviceRPC)cmd;
            var saveConnectedDevice = new Logic.SaveConnectedDevice();
            saveConnectedDevice.Execute(op.serialNumber, context);
        }

        private void tagArrive(object cmd, DataContext context)
        {
            TagArriveRPC op = (TagArriveRPC)cmd;
            var tagArrive = new Logic.TagArrive();
            tagArrive.Execute(op.tagNumber, op.deviceSerialNumber, context);
        }

        private void tagLeave(object cmd, DataContext context)
        {
            TagLeaveRPC op = (TagLeaveRPC)cmd;
            var tagArrive = new Logic.TagLeave();
            tagArrive.Execute(op.tagNumber, context);
        }

        void viewAllowedLocations(object cmd, DataContext context)
        {
            ViewAllowedLocationsRPC op = (ViewAllowedLocationsRPC)cmd;
            var viewAllowed = new Logic.ViewAllowedLocations();
            var allowedLocations = viewAllowed.Execute(op.TagName, context);
            sendRPC(new ViewAllowedLocationsRPC { allowedLocationList = allowedLocations });
        }

        void viewUsers(object cmd, DataContext context)
        {
            ViewUserRPC op = (ViewUserRPC)cmd;
            var viewUsers = new Logic.ViewUsers();
            var userList = viewUsers.Execute(context);
            sendRPC(new ViewUserRPC { userList = userList });
        }

        void getUniqueSerialNumbers(object cmd, DataContext context)
        {
            GetUniqueSerialNumbersRPC op = (GetUniqueSerialNumbersRPC)cmd;
            var uniqueSerials = new Logic.GetAllUniqueSerialNumbers();
            sendRPC(new GetUniqueSerialNumbersRPC { serialNumberList = uniqueSerials.Execute(context) });
        }

        void verifyAdminAccess(int role)
        {
            if (role != (int)NetworkLib.Role.Admin)
                throw new UIClientException("The user does not have access to perform this action");
        }

        void deleteAllowedLocations(object cmd, DataContext context)
        {
            verifyAdminAccess(role);
            DeleteAllowedLocationsRPC op = (DeleteAllowedLocationsRPC)cmd;
            var deleteAllowedLocations = new DeleteAllowedLocations();
            deleteAllowedLocations.Execute(op.tagName, op.locationName, context);
        }
        #endregion

    }
}
