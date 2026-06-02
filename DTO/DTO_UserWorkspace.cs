using ET;
using System.Collections.Generic;

namespace DTO
{
    public class DTO_UserWorkspace
    {

        // CORE USER

        public ET_Users User { get; set; }


        // ROLE PROFILES

        public ET_Guest GuestProfile { get; set; }

        public ET_Host HostProfile { get; set; }

        // FINANCIAL

        public List<ET_HostBankAccount> BankAccounts { get; set; }
            = new List<ET_HostBankAccount>();

        public List<ET_Transactions> Transactions { get; set; }
            = new List<ET_Transactions>();

        // BOOKINGS
        public List<DTO_UserBookingSummary> Bookings { get; set; }
            = new List<DTO_UserBookingSummary>();



        // DASHBOARD STATS

        public int TotalBookings { get; set; }

        public decimal TotalSpent { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TotalTransactions { get; set; }

        // ROLE FLAGS

        public bool IsGuest { get; set; }

        public bool IsHost { get; set; }

        public bool IsAdmin { get; set; }

        // DISPLAY HELPERS

        public string RoleDisplay
        {
            get
            {
                if (IsAdmin)
                    return "Administrator";

                if (IsHost)
                    return "Host";

                return "Guest";
            }
        }

        public string StatusDisplay
        {
            get
            {
                if (User == null)
                    return "Unknown";

                return User.IsActive
                    ? "Active"
                    : "Locked";
            }
        }

        public string VerificationDisplay
        {
            get
            {
                if (HostProfile != null)
                {
                    return HostProfile.IsVerified
                        ? "Verified Host"
                        : "Unverified Host";
                }

                if (GuestProfile != null)
                {
                    return GuestProfile.NationalIDVerified
                        ? "Verified Guest"
                        : "Unverified Guest";
                }

                return "";
            }
        }

        public string LoyaltyDisplay
        {
            get
            {
                if (GuestProfile == null)
                    return "N/A";

                return $"{GuestProfile.LoyaltyPoints} pts";
            }
        }

        public string RatingDisplay
        {
            get
            {
                if (HostProfile == null ||
                    HostProfile.Rating == null)
                    return "N/A";

                return $"{HostProfile.Rating:0.0} ★";
            }
        }
    }
}
