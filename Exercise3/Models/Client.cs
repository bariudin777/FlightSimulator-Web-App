using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Exercise3.Models
{
    public class Client
    {
        private string connectedIP;
        private int connectedPort;

        //private TcpListener server;
        private TcpClient client;
        //private StreamReader reader;
        //private StreamWriter writer;
        private NetworkStream stream;
        //private StringBuilder sb;
        //private XmlWriter xmlWriter;
        private bool isConnected;
        private readonly Dictionary<string, string> paths;
        private readonly DataManager dataManager;


        #region Singleton
        private static Client m_instance = null;

        public static Client Instance()
        {
            if (m_instance == null)
            {
                m_instance = new Client();
            }
            return m_instance;
        }
        private Client()
        {
            isConnected = false;
            this.dataManager = DataManager.Instance();
            paths = new Dictionary<string, string>
            {
                { "LON", "/position/longitude-deg" },
                { "LAT", "/position/latitude-deg" },
                { "RUDDER", "controls/flight/rudder" },
                { "THROTTLE", "controls/engines/current-ending/throttle" }
            };
        }
        #endregion

        public bool Connect(string ip, int port)
        {
            if (isConnected)
            {
                // Check if the client is already connected to the requested address
                if (connectedIP == ip && connectedPort == port)
                {
                    return true;
                }
                // If not, disconnect from the current connection in order to establish the newly requested connection
                this.Disconnect();
            }

            // Establish connection
            try
            {
                this.client = new TcpClient();
                while (!this.client.Connected)
                {
                    try
                    {
                        this.client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    }
                    catch (Exception) { }
                }

                Debug.WriteLine("Connected!");
                this.stream = client.GetStream();
                isConnected = true;

                connectedIP = ip;
                connectedPort = port;
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception: {0}", e);
                Debug.WriteLine("Exception: {0}", e);
                isConnected = false;
            }
            return isConnected;
        }

        public string GetFlightData(bool isGetAllParameters)
        {
            string lon = this.GetValueFromPath(this.paths["LON"]);
            string lat = this.GetValueFromPath(this.paths["LAT"]);
            //string lon = rnd.Next(200, 1000).ToString();
            //string lat = rnd.Next(200, 800).ToString();
            Point p = new Point(lon, lat);
            this.dataManager.AddPoint(p);

            string rudder = null;
            string throttle = null;

            if (isGetAllParameters)
            {
                rudder = GetValueOf("rudder");
                throttle = GetValueOf("throttle");
            }
            return ToXml(p, rudder, throttle);
        }

        public string GetValueOf(string parameter)
        {
            // Correcting for semi-invalid input and checking if the key exists in the table
            parameter = parameter.ToUpper();
            if (parameter.Equals("LONTITUDE"))
            {
                parameter = "LON";
            }
            else if (parameter.Equals("LATITUDE"))
            {
                parameter = "LAT";
            }
            if (this.paths.ContainsKey(parameter))
            {
                return null;
            }
            string value = this.GetValueFromPath(this.paths[parameter]);
            if (parameter.Equals("RUDDER")) this.dataManager.AddRudderValue(value);
            if (parameter.Equals("THROTTLE")) this.dataManager.AddThrottleValue(value);
            return value;
        }

        private string GetValueFromPath(string path)
        {
            // Sending the command
            Byte[] msg = Encoding.ASCII.GetBytes("get " + path + "\r\n");
            this.stream.Write(msg, 0, msg.Length);

            // Storing the received output
            msg = new byte[256];
            Int32 length = this.stream.Read(msg, 0, msg.Length);
            string value = Encoding.ASCII.GetString(msg, 0, length);
            
            // Extracting the value
            Match match = new Regex("[-]?[0-9]+[.]?[0-9]+").Match(value);
            return value.Substring(match.Index, match.Length);
        }

        private string ToXml(Point p, string rudder, string throttle)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("data");       // root

                p.ToXml(writer);

                if (rudder != null)
                {
                    writer.WriteStartElement("rudder");
                    writer.WriteValue(rudder);
                    writer.WriteEndElement();
                }
                if (throttle != null)
                {
                    writer.WriteStartElement("throttle");
                    writer.WriteValue(throttle);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();              // close root
                writer.WriteEndDocument();
                writer.Flush();
                return sb.ToString();
            }
        }

        public void Disconnect()
        {
            //this.reader.Close();
            this.client.GetStream().Close();
            this.client.Close();
            this.isConnected = false;
        }
    }
}