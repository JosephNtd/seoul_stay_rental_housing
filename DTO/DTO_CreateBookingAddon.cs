using System;

namespace DTO
{
    public class DTO_CreateBookingAddon
    {

        // SERVICE INFO

        public long ServiceID { get; set; }

        public string ServiceName { get; set; }

        public string ServiceTypeName { get; set; }

        // PRICING

        public decimal Price { get; set; }

        public long NumberOfPeople { get; set; } = 1;

        // SCHEDULE

        public DateTime FromDate { get; set; }

        // NOTES

        public string Notes { get; set; }

        // COMPUTED

        public decimal TotalPrice
        {
            get
            {
                return Price * NumberOfPeople;
            }
        }

        // DISPLAY HELPERS

        public string PriceDisplay
        {
            get
            {
                return $"{Price:N0} ₫";
            }
        }

        public string TotalPriceDisplay
        {
            get
            {
                return $"{TotalPrice:N0} ₫";
            }
        }

        public string FromDateDisplay
        {
            get
            {
                return FromDate.ToString("dd MMM yyyy");
            }
        }

        public string SummaryDisplay
        {
            get
            {
                return
                    $"{ServiceName} × {NumberOfPeople} pax = {TotalPriceDisplay}";
            }
        }
    }
}