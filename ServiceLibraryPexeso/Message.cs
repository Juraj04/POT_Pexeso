using System;
using System.Runtime.Serialization;

namespace ServiceLibraryPexeso
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Receiver { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string Content { get; set; }

        public override string ToString()
        {
            return "\r\n" + Date + "\r\n" + Sender + "\r\n" + Content + "\r\n";
        }
    }
}
