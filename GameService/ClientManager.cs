using SharedLibrary;
using SharedLibrary.Interfaces.Webservices;
using System;
using System.Collections.Generic;
using System.Threading;
namespace GameService
{
    internal static class ClientManager
    {
        private static Dictionary<Guid, KnownClient> RegisteredTokens = new Dictionary<Guid, KnownClient>();
        private static Random gRandom = new Random();

        static ClientManager()
        {
            ThreadPool.QueueUserWorkItem(ClientManager.ValidateClientConnections);
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

                //Determine which Clients are Active/In-Active
                foreach (KnownClient vClient in RegisteredTokens.Values)
                {
                    ThreadPool.QueueUserWorkItem(ValidateClientConnection,vClient);
                }

                Thread.Sleep(10000);
            }
        }

        private static void ValidateClientConnection(object pObjectState)
        {
            KnownClient vClient = pObjectState as KnownClient;
            try
            {
                if (vClient.ClientCallBack.IsClientAlive())
                {
                    //Console.WriteLine("Client is active: " + vClient.ClientToken.Token);
                }
            }
            catch
            {
                Console.WriteLine("Disconnecting user: " + vClient.ClientToken.Token);
                RegisteredTokens.Remove(vClient.ClientToken.Token);
            }
        }

        internal static void RegisterClientToken(ClientToken pClientToken, IGameWcfServiceCallback pCallback)
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

        internal static void UnregisterClientToken(ClientToken pClientToken)
        {
            RegisteredTokens.Remove(pClientToken.Token);
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