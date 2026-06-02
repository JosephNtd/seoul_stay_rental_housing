using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_UserDisplay
    {
        public long UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Administrator", "Host", "Guest"
        public string Status { get; set; }
        public string LastActive { get; set; } // Chuỗi hiển thị như "2 mins ago", "Yesterday"...
        public string Country { get; set; }

        public int TotalBookings { get; set; }
    }
}
