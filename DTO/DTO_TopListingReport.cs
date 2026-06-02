using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_TopListingReport
    {
        public long ItemID { get; set; }

        public string Title { get; set; }

        public decimal Revenue { get; set; }

        public int BookingCount { get; set; }
    }
}
