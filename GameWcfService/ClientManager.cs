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
        private static Dictionary<Guid, KnownClient> RegisteredTokens = new Dictionary<Guid, KnownClient>();
        private static Random gRandom = new Random();
        private static object gLock = new object();

        static ClientManager()
        {
            Thread vPollClientsThread = new Thread(ClientManager.ValidateClientConnections);
            vPollClientsThread.Start();
        }

        public static void UpdateClientDetails(ClientToken pClientToken, ClientDetails pClientDetails)
        {
            RegisteredTokens[pClientToken.Token].ClientDetails = pClientDetails;
        }

        private static void ValidateClientConnections(object pObjectState)
        {
            while (true)
            {
                List<KnownClient> vClientsToRemove = new List<KnownClient>();
                List<KnownClient> vConnectedClients;
                lock (gLock)
                {
                    vConnectedClients = new List<KnownClient>(RegisteredTokens.Values);
                }

                //Determine which Clients are Active/In-Active
                foreach (KnownClient vClient in vConnectedClients)
                {
                    ThreadPool.QueueUserWorkItem(ValidateClientConnection,vClient);
                }

                Thread.Sleep(30000);
            }
        }

        private static void ValidateClientConnection(object pObjectState)
        {
            KnownClient vClient = pObjectState as KnownClient;
            try
            {
                if (vClient.ClientCallBack.IsClientAlive())
                {
                    gLogger.Trace("Client is active: " + vClient.ClientToken.Token);
                }
            }
            catch
            {
                gLogger.Trace("Disconnecting user: " + vClient.ClientToken.Token);
                lock (gLock)
                {
                    RegisteredTokens.Remove(vClient.ClientToken.Token);
                }
            }
        }

        internal static void RegisterClientToken(ClientToken pClientToken, IGameWcfServiceCallback pCallback)
        {
            lock (gLock)
            {
                if (RegisteredTokens.ContainsKey(pClientToken.Token))
                {
                    RegisteredTokens[pClientToken.Token] = new KnownClient(pClientToken, pCallback);
                }
                else
                {
                    RegisteredTokens.Add(pClientToken.Token, new KnownClient(pClientToken, pCallback));
                }
            }
            gLogger.Trace("Client Registered: " + pClientToken.Token);
        }

        internal static void UnregisterClientToken(ClientToken pClientToken)
        {
            lock (gLock)
            {
                RegisteredTokens.Remove(pClientToken.Token);
            }
        }

        internal static bool IsClientTokenRegistered(ClientToken pClientToken)
        {
            return RegisteredTokens.ContainsKey(pClientToken.Token);
        }

        internal static IEnumerable<KnownClient> ClientsCloseBy()
        {
            return RegisteredTokens.Values;
        }
    }

    internal class KnownClient
    {
        internal KnownClient(ClientToken pClientToken, IGameWcfServiceCallback pClientCallBack)
        {
            ClientToken = pClientToken;
            ClientCallBack = pClientCallBack;
            ClientDetails = new ClientDetails();
        }
        internal ClientToken ClientToken { get; set; }
        internal IGameWcfServiceCallback ClientCallBack { get; set; }
        internal ClientDetails ClientDetails { get; set; }
    }
}