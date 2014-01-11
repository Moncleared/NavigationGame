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
            Callback.ServerSays("This is a test");
            return true;
        }

        public ClientToken RegisterClient()
        {
            ClientToken vClientToken = new ClientToken();
            ClientManager.RegisterClientToken(vClientToken, this.Callback);
            return vClientToken;
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
