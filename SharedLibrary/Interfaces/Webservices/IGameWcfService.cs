namespace SharedLibrary.Interfaces.Webservices
{
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IGameWcfServiceCallback))]
    public interface IGameWcfService
    {
        /// <summary>
        /// Just checks if the server is alive
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool isAlive();


        /// <summary>
        /// Registers the client, ensure you save the ClientToken otherwise you will not be able to send requests to the server without it
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ClientToken RegisterClient();

        /// <summary>
        /// Unregister the client
        /// </summary>
        /// <param name="pClientToken"></param>
        [OperationContract]
        void UnregisterClient(ClientToken pClientToken);

        /// <summary>
        /// Updates the Client Information to the Server
        /// </summary>
        /// <param name="pClientToken"></param>
        /// <param name="pClientDetails"></param>
        [OperationContract]
        void SendClientDetails(ClientToken pClientToken, ClientDetails pClientDetails);
    }
}
