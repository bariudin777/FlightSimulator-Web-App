using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Net;
using Exercise3.Models;

namespace Exercise3.Controllers
{
    public class DisplayController : Controller
    {
        /// <summary>
        /// Redirect
        /// Redirects the url args by regex
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Redirect(string arg1, string arg2)
        {
            // If the first argument is in the form of an IP address
            Regex rx = new Regex("(\\d+\\.){3}\\d+");
            Match match = rx.Match(arg1);
            if (match.Success)
            {
                return SingleSample(arg1, Int32.Parse(arg2));
            }
            else
            {
                return DisplayFromFile(arg1, Int32.Parse(arg2));
            }
        }

        /// <summary>
        /// Mission 1 - SinglePoint Display
        /// Displays planes location
        /// </summary>
        /// <returns></returns>
        public ActionResult SingleSample(string ip, int port)
        {
            Client.Instance().Connect(ip, port);
            return View("SingleSample");
        }

        // mission 2
        public ActionResult DisplayInIntervals(string ip, int port, int frequency)
        {
            Session["frequency"] = frequency;
            Session["ip"] = ip;
            Session["port"] = port;
            Client.Instance().Connect(ip, port);
            return View("DisplayInIntervals");
        }

        /// <summary>
        /// SaveAndDispaly
        /// Mission 4- display the route and saves the args from simulator in xml
        /// </summary>
        /// <returns> ActionResult </returns>
        public ActionResult SaveAndDisplay(string ip, int port, int frequency, int duration, string fileName)
        {
            Client.Instance().Connect(ip, port);
            ViewBag.fileName = fileName;
            Session["frequency"] = frequency;
            Session["duration"] = duration;
            return View("SaveAndDisplay");
        }

        /// <summary>
        /// Mission 4 - Display path from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public ActionResult DisplayFromFile(string fileName, int frequency)
        {
            Session["frequency"] = frequency;

            DataManager dataManager = DataManager.Instance();
            FileManager fileManager = FileManager.Instance();

            List<Point> points = fileManager.LoadData(fileName);
            // If there was an error in reading the points or there aren't any, direct to an 'error' view
            if (points == null || points.Count == 0)
            {
                return View("DefaultMap");
            }
            // If there weren't any errors, add the points to the data base
            dataManager.AddPointsFromCollection(fileManager.LoadData(fileName));
            Point head = dataManager.GetNextPoint();
            ViewBag.lon = head.Lon;
            ViewBag.lat = head.Lat;
            return View("DisplayFromFile");
        }
        /// <summary>
        /// GetFlightData
        /// get bool var from view- define what mission to use
        /// </summary>
        /// <returns>string</returns>
        [HttpPost]
        public string GetFlightData(bool isGetAllParameters)
        {
            Client client = Client.Instance();
            return client.GetFlightData(isGetAllParameters);
        }
        /// <summary>
        /// Save Data
        /// saves the data in FileManager with file-name from url
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void SaveData(string fileName)
        {
            FileManager manager = FileManager.Instance();

            try
            {
                manager.SaveData(fileName);
            }
            catch (Exception e)
            {
                string error = e.ToString();
                Console.Write("didnt complite the save: {0}", error);
            }
        }

        /// <summary>
        /// GetPoint
        /// Gets the point from data-manager
        /// </summary>
        /// <returns> point </returns>
        public string GetPoint()
        {
            DataManager manager = DataManager.Instance();
            Point p = manager.GetNextPoint();
            return new JavaScriptSerializer().Serialize(p);
        }

        /// <summary>
        /// Default Display
        /// Display only the map
        /// </summary>
        /// <returns></returns>
        public ActionResult Default()
        {
            return View("DefaultMap");
        }
    }
}