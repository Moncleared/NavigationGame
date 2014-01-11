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

        static ClientManager()
        {
            ThreadPool.QueueUserWorkItem(ClientManager.ValidateClientConnection);
        }

        private static void ValidateClientConnection(object pObjectState)
        {
            while (true)
            {
                List<KnownClient> vClientsToRemove = new List<KnownClient>();

                //Determine which Clients are Active/In-Active
                foreach (KnownClient vClient in RegisteredTokens.Values)
                {
                    try
                    {
                        if (vClient.ClientCallBack.IsClientAlive())
                        {
                            Console.WriteLine("Client is active: " + vClient.ClientToken);
                        }
                    }
                    catch (Exception vException)
                    {
                        Console.WriteLine("Disconnecting user: " + vClient.ClientToken.Token + " for " + vException.Message);
                        vClientsToRemove.Add(vClient); 
                    }
                }

                //Remove the In-Active Clients from the Dictionary safely
                foreach (KnownClient vClient in vClientsToRemove)
                {
                    RegisteredTokens.Remove(vClient.ClientToken.Token);
                }

                Thread.Sleep(10000);
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
    }

    internal class KnownClient
    {
        internal KnownClient(ClientToken pClientToken, IGameWcfServiceCallback pClientCallBack)
        {
            ClientToken = pClientToken;
            ClientCallBack = pClientCallBack;
        }
        internal ClientToken ClientToken { get; set; }
        internal IGameWcfServiceCallback ClientCallBack { get; set; }
    }
}