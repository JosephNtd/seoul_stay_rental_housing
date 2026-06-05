using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class BUS_Booking
    {
        private readonly DAL_Booking _booking = new DAL_Booking();
        private readonly DAL_ItemPrices _prices = new DAL_ItemPrices();
        private readonly DAL_Items _dalItems = new DAL_Items();
        private readonly DAL_AddonService _addonDal = new DAL_AddonService();
        private readonly DAL_Services _serviceDal = new DAL_Services();

        public List<DTO_BookingCard> GetBookingCards()
        {
            return _booking.GetBookingCards();
        }

        public DTO_BookingDetails GetBookingDetails(long bookingId)
        {
            return _booking.GetBookingDetails(bookingId);
        }

        public List<DTO_BookingTimeline> GetBookingTimeline(long bookingId)
        {
            return _booking.GetBookingTimeline(bookingId);
        }

        public DTO_BookingStats GetBookingStats()
        {
            return _booking.GetBookingStats();
        }

        public List<DTO_BookingCard> SearchBookings(string keyword)
        {
            return _booking.SearchBookings(keyword);
        }

        public List<DTO_BookingCard> FilterBookings(string status)
        {
            return _booking.FilterBookings(status);
        }

        public decimal CalculateBookingPrice(long itemId, DateTime checkIn, DateTime checkOut)
        {
            return _prices.GetPrices(itemId).Where(x => x.Date >= checkIn.Date && x.Date < checkOut.Date).Sum(x => x.Price);
        }

        public bool ValidatePricing(long itemId, DateTime checkIn, DateTime checkOut, decimal expectedTotal)
        {
            decimal actual = CalculateBookingPrice(itemId, checkIn, checkOut);

            return actual == expectedTotal;
        }

        public bool ValidateAvailability(long itemId, DateTime checkIn, DateTime checkOut)
        {
            var blockedDates = _prices.GetUnavailableDates(itemId, checkIn, checkOut);
            return blockedDates.Count == 0;
        }

        public bool ValidateBookingConflict(long itemId, DateTime checkIn, DateTime checkOut)
        {
            var bookedDates = _prices.GetBookedDates(itemId, checkIn, checkOut);
            return bookedDates.Count == 0;
        }

        public bool ValidateDateOverlap(long itemId, DateTime checkIn, DateTime checkOut, long? ignoreBookingId = null)
        {
            var bookings = GetBookingCards().Where(x => x.ItemID == itemId && x.BookingStatus != "Cancelled" && x.BookingStatus != "Refunded");

            if (ignoreBookingId != null)
            {
                bookings = bookings.Where(x => x.BookingID != ignoreBookingId.Value);
            }

            return !bookings.Any(x => checkIn < x.CheckOutDate && checkOut > x.CheckInDate);
        }

        public bool ValidateBooking(long itemId, DateTime checkIn, DateTime checkOut, decimal expectedTotal, out string message)
        {
            message = "";

            if (checkIn.Date >= checkOut.Date)
            {
                message = "Check-out date must be after check-in date.";
                return false;
            }

            if (!ValidateAvailability(itemId, checkIn, checkOut))
            {
                message = "Listing is unavailable for selected dates.";
                return false;
            }

            if (!ValidateBookingConflict(itemId, checkIn, checkOut))
            {
                message = "Listing already has a booking in this period.";
                return false;
            }

            if (!ValidatePricing(itemId, checkIn, checkOut, expectedTotal))
            {
                message = "Pricing mismatch detected.";

                return false;
            }

            return true;
        }

        public bool CanConfirm(DTO_BookingCard booking)
        {
            if (booking == null)
                return false;

            return booking.BookingStatus == "Pending";
        }

        public bool CanCheckIn(DTO_BookingCard booking)
        {
            if (booking == null)
                return false;

            return booking.BookingStatus == "Confirmed" && DateTime.Today >= booking.CheckInDate.Date;
        }

        public bool CanCheckOut(DTO_BookingCard booking)
        {
            if (booking == null)
                return false;

            return booking.BookingStatus == "CheckedIn";
        }

        public bool CanCancel(DTO_BookingCard booking)
        {
            if (booking == null)
                return false;

            return booking.BookingStatus != "Completed" &&
                   booking.BookingStatus != "Cancelled" &&
                   booking.BookingStatus != "Refunded";
        }

        public bool ConfirmBooking(long bookingId, long changedByUserId)
        {
            var booking = _booking.GetBookingEntity(bookingId);

            if (booking == null)
                return false;

            if (booking.BookingStatus != "Pending")
                return false;

            string oldStatus = booking.BookingStatus;

            bool updated = _booking.UpdateBookingStatus(bookingId, "Confirmed");

            if (!updated)
                return false;

            _booking.InsertBookingTimeline(bookingId, oldStatus, "Confirmed", changedByUserId, "Booking confirmed");

            return true;
        }

        public bool CheckInBooking(long bookingId, long changedByUserId)
        {
            var booking = _booking.GetBookingEntity(bookingId);

            if (booking == null)
                return false;

            if (booking.BookingStatus != "Confirmed")
                return false;

            bool updated = _booking.UpdateBookingStatus(bookingId, "CheckedIn");

            if (!updated)
                return false;

            _booking.InsertBookingTimeline(bookingId, "Confirmed", "CheckedIn", changedByUserId, "Guest checked in");

            return true;
        }

        public bool CheckOutBooking(long bookingId, long changedByUserId)
        {
            var booking = _booking.GetBookingEntity(bookingId);

            if (booking == null)
                return false;

            if (booking.BookingStatus != "CheckedIn")
                return false;

            bool updated =
                _booking.UpdateBookingStatus(bookingId, "Completed");

            if (!updated)
                return false;

            _booking.InsertBookingTimeline(bookingId, "CheckedIn", "Completed", changedByUserId, "Guest checked out");

            return true;
        }

        public bool CancelBooking(long bookingId, long changedByUserId, string reason)
        {
            var booking = _booking.GetBookingEntity(bookingId);

            if (booking == null)
                return false;

            if (booking.BookingStatus == "Completed")
                return false;

            if (booking.BookingStatus == "Cancelled")
                return false;

            string oldStatus = booking.BookingStatus;

            bool updated = _booking.UpdateBookingStatus(bookingId, "Cancelled");

            if (!updated)
                return false;

            _booking.InsertBookingTimeline(bookingId, oldStatus, "Cancelled", changedByUserId, reason);

            return true;
        }

        public int GetTotalBookings()
        {
            return GetBookingCards()
                .Count;
        }

        public int GetPendingBookings()
        {
            return GetBookingCards().Count(x => x.BookingStatus == "Pending");
        }

        public int GetConfirmedBookings()
        {
            return GetBookingCards().Count(x => x.BookingStatus == "Confirmed");
        }

        public int GetCheckedInBookings()
        {
            return GetBookingCards()
                .Count(x => x.BookingStatus == "CheckedIn");
        }

        public int GetCompletedBookings()
        {
            return GetBookingCards()
                .Count(x => x.BookingStatus == "Completed");
        }

        public decimal GetTotalRevenue()
        {
            return GetBookingCards()
                .Where(x => x.BookingStatus != "Cancelled").Sum(x => x.FinalPrice);
        }

        
        /// <summary>
        /// CREATE MANUAL BOOKING ( TRẢ VỀ bookingId)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        public (bool Success, long BookingId, string Error) CreateManualBooking(DTO_CreateBooking dto)
        {
            if (dto == null)
                return (false, 0, "Booking information is missing.");

            string error;

            if (!ValidateGuest(dto, out error))
                return (false, 0, error);

            if (!ValidateListing(dto, out error))
                return (false, 0, error);

            if (!ValidateDates(dto, out error))
                return (false, 0, error);

            if (!ValidateCapacity(dto, out error))
                return (false, 0, error);

            if (!ValidatePricing_Manual(dto, out error))
                return (false, 0, error);

            if (!ValidateNights(dto, out error))
                return (false, 0, error);

            if (!ValidateOverlap(dto, out error))
                return (false, 0, error);

            if (!ValidateAddonServices(dto, out error))
                return (false, 0, error);

            long bookingId = _booking.CreateManualBooking(dto);

            if (bookingId <= 0)
                return (false, 0, "Unable to create booking.");

            return (true, bookingId, string.Empty);
        }

        // =====================================================
        // ADDON SERVICES VALIDATION (MỚI)
        // =====================================================

        private bool ValidateAddonServices(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            // Không có addon thì bỏ qua — hoàn toàn hợp lệ
            if (dto.AddonList == null || !dto.AddonList.Any())
                return true;

            foreach (var addon in dto.AddonList)
            {
                // Lấy thông tin service từ DB
                var service = _serviceDal.GetByID(addon.ServiceID);

                if (service == null)
                {
                    error = $"Service '{addon.ServiceName}' not found.";
                    return false;
                }

                // VALIDATE: Số người phải >= 1
                if (addon.NumberOfPeople < 1)
                {
                    error = $"'{service.Name}': Number of people must be at least 1.";
                    return false;
                }

                // VALIDATE: Ngày áp dụng phải nằm trong khoảng lưu trú
                if (addon.FromDate.Date < dto.CheckInDate.Date ||
                    addon.FromDate.Date >= dto.CheckOutDate.Date)
                {
                    error = $"'{service.Name}': Service date ({addon.FromDate:dd/MM/yyyy}) " +
                            $"must be within the stay period " +
                            $"({dto.CheckInDate:dd/MM/yyyy} – {dto.CheckOutDate:dd/MM/yyyy}).";
                    return false;
                }

                // VALIDATE: DayOfWeek
                if (!ValidateDayOfWeek(service.DayOfWeek, addon.FromDate, out string dowError))
                {
                    error = $"'{service.Name}': {dowError}";
                    return false;
                }

                // VALIDATE: DayOfMonth
                if (!ValidateDayOfMonth(service.DayOfMonth, addon.FromDate, out string domError))
                {
                    error = $"'{service.Name}': {domError}";
                    return false;
                }

                // VALIDATE: DailyCap
                // Đếm số người đã đặt cùng service + cùng ngày trong DB
                long currentDailyUsage = _addonDal.CountDailyUsage(addon.ServiceID, addon.FromDate);

                // Cộng thêm số người trong cùng dto (trường hợp addon trùng nhau)
                long sameAddonInDto = dto.AddonList
                    .Where(x => x.ServiceID == addon.ServiceID
                             && x.FromDate.Date == addon.FromDate.Date
                             && !ReferenceEquals(x, addon))
                    .Sum(x => x.NumberOfPeople);

                long totalDailyUsage = currentDailyUsage + sameAddonInDto + addon.NumberOfPeople;

                if (totalDailyUsage > service.DailyCap)
                {
                    error = $"'{service.Name}' on {addon.FromDate:dd/MM/yyyy}: " +
                            $"Daily capacity exceeded. " +
                            $"Limit: {service.DailyCap}, " +
                            $"Already booked: {currentDailyUsage + sameAddonInDto}, " +
                            $"Requesting: {addon.NumberOfPeople}.";
                    return false;
                }

                // VALIDATE: BookingCap
                // Đếm số lần service này xuất hiện trong cùng 1 booking (trong dto)
                int sameServiceCount = dto.AddonList
                    .Count(x => x.ServiceID == addon.ServiceID);

                if (sameServiceCount > service.BookingCap)
                {
                    error = $"'{service.Name}': " +
                            $"Booking capacity exceeded. " +
                            $"Maximum {service.BookingCap} time(s) per booking, " +
                            $"but {sameServiceCount} selected.";
                    return false;
                }
            }

            return true;
        }

        // =====================================================
        // VALIDATE DAY OF WEEK
        // Format DB: "1,3,5" (1=Sunday, 2=Monday, ..., 7=Saturday)
        // Tương ứng SQL Server DATEPART(dw, ...)
        // C# DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
        // => Chuyển đổi: C# DayOfWeek + 1 = SQL DayOfWeek
        // =====================================================

        private bool ValidateDayOfWeek(string dayOfWeekRule, DateTime fromDate, out string error)
        {
            error = string.Empty;

            // Nếu rule rỗng hoặc null → không giới hạn → hợp lệ
            if (string.IsNullOrWhiteSpace(dayOfWeekRule))
                return true;

            string trimmed = dayOfWeekRule.Trim();

            if (string.IsNullOrEmpty(trimmed))
                return true;

            // Parse danh sách ngày cho phép
            HashSet<int> allowedDays = ParseNumberList(trimmed);

            if (!allowedDays.Any())
                return true;

            // Chuyển C# DayOfWeek sang SQL Server convention (1-based, Sunday=1)
            int sqlDayOfWeek = (int)fromDate.DayOfWeek + 1;

            if (!allowedDays.Contains(sqlDayOfWeek))
            {
                error = $"This service is not available on {fromDate.DayOfWeek}s.";
                return false;
            }

            return true;
        }

        // =====================================================
        // VALIDATE DAY OF MONTH
        // Format DB: "1,2,3,15-20" (hỗ trợ dấu phẩy và range)
        // =====================================================

        private bool ValidateDayOfMonth(string dayOfMonthRule, DateTime fromDate, out string error)
        {
            error = string.Empty;

            // Nếu rule rỗng hoặc null → không giới hạn → hợp lệ
            if (string.IsNullOrWhiteSpace(dayOfMonthRule))
                return true;

            string trimmed = dayOfMonthRule.Trim();

            if (string.IsNullOrEmpty(trimmed))
                return true;

            // Parse danh sách ngày cho phép
            HashSet<int> allowedDays = ParseNumberList(trimmed);

            if (!allowedDays.Any())
                return true;

            int dayOfMonth = fromDate.Day;

            if (!allowedDays.Contains(dayOfMonth))
            {
                error = $"This service is not available on day {dayOfMonth} of the month. " +
                        $"Allowed days: {trimmed}.";
                return false;
            }

            return true;
        }

        // =====================================================
        // PARSE NUMBER LIST
        // Hỗ trợ cả dấu phẩy và range (gạch ngang)
        // Ví dụ: "1,2,3,15-20" → {1,2,3,15,16,17,18,19,20}
        // =====================================================

        private HashSet<int> ParseNumberList(string input)
        {
            HashSet<int> result = new HashSet<int>();

            if (string.IsNullOrWhiteSpace(input))
                return result;

            string[] parts = input.Split(',');

            foreach (string part in parts)
            {
                string trimmed = part.Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Kiểm tra range (có dấu gạch ngang)
                if (trimmed.Contains("-"))
                {
                    string[] rangeParts = trimmed.Split('-');

                    if (rangeParts.Length == 2)
                    {
                        int start;
                        int end;

                        if (int.TryParse(rangeParts[0].Trim(), out start) &&
                            int.TryParse(rangeParts[1].Trim(), out end))
                        {
                            for (int i = start; i <= end; i++)
                            {
                                result.Add(i);
                            }
                        }
                    }
                }
                else
                {
                    int number;

                    if (int.TryParse(trimmed, out number))
                    {
                        result.Add(number);
                    }
                }
            }

            return result;
        }

        // =====================================================
        // EXISTING VALIDATIONS (GIỮ NGUYÊN + CẬP NHẬT PRICING)
        // =====================================================

        private bool ValidateOverlap(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            bool available = _booking.ValidateBookingOverlap(dto.ItemID, dto.CheckInDate, dto.CheckOutDate);
            if (!available)
            {
                error = "This listing already has a booking during the selected dates.";
                return false;
            }

            return true;
        }
        private bool ValidateNights(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            if (dto.Nights == null)
            {
                error = "Night pricing is missing.";
                return false;
            }

            if (!dto.Nights.Any())
            {
                error = "Night pricing is missing.";
                return false;
            }

            int expectedNights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;

            if (dto.Nights.Count != expectedNights)
            {
                error = "Night count mismatch.";
                return false;
            }

            return true;
        }

        // =====================================================
        // VALIDATE PRICING (CẬP NHẬT — CỘNG THÊM ADDON TOTAL)
        // Công thức mới:
        // FinalAmount = BaseAmount - Discount + CleaningFee
        //             + ServiceFee + Tax + AddonServicesTotal
        // =====================================================

        private bool ValidatePricing_Manual(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            if (dto.BaseAmount <= 0)
            {
                error = "Base amount is invalid.";
                return false;
            }

            if (dto.FinalAmount <= 0)
            {
                error = "Final amount is invalid.";
                return false;
            }

            decimal expected = dto.BaseAmount
                             - dto.DiscountAmount
                             + dto.CleaningFee
                             + dto.ServiceFee
                             + dto.TaxAmount
                             + dto.AddonServicesTotal;

            if (Math.Abs(dto.FinalAmount - expected) > 0.01m)
            {
                error = $"Final amount validation failed. " +
                        $"Expected: {expected:N0}, Actual: {dto.FinalAmount:N0}.";
                return false;
            }

            return true;
        }
        private bool ValidateDates(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            if (dto.CheckInDate.Date >= dto.CheckOutDate.Date)
            {
                error = "Check-out date must be after check-in date.";
                return false;
            }

            //if (dto.CheckInDate.Date < DateTime.Today)
            //{
            //    error = "Check-in date cannot be in the past.";
            //    return false;
            //}

            return true;
        }
        private bool ValidateListing(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            if (dto.ItemID <= 0)
            {
                error = "Please select a listing.";

                return false;
            }

            return true;
        }
        private bool ValidateCapacity(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            var item = _dalItems.GetEditItem(dto.ItemID);

            if (item == null)
            {
                error = "Listing not found.";
                return false;
            }

            if (dto.NumberOfGuests > item.Capacity)
            {
                error = $"Maximum capacity is {item.Capacity} guests.";
                return false;
            }

            return true;
        }
        private bool ValidateGuest(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            if (dto.GuestUserID <= 0)
            {
                error = "Please select a guest.";

                return false;
            }

            return true;
        }
    }
}