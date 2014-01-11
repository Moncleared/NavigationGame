using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost host = new WebServiceHost(typeof(GameWcfService));
            try
            {
                host.Open();
                Console.WriteLine("Service is running: " + host.BaseAddresses[0].AbsoluteUri);
                Console.WriteLine("Press enter to quit...");
                Console.ReadLine();
                host.Close();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
                Console.ReadLine();
                host.Abort();
            }
        }
    }
}
