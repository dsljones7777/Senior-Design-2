﻿using SharedLib.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Network.NetworkLib;

namespace UIDemo
{

    public partial class LoginScreen : Form
    {
        UIClientConnection  connection = new UIClientConnection();
        List<ServerMessage> msgQueue = new List<ServerMessage>();
        System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer();
        public LoginScreen()
        {
            InitializeComponent();
            connection.Connected += OnConnected;
            connection.ServerMessageReceived += OnServerMessageReceived;
        }

        private void handleServerReportedError(ServerMessage e)
        {
            this.Enabled = false;
            string caption;
            //Is the error a server / client UI error
            if (e.deviceSerial == null)
                caption = "Server Error";
            else
                caption = "Device Error (SN: " + e.deviceSerial + ")";
            DialogResult result = MessageBox.Show(this, e.message, caption, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            this.Enabled = true;
            if (result == DialogResult.Retry)
                e.retry = true;
            else
                e.retry = false;
            ErrorReplyRPC error = new ErrorReplyRPC()
            {
                msg = e.message,
                retry = e.retry,
                serialNumber = e.deviceSerial
            };
            try
            {
                error.executeAsync();
            }
            catch(Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void OnServerMessageReceived(object sender, Network.NetworkLib.ServerMessage e)
        {
            lock(msgQueue)
            {
                msgQueue.Add(e);
            }
            Action uiAction = new Action(
                () => 
                {
                    do
                    {
                        if (!Monitor.TryEnter(msgQueue))
                            return;
                        ServerMessage msg;
                        try
                        {
                            if (msgQueue.Count == 0)
                                return;
                            msg = msgQueue[0];
                            msgQueue.RemoveAt(0);
                        }
                        finally
                        {
                            Monitor.Exit(msgQueue);
                        }
                        handleServerReportedError(e);
                    } while (true);
                });
            if (InvokeRequired)
                this.BeginInvoke(uiAction);
            else
                //unexpected error - should not occur
                throw new Exception("Event handler can only be non- UI thread");
        }
        
        private void Form1_Shown(object sender, EventArgs e)
        {
            animTimer.Tick += AnimTimer_Tick;
            animTimer.Interval = 2000;
            animTimer.Start();
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            animTimer.Stop();
            loginLogoPictureBox.Image = UIDemo.Properties.Resources.SecureID_Static;
        }

        //void OnConnectingError(object sender, Exception error)
        //{
        //    Action uiAction = new Action(
        //        () => 
        //        {
        //            this.Enabled = false;
        //            DialogResult msgResult = MessageBox.Show(this, error.Message, "Failed to Connect", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
        //            if (msgResult != DialogResult.Retry)
        //                this.Close();
        //            else
        //                this.Enabled = true;

        //        });
        //    if (InvokeRequired)
        //        Invoke(uiAction);
        //    else
        //        uiAction.Invoke();
        //}

        void OnConnected(object sender, EventArgs args)
        {
            Action uiAction = new Action(
                () =>
                {
                    this.Text = "Tag Center (Connected)";
                });
            if (InvokeRequired)
                Invoke(uiAction);
            else
                uiAction.Invoke();
        }
        
        private async void loginButton_Click(object sender, EventArgs e)
        {
            //Change logo to blinking
            loginLogoPictureBox.Image = UIDemo.Properties.Resources.SecureID_Blink;
            //Send login user rpc
            LoginUserRPC rpc = new LoginUserRPC()
            {
                username = usrnameTextbox.Text,
                password = pwdTextbox.Text
            };

            try
            {
                rpc = (LoginUserRPC) await rpc.executeAsync();
            }
            catch(Exception except)
            {
                MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                //Change logo back
                loginLogoPictureBox.Image = UIDemo.Properties.Resources.SecureID_Static;
                pwdTextbox.Text = "";
            }

            //Load the main
            MainWindow mainWindow = new MainWindow(usrnameTextbox.Text,true);
            this.Hide();
            mainWindow.ShowDialog(this);
            this.Show();
        }

        private async void LoginScreen_Load(object sender, EventArgs e)
        {
            while(true)
            {
                //Connect to server
                try
                {
                    await connection.connect(UIDemo.Properties.Settings.Default.HostIP, UIDemo.Properties.Settings.Default.HostPort);
                    return;
                }
                catch (Exception except)
                {
                    if (MessageBox.Show(this, except.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry)
                    {
                        this.Close();
                        return;
                    }
                }
            }
        }
    }
}
