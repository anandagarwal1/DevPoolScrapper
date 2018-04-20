using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoolScrapper
{
    public class GeoCoordinateMap
    {
        public double p1;
        public double p2;

        public GeoCoordinateMap(double p1, double p2)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
        }

        public GeoCoordinateMap()
        {
            // TODO: Complete member initialization
        }
        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}
