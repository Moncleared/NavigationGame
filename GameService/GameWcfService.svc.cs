using SharedLibrary;
using SharedLibrary.Interfaces.Webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace GameService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class GameWcfService : IGameWcfService
    {
        public bool isAlive()
        {
            Callback.ServerSays("Client is asking if I'm Alive...");
            return true;
        }

        public ClientToken RegisterClient()
        {
            ClientToken vClientToken = new ClientToken();
            ClientManager.RegisterClientToken(vClientToken, this.Callback);
            return vClientToken;
        }

        public void SendClientDetails(ClientToken pClientToken, ClientDetails pClientDetails)
        {
            if (ClientManager.IsClientTokenRegistered(pClientToken))
            {
                ClientManager.UpdateClientDetails(pClientToken, pClientDetails);
                //Console.WriteLine("Client Details recieved: " + pClientDetails.ToString());
            }
            else
            {
                Console.WriteLine("Rejecting client due to not being registered");
            }
        }
        public List<ClientDetails> GetClientsNearMe(ClientToken pClientToken, ClientDetails pClientDetails)
        {
            List<ClientDetails> vNearByClients = new List<ClientDetails>();
            if (ClientManager.IsClientTokenRegistered(pClientToken))
            {
                foreach (KnownClient vKnownClient in ClientManager.ClientsCloseBy())
                {
                    vNearByClients.Add(vKnownClient.ClientDetails);
                }
            }
            else
            {
                Console.WriteLine("Rejecting client due to not being registered");
            }
            return vNearByClients;
        }

        IGameWcfServiceCallback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IGameWcfServiceCallback>();
            }
        }
    }
}
