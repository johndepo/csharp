using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;


namespace SocketMonitor
{

	public partial class SocketMonitor : Form
	{
		int m_portNumber = 0;
		//static bool m_runThreads = false;
		bool m_socketListenerStarted;
		TcpListener m_tcpListener;
		private Thread m_listenThread;

		//[STAThread]
		public SocketMonitor()
		{
			InitializeComponent();
			m_socketListenerStarted = false;
		}

        /// <summary>
        /// Allows the Socket Monitor to register a client connecting to the listening port
        /// </summary>
        /// <param name="temp"></param>
        private void UpdateStatus(string temp)
        {
            lblSocketStatus.Text = "Client Connected";
            lblSocketStatus.ForeColor = System.Drawing.Color.Green;
        }
        public delegate void UpdateTextCallBack(string testString);

		private void buttonStartStop_Click( object sender, EventArgs e )
		{
			if (m_socketListenerStarted == false)
			{
				try
				{
					m_portNumber = Convert.ToInt32( textBoxPortNumber.Text );
					m_tcpListener = new TcpListener( IPAddress.Any, m_portNumber );
                    lblSocketStatus.Text = "Listening, awaiting client connection";
                    lblSocketStatus.ForeColor = System.Drawing.Color.Orange;
				}
				catch
				{
                    lblSocketStatus.Text = "Listen Failed";
                    lblSocketStatus.ForeColor = System.Drawing.Color.Red;
					return;
				}
                this.m_listenThread = new Thread( new ThreadStart( ListenForClients ) );
				this.m_listenThread.Start();

				buttonStartStop.Text = "Stop Socket Listener";
				m_socketListenerStarted = true;
			}
			else
			{
				m_socketListenerStarted = false;

				m_tcpListener.Stop();
                lblSocketStatus.Text = "Not Listening";
                lblSocketStatus.ForeColor = System.Drawing.Color.Black;
				buttonStartStop.Text = "Start Socket Listener";
			}
		}

		private void ListenForClients()
		{
            m_tcpListener.Start();
            TcpClient client = null;

            while (m_socketListenerStarted)
            {
                //blocks until a client has connected to the server
                try
                {
                    client = this.m_tcpListener.AcceptTcpClient(); //wait for client to connect
                }
                catch
                {
                    break;
                }

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.SetApartmentState(ApartmentState.STA);
                clientThread.Start(client);
                Thread.Sleep(10);
            }
		}

		private void HandleClientComm( object client )
		{
			ConnectedClientOutput clientOutputDlg = new ConnectedClientOutput( client, m_portNumber );
            lblSocketStatus.Invoke(new UpdateTextCallBack(this.UpdateStatus), new object[] {""});
			clientOutputDlg.Show();
			Application.Run();
		}

		private void SocketMonitor_FormClosing( object sender, FormClosingEventArgs e )
		{
			m_socketListenerStarted = false;
			m_tcpListener.Stop();
            lblSocketStatus.Text = "Port Closed";
            lblSocketStatus.ForeColor = System.Drawing.Color.Black;
		}

		private void SocketMonitor_FormClosed( object sender, FormClosedEventArgs e ) //cannot enter when no initial port opened
		{
			Application.Exit();
		}
	}
}
