using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Xml;

namespace Exercise3.Models
{
    public class FileManager
    {
        private const string FILE_PATH = "~/App_Data/{0}.xml";
        private readonly DataManager dataManager;

        public bool IsLoaded { get; private set; }

        #region Singleton
        private static FileManager m_instance = null;
        public static FileManager Instance()
        {
            if (m_instance == null)
            {
                m_instance = new FileManager();
            }
            return m_instance;
        }
        private FileManager()
        {
            this.dataManager = DataManager.Instance();
            //this.points = new List<Point>();
            //this.rudderValues = new List<string>();
            //this.throttleValues = new List<string>();
            IsLoaded = false;
        }
        #endregion

        public void SaveData(string fileName)
        {
            // Correcting for semi-invalid file name
            if (fileName.EndsWith(".xml"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }

            // Getting the relative path to the file
            string path = HttpContext.Current.Server.MapPath(String.Format(FILE_PATH, fileName));
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true
            };

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Data");       // Root element
                int index = 0;
                List<Point> points = this.dataManager.GetPoints();
                List<string> rudderValues = this.dataManager.GetRudderValues();
                List<String> throttleValues = this.dataManager.GetThrottleValues();

                // Iterate over all the points and save them to the file. Save any additional values if exist
                while (index < points.Count)
                {
                    points[index].ToXml(writer);

                    if (rudderValues.Count > 0 && index < rudderValues.Count)
                    {
                        writer.WriteStartElement("rudder");
                        writer.WriteValue(rudderValues[index]);
                        writer.WriteEndElement();
                    }
                    if (throttleValues.Count > 0 && index < throttleValues.Count)
                    {
                        writer.WriteStartElement("throttle");
                        writer.WriteValue(throttleValues[index]);
                        writer.WriteEndElement();
                    }
                    index++;
                }
                writer.WriteEndElement();               // Close root element
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        public List<Point> LoadData(string fileName)
        {
            if (IsLoaded)
            {
                return this.dataManager.GetPoints();
            }

            // Correcting for semi-invalid file name
            if (fileName.EndsWith(".xml"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }

            // Getting the relative path to the file
            string path = HttpContext.Current.Server.MapPath(String.Format(FILE_PATH, fileName));
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
                return null;
            }

            // Loading only the 'point' nodes from the file
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.Name.Equals("Point") || node.Name.Equals("point"))
                {
                    // Converting to float in order to remove any white spaces
                    float lon = float.Parse(node.ChildNodes.Item(0).InnerXml);
                    float lat = float.Parse(node.ChildNodes.Item(1).InnerXml);
                    Point p = new Point(lon, lat);
                    this.dataManager.AddPoint(p);
                }
            }
            IsLoaded = true;
            return this.dataManager.GetPoints();
        }
    }
}