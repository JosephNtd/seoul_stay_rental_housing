using System;
using System.Collections.Generic;

namespace DTO
{
    public class DTO_BookingCalendar
    {

        // CORE BOOKING


        public long BookingID { get; set; }

        public Guid BookingGUID { get; set; }

        public long ItemID { get; set; }

        public long GuestUserID { get; set; }


        // LISTING


        public string ListingTitle { get; set; }

        public string ListingType { get; set; }

        public string ListingThumbnail { get; set; }

        public string AreaName { get; set; }


        // GUEST


        public string GuestName { get; set; }

        public string GuestAvatar { get; set; }

        public int NumberOfGuests { get; set; }


        // DATES


        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int TotalNights { get; set; }


        // STATUS


        public string BookingStatus { get; set; }

        public bool IsPending =>
            BookingStatus == "Pending";

        public bool IsConfirmed =>
            BookingStatus == "Confirmed";

        public bool IsCheckedIn =>
            BookingStatus == "CheckedIn";

        public bool IsCompleted =>
            BookingStatus == "Completed";

        public bool IsCancelled =>
            BookingStatus == "Cancelled";


        // PAYMENT


        public bool IsPaid { get; set; }

        public decimal FinalPrice { get; set; }


        // CALENDAR DISPLAY


        public string BookingCode =>
            $"BK-{BookingID:000000}";

        public string DateRangeDisplay =>
            $"{CheckInDate:dd MMM} → {CheckOutDate:dd MMM}";

        public string GuestDisplay =>
            $"{NumberOfGuests} guest(s)";

        public string PriceDisplay =>
            $"${FinalPrice:0,0.##}";


        // OCCUPANCY


        public bool IsActiveOnDate(
            DateTime date)
        {
            return date.Date >= CheckInDate.Date
                &&
                   date.Date <= CheckOutDate.Date
                &&
                   !IsCancelled;
        }

        public bool IsCheckInDate(
            DateTime date)
        {
            return date.Date ==
                   CheckInDate.Date;
        }

        public bool IsCheckOutDate(
            DateTime date)
        {
            return date.Date ==
                   CheckOutDate.Date;
        }


        // CURRENT STAY FLAGS


        public bool IsCurrentStay
        {
            get
            {
                DateTime today =
                    DateTime.Today;

                return today >= CheckInDate.Date
                    &&
                       today <= CheckOutDate.Date
                    &&
                       !IsCancelled;
            }
        }

        public bool IsUpcomingStay =>
            DateTime.Today < CheckInDate.Date;

        public bool IsPastStay =>
            DateTime.Today > CheckOutDate.Date;


        // UI COLORS


        public string BookingColor
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending":
                        return "#E9A63A";

                    case "Confirmed":
                        return "#3A86E9";

                    case "CheckedIn":
                        return "#2AA876";

                    case "Completed":
                        return "#6C63FF";

                    case "Cancelled":
                        return "#E05252";

                    default:
                        return "#A0A0A0";
                }
            }
        }

        public string BackgroundColor
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending":
                        return "#FFF4DD";

                    case "Confirmed":
                        return "#EAF3FF";

                    case "CheckedIn":
                        return "#EAF8EE";

                    case "Completed":
                        return "#F1EEFF";

                    case "Cancelled":
                        return "#FDECEC";

                    default:
                        return "#F5F5F5";
                }
            }
        }


        // CALENDAR TAGS


        public string CalendarTag
        {
            get
            {
                if (IsCheckedIn)
                    return "IN HOUSE";

                if (IsTodayArrival)
                    return "ARRIVAL";

                if (IsTodayDeparture)
                    return "DEPARTURE";

                if (IsPending)
                    return "PENDING";

                return "BOOKED";
            }
        }


        // TODAY FLAGS


        public bool IsTodayArrival =>
            DateTime.Today ==
            CheckInDate.Date;

        public bool IsTodayDeparture =>
            DateTime.Today ==
            CheckOutDate.Date;


        // OPERATIONS


        public bool CanCheckIn =>
            IsConfirmed
            &&
            DateTime.Today >= CheckInDate.Date;

        public bool CanCheckOut =>
            IsCheckedIn;

        public bool CanCancel =>
            !IsCancelled
            &&
            !IsCompleted;


        // OCCUPANCY HELPERS


        public int GetStayProgressPercentage()
        {
            if (!IsCurrentStay)
                return 0;

            double totalDays =
                (CheckOutDate.Date -
                 CheckInDate.Date).TotalDays;

            double currentDays =
                (DateTime.Today -
                 CheckInDate.Date).TotalDays;

            if (totalDays <= 0)
                return 100;

            return (int)(
                currentDays /
                totalDays * 100);
        }


        // SUMMARY


        public string SummaryDisplay
        {
            get
            {
                return
                    $"{GuestName} • " +
                    $"{DateRangeDisplay} • " +
                    $"{PriceDisplay}";
            }
        }


        // DAILY OCCUPANCY


        public List<DateTime> OccupiedDates
        {
            get
            {
                List<DateTime> dates = new List<DateTime>();

                DateTime current = CheckInDate.Date;

                while (current < CheckOutDate.Date)
                {
                    dates.Add(current);
                    current = current.AddDays(1);
                }

                return dates;
            }
        }
    }
}