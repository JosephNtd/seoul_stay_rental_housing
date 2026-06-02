using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_GuestLookup
    {
        public long UserID { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Country { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{FullName} - {PhoneNumber}";
            }
        }
    }
}
