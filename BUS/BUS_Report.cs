using DAL;
using DTO;
using System.Collections.Generic;

namespace BUS
{
    public class BUS_Report
    {
        private readonly DAL_Report _dal =
            new DAL_Report();
        public DTO_BookingStats GetBookingStatsSafe()
        {
            try
            {
                return _dal.GetBookingStats();
            }
            catch
            {
                return new DTO_BookingStats();
            }
        }

        #region Dashboard

        public DTO_BookingStats GetBookingStats() => _dal.GetBookingStats();

        #endregion

        #region Revenue

        public List<DTO_RevenueTrend> GetRevenueTrend() => _dal.GetRevenueTrend();

        #endregion

        #region Booking Status

        public List<DTO_BookingStatusReport> GetBookingStatus() => _dal.GetBookingStatus();

        #endregion

        #region Listing Performance

        public List<DTO_TopListingReport> GetTopListings() => _dal.GetTopListings();

        #endregion

        #region Area Revenue

        public List<DTO_AreaRevenueReport> GetRevenueByArea() => _dal.GetRevenueByArea();


        #endregion
    }
}