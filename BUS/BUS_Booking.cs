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
        public bool CreateManualBooking(DTO_CreateBooking dto, out string error)
        {
            error = string.Empty;

            if (dto == null)
            {
                error = "Booking information is missing.";
                return false;
            }
            if (!ValidateGuest(dto, out error))
                return false;

            if (!ValidateListing(dto, out error))
                return false;

            if (!ValidateDates(dto, out error))
                return false;

            if (!ValidateCapacity(dto, out error))
                return false;

            if (!ValidatePricing_Manual(dto, out error))
                return false;

            if (!ValidateNights(dto, out error))
                return false;

            if (!ValidateOverlap(dto, out error))
                return false;

            long bookingId = _booking.CreateManualBooking(dto);

            if (bookingId <= 0)
            {
                error = "Unable to create booking.";
                return false;
            }
            return true;
        }
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

            decimal expected = dto.BaseAmount - dto.DiscountAmount + dto.CleaningFee + dto.ServiceFee + dto.TaxAmount;
            if (Math.Abs(dto.FinalAmount - expected) > 0.01m)
            {
                error = "Final amount validation failed.";
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