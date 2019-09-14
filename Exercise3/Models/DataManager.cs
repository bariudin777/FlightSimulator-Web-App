using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exercise3.Models
{
    public class DataManager
    {
        private List<Point> points;
        private List<string> rudderValues;
        private List<string> throttleValues;

        public int Index { get; private set; }

        #region Singleton
        private static DataManager m_instance = null;
        public static DataManager Instance()
        {
            if (m_instance == null)
            {
                m_instance = new DataManager();
            }
            return m_instance;
        }

        private DataManager()
        {
            this.points = new List<Point>();
            this.rudderValues = new List<string>();
            this.throttleValues = new List<string>();
            Index = 0;
        }
        #endregion

        public Point GetNextPoint()
        {
            if (Index >= this.points.Count)
            {
                return null;
            }
            Point p = this.points.ElementAt(Index++);
            return p;
        }

        public void AddPointsFromCollection(IEnumerable<Point> points)
        {
            // Check if the lists have different points
            for (int i = 0; i < this.points.Count; i++)
            {
                Point p1 = this.points.ElementAt(i);
                Point p2 = this.points.ElementAt(i);
                if (p1.Lon == p2.Lon && p1.Lat == p2.Lat)
                {
                    return;
                }
            }
            // If not, add all the points to the list
            this.points.AddRange(points);
        }

        public void AddPoint(Point p)
        {
            points.Add(p);
        }

        public void AddRudderValue(string value)
        {
            this.rudderValues.Add(value);
        }

        public void AddThrottleValue(string value)
        {
            this.throttleValues.Add(value);
        }

        public List<Point> GetPoints()
        {
            return this.points;
        }

        public List<string> GetRudderValues()
        {
            return this.rudderValues;
        }

        public List<string> GetThrottleValues()
        {
            return this.throttleValues;
        }
    }
}