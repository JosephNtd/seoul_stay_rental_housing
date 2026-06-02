using DAL;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_ItemPrices
    {
        DAL_ItemPrices _dal = new DAL_ItemPrices();

        public const decimal MIN_PRICE = 1m;          // tối thiểu 1$
        public const decimal MAX_PRICE = 99999999m;    // tối đa 1 triệu $

        public List<ET_ItemPrices> GetPrices(long itemId) => _dal.GetPrices(itemId);

        public bool SetPrice(long itemId, DateTime date, decimal price, long policyId)
            => _dal.SetPrice(itemId, date, price, policyId);

        public bool DeletePrice(long itemId, DateTime date) => _dal.DeletePrice(itemId, date);

        public List<ET_CancellationPolicies> GetPolicies() => _dal.GetPolicies();

        public bool ToggleAvailability(long itemId, DateTime date, bool isAvailable)
        {
            return _dal.ToggleAvailability(itemId, date, isAvailable);
        }

        public List<DateTime> GetUnavailableDates(long itemId, DateTime from, DateTime to)
            => _dal.GetUnavailableDates(itemId, from, to);

        public List<DateTime> GetBookedDates(long itemId, DateTime from, DateTime to)
            => _dal.GetBookedDates(itemId, from, to);

        public bool SetAvailability(long itemId, DateTime date, bool isAvailable)
            => _dal.SetAvailability(itemId, date, isAvailable);
        public List<ET_ItemPrices> GetPricesInRange(long itemId, DateTime checkIn, DateTime checkOut)
        {
            return GetPrices(itemId)
                .Where(x =>
                    x.Date >= checkIn.Date &&
                    x.Date < checkOut.Date)
                .OrderBy(x => x.Date)
                .ToList();
        }
        public bool ValidatePriceUpdate(long itemId, List<DateTime> dates, decimal? price, long policyId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (dates == null || dates.Count == 0)
            {
                errorMessage = "No dates selected.";
                return false;
            }

            // Kiểm tra giá nếu có
            if (price.HasValue)
            {
                if (price.Value < MIN_PRICE)
                {
                    errorMessage = $"Price must be at least {MIN_PRICE:C}.";
                    return false;
                }
                if (price.Value > MAX_PRICE)
                {
                    errorMessage = $"Price cannot exceed {MAX_PRICE:C}.";
                    return false;
                }
            }

            // Không sửa ngày quá khứ
            DateTime today = DateTime.Today;
            var pastDates = dates.Where(d => d < today).ToList();
            if (pastDates.Any())
            {
                errorMessage = $"Cannot update for past date(s): {string.Join(", ", pastDates.OrderBy(d => d).Select(d => d.ToString("yyyy-MM-dd")))}.";
                return false;
            }

            // Không sửa ngày blocked
            var blocked = _dal.GetBlockedDatesInSet(itemId, dates);
            if (blocked.Any())
            {
                errorMessage = $"Cannot update for blocked date(s): {string.Join(", ", blocked.OrderBy(d => d).Select(d => d.ToString("yyyy-MM-dd")))}.";
                return false;
            }

            // Không sửa ngày booked
            var booked = _dal.GetBookedDatesInSet(itemId, dates);
            if (booked.Any())
            {
                errorMessage = $"Cannot update for booked date(s): {string.Join(", ", booked.OrderBy(d => d).Select(d => d.ToString("yyyy-MM-dd")))}.";
                return false;
            }

            // Kiểm tra policy hợp lệ
            var policies = GetPolicies();
            if (policies.All(p => p.ID != policyId))
            {
                errorMessage = "Invalid cancellation policy.";
                return false;
            }

            return true;
        }
    }
}
