namespace SharedLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class ClientDetails
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public List<Location> Locations {get; set;}

        [DataMember]
        public int HP { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}
