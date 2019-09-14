using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Exercise3.Models
{
    public class Point
    {
        public float Lon { get; set; }
        public float Lat { get; set; }

        public Point(string lon, string lat)
        {
            this.Lon = float.Parse(lon);
            this.Lat = float.Parse(lat);
        }

        public Point(float lon, float lat)
        {
            this.Lon = lon;
            this.Lat = lat;
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("point");
            writer.WriteElementString("lon", this.Lon.ToString());
            writer.WriteElementString("lat", this.Lat.ToString());
            writer.WriteEndElement();
        }
    }
}