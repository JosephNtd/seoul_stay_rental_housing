using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_KpiCard
    {
        public decimal TotalRevenue { get; set; }

        public int TotalBookings { get; set; }

        public decimal AverageBookingValue { get; set; }

        public decimal OccupancyRate { get; set; }
    }
}
