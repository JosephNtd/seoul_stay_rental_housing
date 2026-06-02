using System;

namespace DTO
{
    public class DTO_CreateBookingNight
    {
        public DateTime Date { get; set; }

        public long ItemPriceID { get; set; }

        public decimal Price { get; set; }

        public long CancellationPolicyID { get; set; }

        public string PolicyName { get; set; }

        public string DateDisplay
        {
            get
            {
                return
                    Date.ToString(
                        "dd MMM yyyy");
            }
        }

        public string PriceDisplay
        {
            get
            {
                return
                    $"{Price:N0} ₫";
            }
        }
    }
}