using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoolScrapper
{
    public class GeoCoordinate
    {
        private double p1;
        private double p2;

        public GeoCoordinate(double p1, double p2)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
        }

        public GeoCoordinate()
        {
            // TODO: Complete member initialization
        }
        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}
