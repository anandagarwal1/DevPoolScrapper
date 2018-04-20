using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PoolScrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new Service1() 
            //};
            //ServiceBase.Run(ServicesToRun);
            //PoolHandler.GetPoolData();

            GeoCoordinate gc = new GeoCoordinate();
            IList<GeoCoordinate> geoCoordinates = new List<GeoCoordinate>();
            //Field 1
            gc.Longitude = 153.0947427;
            gc.Latitude = -30.3223943;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.0946219;
            gc.Latitude = -30.3230217;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.095153;
            gc.Latitude = -30.3231027;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.0952764;
            gc.Latitude = -30.322473;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.0947427;
            gc.Latitude = -30.3223943;

            geoCoordinates.Add(gc);

            PoolHandler.GetPolygonCentroid(geoCoordinates);

            geoCoordinates = new List<GeoCoordinate>();
            gc = new GeoCoordinate();
            //Field 2
            gc.Longitude = 153.0954374;
            gc.Latitude = -30.3225077;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.0953247;
            gc.Latitude = -30.3231282;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.095837;
            gc.Latitude = -30.3232092;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.0959711;
            gc.Latitude = -30.3225864;

            geoCoordinates.Add(gc);

            gc = new GeoCoordinate();
            gc.Longitude = 153.0954374;
            gc.Latitude = -30.3225077;

            geoCoordinates.Add(gc);

            PoolHandler.GetPolygonCentroid(geoCoordinates);
        }
    }
}
