using System;

namespace DTO
{
    public class DTO_BookingTimeline
    {
         
        // CORE
         

        public long ID { get; set; }

        public Guid GUID { get; set; }

        public long BookingID { get; set; }

         
        // STATUS
         

        public string OldStatus { get; set; }

        public string NewStatus { get; set; }

         
        // CHANGE INFO
         

        public DateTime ChangedDate { get; set; }

        public long? ChangedByUserID { get; set; }

        public string ChangedByName { get; set; }

        public string ChangedByAvatar { get; set; }

        public string Notes { get; set; }

         
        // DISPLAY HELPERS
         

        public string ChangedDateDisplay =>
            ChangedDate.ToString(
                "dd MMM yyyy • HH:mm");

        public string ShortTimeDisplay =>
            ChangedDate.ToString(
                "HH:mm");

        public string ShortDateDisplay =>
            ChangedDate.ToString(
                "dd MMM");

         
        // STATUS DISPLAY
         

        public string StatusDisplay
        {
            get
            {
                switch (NewStatus)
                {
                    case "Pending":
                        return "Pending";

                    case "Confirmed":
                        return "Confirmed";

                    case "CheckedIn":
                        return "Checked-In";

                    case "Completed":
                        return "Completed";

                    case "Cancelled":
                        return "Cancelled";

                    case "Refunded":
                        return "Refunded";

                    default:
                        return "Unknown";
                }
            }
        }

         
        // EVENT TITLE
         

        public string EventTitle
        {
            get
            {
                switch (NewStatus)
                {
                    case "Pending":
                        return "Booking Created";

                    case "Confirmed":
                        return "Booking Confirmed";

                    case "CheckedIn":
                        return "Guest Checked-In";

                    case "Completed":
                        return "Booking Completed";

                    case "Cancelled":
                        return "Booking Cancelled";

                    case "Refunded":
                        return "Refund Issued";

                    default:
                        return "Booking Updated";
                }
            }
        }

         
        // EVENT DESCRIPTION
         

        public string EventDescription
        {
            get
            {
                // CREATED

                if (string.IsNullOrWhiteSpace(
                    OldStatus)
                    &&
                    NewStatus == "Pending")
                {
                    return
                        "Booking request has been created.";
                }

                // STATUS CHANGE

                if (!string.IsNullOrWhiteSpace(
                    OldStatus))
                {
                    return
                        $"Status changed from " +
                        $"{FormatStatus(OldStatus)} " +
                        $"to {FormatStatus(NewStatus)}.";
                }

                return
                    "Booking status updated.";
            }
        }

         
        // STATUS COLORS
         

        public string StatusColor
        {
            get
            {
                switch (NewStatus)
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

                    case "Refunded":
                        return "#8E8E8E";

                    default:
                        return "#A0A0A0";
                }
            }
        }

         
        // STATUS ICON
         

        public string StatusIcon
        {
            get
            {
                switch (NewStatus)
                {
                    case "Pending":
                        return "🕒";

                    case "Confirmed":
                        return "✔";

                    case "CheckedIn":
                        return "🏨";

                    case "Completed":
                        return "✅";

                    case "Cancelled":
                        return "✖";

                    case "Refunded":
                        return "💸";

                    default:
                        return "•";
                }
            }
        }

         
        // FLAGS
         

        public bool IsCreationEvent =>
            string.IsNullOrWhiteSpace(
                OldStatus)
            &&
            NewStatus == "Pending";

        public bool IsCancellation =>
            NewStatus == "Cancelled";

        public bool IsRefund =>
            NewStatus == "Refunded";

        public bool IsCheckIn =>
            NewStatus == "CheckedIn";

        public bool IsCompletion =>
            NewStatus == "Completed";

         
        // PRIVATE HELPERS
         

        private string FormatStatus(
            string status)
        {
            switch (status)
            {
                case "CheckedIn":
                    return "Checked-In";

                default:
                    return status;
            }
        }
    }
}