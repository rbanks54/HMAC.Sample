using System;

namespace HMAC.Authorization
{
    public class MessageRepresentation
    {
        public string Representation { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
    }
}