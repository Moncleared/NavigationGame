namespace SharedLibrary
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [DataContract]
    public class ClientToken
    {
        public ClientToken()
        {
            this.Token = Guid.NewGuid();
        }
        
        /// <summary>
        /// Token is assigned by the Server to validate the client
        /// </summary>
        [DataMember]
        public Guid Token { get; set; }
    }
}
