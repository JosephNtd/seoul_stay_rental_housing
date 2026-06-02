using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_AreaOverview
    {
        public long ID { get; set; }
        public string AreaName { get; set; }
        public int TotalListings { get; set; }
        public int TotalAttractions { get; set; }
        public int TotalItems { get; set; }
        public int TotalAmenities { get; set; }
    }
}
