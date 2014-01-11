namespace SharedLibrary.Interfaces.Webservices
{
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface IGameWcfServiceCallback
    {
        /// <summary>
        /// One way contract for sending an update to a connected/registered client
        /// </summary>
        /// <param name="pClientDetails"></param>
        [OperationContract(IsOneWay = true)]
        void UpdateClient(ClientDetails pClientDetails);

        /// <summary>
        /// Servers way to validating a client is still alive and responding
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool IsClientAlive();
    }
}
