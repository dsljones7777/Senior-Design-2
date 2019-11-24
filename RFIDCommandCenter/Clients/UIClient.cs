using Network;
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
        public Exception lastException;                             //The last exception that occurred on the thread that caused it to exit
        public string clientUsername = null;                        //Null when no one is logged in
        public int role;                                            //Role of the client. Used to determine access to RPCS

        //First obj in tuple is the device serial number, the second object is a string for now but it is what to send to the client
        List<Tuple<string, object>> messagesToSend = new List<Tuple<string, object>>();

        //First obj in tuple is the device serial number, the second object is a bool for now but it is what client sends back to us
        List<Tuple<string, object>> messagesRcvd = new List<Tuple<string, object>>();

        NetworkStream clientStream;

        public UIClient(Socket who,NetworkCommunication comObj) : base (who,comObj)
        {
            clientStream = new NetworkStream(who);
        }
        
        bool tendToClientRequests()
        {
            if (!clientStream.DataAvailable)
                return false;
            BinaryFormatter formatter = new BinaryFormatter();
            object cmd = formatter.Deserialize(clientStream);
            try
            {
                executeRPC(cmd);
            }
            catch (CommandCenterException e)
            {
                try
                {
                    tellClient(null, e.Message);
                }
                catch
                {

                }
            }
            catch(Exception e)
            {

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
        
        void executeRPC(object cmd)
        {
            using (var context = new DataContext())
            {
                if (cmd.GetType() == typeof(LoginUserRPC))
                    loginUser((LoginUserRPC)cmd, context);
                else if (clientUsername == null)
                    throw new Exception("You must be logged in to do that");
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
                    viewTags(context);
                else if (cmd.GetType() == typeof(ErrorReplyRPC))
                {
                    ErrorReplyRPC op = (ErrorReplyRPC)cmd;
                    if (op.serialNumber != null)
                        msgRecevied(op.serialNumber, op.retry);
                }
                else
                    throw new Exception("Client sent an invalid RPC", null);
            }
        }

        public void addErrorMessage(string serial, string message)
        {
            lock (messagesToSend)
            {
                messagesToSend.Add(new Tuple<string, object>(serial, message));
            }
        }

        void msgRecevied(string serial, bool didUsrSayYes)
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

        void viewTags(DataContext context)
        {
            var allTags = new Logic.ViewTags();
            ViewTagsRPC rpc = new ViewTagsRPC();
            rpc.tagList = allTags.Execute(context);
            sendRPC(rpc);
        }

        void loginUser(LoginUserRPC loginData, DataContext context)
        {
            var login = new Logic.Login();
            var pwdBytes = Encoding.ASCII.GetBytes(loginData.password);
            try
            {
                login.Execute(loginData.username, pwdBytes, out role, context);
                loginData.isValidLogin = true;
                clientUsername = loginData.username;
                loginData.isAdmin = (role == (int)NetworkLib.Role.Admin);
            }
            catch
            {
                loginData.isValidLogin = false;
            }
            sendRPC(loginData);
        }

        private static void deleteLocation(object cmd, DataContext context)
        {
            DeleteLocationRPC op = (DeleteLocationRPC)cmd;
            var delLoc = new Logic.DeleteLocation();
            //TODO: Delete location should not include reader serial in
            delLoc.Execute(op.locationName, context);
        }

        private static void saveLocation(object cmd, DataContext context)
        {
            SaveLocationRPC op = (SaveLocationRPC)cmd;
            var saveLoc = new Logic.SaveLocation();
            saveLoc.Execute(op.locationName, op.readerSerialIn, op.readerSerialOut, context);
        }

        private static void deleteSystemUser(object cmd, DataContext context)
        {
            DeleteSystemUserRPC op = (DeleteSystemUserRPC)cmd;
            var delSysUser = new Logic.DeleteSystemUser();
            delSysUser.Execute(op.username, context);
        }

        private static void saveSystemUser(object cmd, DataContext context)
        {
            SaveSystemUserRPC op = (SaveSystemUserRPC)cmd;
            var saveSysUser = new Logic.SaveSystemUser();
            //TODO: Implement hashing
            byte[] passBytes = null;
            saveSysUser.Execute(op.username, passBytes, op.role, context);
        }

        private static void deleteTag(object cmd, DataContext context)
        {
            DeleteTagRPC op = (DeleteTagRPC)cmd;
            var deleteTag = new Logic.DeleteTag();
            deleteTag.Execute(op.tagNumber, context);
        }

        private static void saveTag(object cmd, DataContext context)
        {
            SaveTagRPC op = (SaveTagRPC)cmd;
            var saveTag = new Logic.SaveTag();
            saveTag.Execute(op.tagNumber, op.name, op.guest, context);
        }

        void sendRPC(UINetworkPacket packet)
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

        private static void editLocation(object cmd, DataContext context)
        {
            EditLocationRPC op = (EditLocationRPC)cmd;
            var editLocation = new Logic.EditLocation();
            editLocation.Execute(op.currentLocationName, op.newLocationName, op.readerSerialIn, op.readerSerialOut, context);
        }

        private static void editTag(object cmd, DataContext context)
        {
            EditTagRPC op = (EditTagRPC)cmd;
            var editTag = new Logic.EditTag();
            editTag.Execute(op.tagNumber, op.name, op.lost,op.guest, context);
        }

        private static void removeConnectedDevices(DataContext context)
        {
            var removeDevices = new Logic.RemoveAllConnectedDevices();
            removeDevices.Execute(context);
        }

        private static void saveAllowedTagLocation(object cmd, DataContext context)
        {
            SaveAllowedLocationsRPC op = (SaveAllowedLocationsRPC) cmd;
            var saveAllowedLocations = new Logic.SaveAllowedTagLocation();
            saveAllowedLocations.Execute(op.tagID, op.locationID, context);
        }

        private static void saveConnectedDevice(object cmd, DataContext context)
        {
            SaveConnectedDeviceRPC op = (SaveConnectedDeviceRPC)cmd;
            var saveConnectedDevice = new Logic.SaveConnectedDevice();
            saveConnectedDevice.Execute(op.serialNumber, context);
        }

        private static void tagArrive(object cmd, DataContext context)
        {
            TagArriveRPC op = (TagArriveRPC)cmd;
            var tagArrive = new Logic.TagArrive();
            tagArrive.Execute(op.tagNumber, op.deviceSerialNumber, context);
        }

        private static void tagLeave(object cmd, DataContext context)
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

        //void editUser(object cmd, DataContext context)
        //{
        //    EditUserRPC op = (EditUserRPC)cmd;
        //    var editUser = new Logic.EditUser();
        //    editUser.Execute(op)
        //}

        #endregion

    }
}
