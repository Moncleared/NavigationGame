using SharedLibrary;
using SharedLibrary.Interfaces.Webservices;
using System;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace GameWcfService
{
    internal static class ClientManager
    {
        private static Logger gLogger = LogManager.GetLogger("ClientManager");
        internal static Dictionary<Guid, Client> RegisteredClients = new Dictionary<Guid, Client>();
        private static Random gRandom = new Random();
        private static object gLock = new object();

        static ClientManager()
        {
            Thread vPollClientsThread = new Thread(ClientManager.ValidateClientConnections);
            vPollClientsThread.Start();
        }

        public static void UpdateClientDetails(ClientToken pClientToken, ClientDetails pClientDetails)
        {
            RegisteredClients[pClientToken.Token].Details = pClientDetails;
        }

        private static void ValidateClientConnections(object pObjectState)
        {
            while (true)
            {
                List<Client> vClientsToRemove = new List<Client>();
                List<Client> vConnectedClients;
                lock (gLock)
                {
                    vConnectedClients = new List<Client>(RegisteredClients.Values);
                }

                //Determine which Clients are Active/In-Active
                foreach (Client vClient in vConnectedClients)
                {
                    ThreadPool.QueueUserWorkItem(ValidateClientConnection,vClient);
                }

                Thread.Sleep(30000);
            }
        }

        private static void ValidateClientConnection(object pObjectState)
        {
            Client vClient = pObjectState as Client;
            try
            {
                if (vClient.Callback.IsClientAlive())
                {
                    gLogger.Trace("Client is active: " + vClient.Token.Token);
                }
            }
            catch
            {
                gLogger.Trace("Disconnecting user: " + vClient.Token.Token);
                lock (gLock)
                {
                    RegisteredClients.Remove(vClient.Token.Token);
                }
            }
        }

        internal static void RegisterClientToken(ClientToken pClientToken, IGameWcfServiceCallback pCallback)
        {
            lock (gLock)
            {
                if (RegisteredClients.ContainsKey(pClientToken.Token))
                {
                    RegisteredClients[pClientToken.Token] = new Client(pClientToken, pCallback);
                }
                else
                {
                    RegisteredClients.Add(pClientToken.Token, new Client(pClientToken, pCallback));
                }
            }
            gLogger.Trace("Client Registered: " + pClientToken.Token);
        }

        internal static void UnregisterClientToken(ClientToken pClientToken)
        {
            lock (gLock)
            {
                RegisteredClients.Remove(pClientToken.Token);
            }
            gLogger.Trace("Client Unregistered: " + pClientToken.Token);
        }

        internal static bool IsClientTokenRegistered(ClientToken pClientToken)
        {
            return RegisteredClients.ContainsKey(pClientToken.Token);
        }

        internal static IEnumerable<Client> ClientsCloseBy()
        {
            return RegisteredClients.Values;
        }
    }

    internal class Client
    {
        internal Client(ClientToken pToken, IGameWcfServiceCallback pCallBack)
        {
            Token = pToken;
            Callback = pCallBack;
            Details = new ClientDetails();
        }
        internal ClientToken Token { get; set; }
        internal IGameWcfServiceCallback Callback { get; set; }
        internal ClientDetails Details { get; set; }
    }
}