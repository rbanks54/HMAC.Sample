using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMAC.Sample.Models
{
    /*
     * These would have been the previous definitions of the ViewModel.
     * Shows how the model would have evolved over time.
     * 
     * The _v1_x extension is just for reference so we know what it looked like at that point in time
     * 
    public class ContactViewModel_v1
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ContactViewModel_v1_1
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
     */

    public class ContactViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; } //should use a viewModel here too, in case Address changes. Just keeping it simple for now.
    }

    public class ContactViewModel_v2
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
    }
}