using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient
{
    class Program
    {
        private static GameWcfServiceClient vClient;
        private static ClientToken vMyToken;

        private static bool gShouldLoop = true;
        static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Console.WriteLine("Initializing Callback handler");
            InstanceContext instanceContext = new InstanceContext(new CallbackHandler());

            Console.WriteLine("Creating Client connection to Service");
            vClient = new GameWcfServiceClient(instanceContext);

            Console.WriteLine("Is the Server alive?");
            Console.WriteLine("Response: " + vClient.isAlive());
            vMyToken = vClient.RegisterClient();
            ClientDetails vDetails = new ClientDetails();
            Random vRandom = new Random();
            vDetails.Id = vRandom.Next(10000);
            Console.WriteLine("My Client ID=" + vDetails.Id);
            int vCount = 0;
            while (gShouldLoop)
            {
                vDetails.HP = vRandom.Next(100);
                vClient.SendClientDetails(vMyToken, vDetails);

                if (vCount % 10 == 0) Console.WriteLine("Still running: " + vCount);

                vCount++;
                Thread.Sleep(100);
            }

            vClient.Close();
            Console.WriteLine("Reading...");
            Console.ReadLine();
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            gShouldLoop = false;
            vClient.UnregisterClient(vMyToken);
            Thread.Sleep(1000);
            vClient.Close();
            //switch (sig)
            //{
            //    case CtrlType.CTRL_C_EVENT:
            //    case CtrlType.CTRL_LOGOFF_EVENT:
            //    case CtrlType.CTRL_SHUTDOWN_EVENT:
            //    case CtrlType.CTRL_CLOSE_EVENT:
            //    default:
            //}
            return false;
        }
    }

    public class CallbackHandler : IGameWcfServiceCallback
    {
        public void UpdateClient(ClientDetails pClientDetails)
        {
            Console.WriteLine("I recieved an update from" + pClientDetails.Id + " HP= " + pClientDetails.HP);
        }

        public bool IsClientAlive()
        {
            Console.WriteLine("I'm being pinged by the server");
            return true;
        }
    }
}
