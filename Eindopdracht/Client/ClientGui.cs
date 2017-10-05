﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientGui : Form
    {
        private readonly string _ip = "127.0.0.1";
        readonly int _port = 6969;
        private TcpClient _client;
        private Session _session;
        private Thread _readThread;

        public ClientGui()
        {
            InitializeComponent();
        }

        //Connect
        #region
        private void verbindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session == null)
            {
                try
                {
                    if (IPAddress.TryParse(_ip, out IPAddress _))
                    {
                        _client = new TcpClient(_ip, _port);
                        _session = new Session(_client);
                        _readThread = new Thread(_session.Read);
                        _readThread.Start();
                        System.Diagnostics.Debug.WriteLine("Verbonden");
                    }
                    else
                    {
                        throw new Exception("IPaddres invalid");
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            else
            {
                MessageBox.Show("You are already connected to a server");
            }
        }
        #endregion

        //Disconnect
        #region
        private void verbreekVerbindingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _client?.Close();
            _readThread?.Abort();
            _session = null;
        }
        #endregion

        //Play
        #region
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                dynamic request = new
                {
                    Action = "song/play"
                };
                _session.Send(JsonConvert.SerializeObject(request));
            } else
            {
                MessageBox.Show("Not connected to server");
            }
        }
        #endregion

        //Pause
        #region
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                dynamic request = new
                {
                    Action = "song/pause"
                };
                _session.Send(JsonConvert.SerializeObject(request));
            }
            else
            {
                MessageBox.Show("Not connected to server");
            }
        }
        #endregion

        //Stop
        #region
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                dynamic request = new
                {
                    Action = "song/stop"
                };
                _session.Send(JsonConvert.SerializeObject(request));
            }
            else
            {
                MessageBox.Show("Not connected to server");
            }
        }
        #endregion

        //Next
        #region
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                dynamic request = new
                {
                    Action = "song/next"
                };
                _session.Send(JsonConvert.SerializeObject(request));
            }
            else
            {
                MessageBox.Show("Not connected to server");
            }
        }
        #endregion

        //Previous
        #region
        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                dynamic request = new
                {
                    Action = "song/previous"
                };
                _session.Send(JsonConvert.SerializeObject(request));
            } else
            {
                MessageBox.Show("Not connected to server");
            }
        }
        #endregion

        //Refresh song list
        #region
        private void refreshSongListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session != null)
            {
                dynamic request = new
                {
                    Action = "playlist/allsongs"
                };
                _session.Send(JsonConvert.SerializeObject(request));
            }
            else
            {
                MessageBox.Show("Not connected to server");
            }
        }
        #endregion

        private void toevoegenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void importerenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exporterenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
