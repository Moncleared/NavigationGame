namespace GameWcfService
{
    using System;
    using SharedLibrary;
    using SharedLibrary.Interfaces.Webservices;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using NLog;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class GameWcfService : IGameWcfService
    {
        private static Logger gLogger = LogManager.GetLogger("GameWcfService");

        public bool isAlive()
        {
            gLogger.Trace("Recieved request for isAlive, callback sent");
            return true;
        }

        public ClientToken RegisterClient()
        {
            ClientToken vClientToken = new ClientToken();
            ClientManager.RegisterClientToken(vClientToken, this.Callback);
            return vClientToken;
        }

        public void UnregisterClient(ClientToken pClientToken)
        {
            ClientManager.UnregisterClientToken(pClientToken);
        }

        private void NotifyClients(ClientDetails pClientDetails)
        {
            foreach(KnownClient vClient in ClientManager.ClientsCloseBy() ) {
                if (vClient.ClientDetails.Id != pClientDetails.Id)
                {
                    vClient.ClientCallBack.UpdateClient(pClientDetails);
                }
            }
        }

        public void SendClientDetails(ClientToken pClientToken, ClientDetails pClientDetails)
        {
            try
            {
                if (ClientManager.IsClientTokenRegistered(pClientToken))
                {
                    this.NotifyClients(pClientDetails);
                    ClientManager.UpdateClientDetails(pClientToken, pClientDetails);
                }
                else
                {
                    Console.WriteLine("Rejecting client due to not being registered");
                }
            }
            catch (Exception vException)
            {
                gLogger.ErrorException("Exception encountered in SendClientDetails=" + vException.Message, vException);
            }
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
