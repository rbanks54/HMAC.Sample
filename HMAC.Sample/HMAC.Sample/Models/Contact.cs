using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMAC.Sample.Models
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        //Pretend this field was added as a new property after we built the v1 API
        public Address Address { get; set; }
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
    }

    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }
}