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

            //This Method is used to store Master Draw (Pool List) into Sheet
            //PoolHandler.GetPoolData();

            //This method is used to store Field Metrix in sheet
            PoolHandler.GetFieldMatrix();
        }
    }
}
