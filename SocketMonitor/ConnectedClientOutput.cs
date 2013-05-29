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
	public partial class ConnectedClientOutput : Form
	{
		TcpClient tcpClient;
		NetworkStream clientStream;

		public ConnectedClientOutput( object client, int portNumber )
		{
			InitializeComponent();
			tcpClient = (TcpClient) client;
			clientStream = tcpClient.GetStream();
			labelPortNumber.Text = portNumber.ToString();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			int bytesRead;
			bytesRead = 0;
			byte[] message = new byte[4096];
			try
			{
				if (clientStream.DataAvailable)
				{
					//blocks until a client sends a message
					bytesRead = clientStream.Read( message, 0, 4096 );
					if (bytesRead == 0)
					{
						//the client has disconnected from the server
						Close();
					}

					//message has successfully been received
					ASCIIEncoding encoder = new ASCIIEncoding();
					string strOutput = encoder.GetString( message, 0, bytesRead ) + "\n";
					textBoxOutput.Text += strOutput;
					textBoxOutput.SelectionStart = textBoxOutput.TextLength - 1;
					textBoxOutput.ScrollToCaret();
				}
			}
			catch
			{
				//a socket error has occured                
				Close();
			}
		}

        private void buttonSaveLog_Click(object sender, EventArgs e)
        {
            DialogResult result = new DialogResult();

            string selectFilePath = "";

            folderBrowserDialogLog.Description = "Select the location for the log to be saved:";

            result = folderBrowserDialogLog.ShowDialog(); //display dialog

            if (result == DialogResult.OK)
            {
                selectFilePath = folderBrowserDialogLog.SelectedPath;
                string logFileName = generateLogName();
                string logFileText = grabLogFile();
                saveLogFile(selectFilePath, logFileName, logFileText);
            }

            if (result == DialogResult.No)
            {
                //ToDo: display "Log Save Cancelled"
            }
        }

        /// <summary>
        /// Returns the entire text from the multiline textbox
        /// </summary>
        /// <returns></returns>
        private string grabLogFile()
        {
            string logText = textBoxOutput.Text;
            return logText;
        }

        /// <summary>
        /// Creates a name based on the current time on pc
        /// </summary>
        /// <returns></returns>
        private string generateLogName()
        {
            string name = "SocketMonitor Log " + DateTime.UtcNow.ToString("u").Replace(':', '-');
            return name;
        }


        /// <summary>
        /// Saves a provided text file to a specified location and name
        /// </summary>
        /// <param name="filePath">Directory of where the file is stored as a string</param>
        /// <param name="fileName">Name of the file as a string</param>
        /// <param name="fileText">Contents of the text file as a string</param>
        /// 
        private void saveLogFile(string filePath, string fileName, string fileText)
        {
            string documentPath = filePath + @"\" + fileName + ".txt";

            using (StreamWriter outfile = new StreamWriter(documentPath, true)) //mydocpath + @"\UserInputFile.txt"
            {
                outfile.Write(fileText.ToString()); //ToString not necessary
            }

            MessageBox.Show("Log File Created.");
        }

        private void buttonResetOutput_Click(object sender, EventArgs e)
        {
            textBoxOutput.Clear();
        }
	}

}
