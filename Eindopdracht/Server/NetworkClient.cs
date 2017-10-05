﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Server
{
    public class NetworkClient
    {
        TcpClient client;
        private readonly NetworkStream _stream;
        Thread readThread;



        public NetworkClient(TcpClient client)
        {
            this.client = client;
            _stream = client.GetStream();
            readThread = new Thread(Read);
            readThread.Start();
        }

        public void Close()
        {
            readThread.Abort();
            _stream.Close();
            client.Close();
        }

		//Send to client
		#region
		public void Send(string message)
		{
			System.Diagnostics.Debug.WriteLine("Send: \r\n" + message);
			byte[] prefixArray = BitConverter.GetBytes(message.Length);
			byte[] requestArray = System.Text.Encoding.Default.GetBytes(message);
			byte[] buffer = new Byte[prefixArray.Length + message.Length];
			prefixArray.CopyTo(buffer, 0);
			requestArray.CopyTo(buffer, prefixArray.Length);
			_stream.Write(buffer, 0, buffer.Length);
		}
		#endregion


        // Read from client
		#region
		public void Read()
		{
			while (true)
			{
				try
				{
                    StringBuilder response = new StringBuilder();
					int totalBytesreceived = 0;
					int lengthMessage = -1;
					byte[] receiveBuffer = new byte[1024];
					bool messagereceived = false;

					do
					{
						int numberOfBytesRead = _stream.Read(receiveBuffer, 0, receiveBuffer.Length);
						totalBytesreceived += numberOfBytesRead;
						string received = Encoding.ASCII.GetString(receiveBuffer, 0, numberOfBytesRead);
						response.AppendFormat("{0}", received);
						if (lengthMessage == -1)
						{
							if (receiveBuffer.Length >= 4)
							{
								Byte[] lengthMessageArray = new Byte[4];
								Array.Copy(receiveBuffer, 0, lengthMessageArray, 0, 3);
								lengthMessage = BitConverter.ToInt32(lengthMessageArray, 0);
								if ((totalBytesreceived - 4) == lengthMessage)
								{
									messagereceived = true;
								}
							}
						}
						else if ((totalBytesreceived - 4) == lengthMessage)
						{
							messagereceived = true;
						}
					}
					while (!messagereceived);
					_stream.Flush();
					string toReturn = response.ToString().Substring(4);
					System.Diagnostics.Debug.WriteLine("Received: \r\n" + toReturn);
                    ProcesAnswer(JsonConvert.DeserializeObject(toReturn));
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.Message);
				}
			}
		}
        #endregion

        //Process the request from client
        #region

        public void ProcesAnswer(dynamic jsonData)
        {
            if (jsonData.Action == "song/play")
            {
                Program.RecievedPlay();
            }
            else if (jsonData.Action == "song/pause")
            {
                Program.RecievedPause();
            }
            else if (jsonData.Action == "song/stop")
            {
                Program.RecievedStop();
            }
            else if (jsonData.Action == "song/next")
            {
                Program.RecievedNext();
            }
            else if (jsonData.Action == "song/previous")
            {
                Program.RecievedPrevious();
            }
            else if (jsonData.Action == "playlist/allsongs")
            {
                dynamic answer = new
                {
                    Action = "playlist/allsongs",
                    data = new
                    {
                        songs = MusicLibrary.songs.ToArray()
                    }
                };
                Send(JsonConvert.SerializeObject(answer));
            } else if (jsonData.Action == "play/selectedsong")
            {
                if (MusicLibrary.ContainsSong((string) jsonData.data.song))
                {
                    Program.PlaySelectedSong((string) jsonData.data.song);
                    Program.RecievedPlay();
                }
            }
            else if (jsonData.Action == "disconnect")
            {
                Close();
                Program.CloseClient(this);
            }
        }

        #endregion

        }
    }
