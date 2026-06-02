using DTO;
using ET;
using System.Linq;

namespace DAL
{
    public class DAL_UserWorkspace
    {
        // DEPENDENCIES

        private readonly DAL_User _user =
            new DAL_User();

        private readonly DAL_Guest _guest =
            new DAL_Guest();

        private readonly DAL_Host _host =
            new DAL_Host();

        private readonly DAL_HostBankAccount _bank =
            new DAL_HostBankAccount();

        private readonly DAL_UserBooking _booking =
            new DAL_UserBooking();

        // MAIN WORKSPACE

        public DTO_UserWorkspace GetWorkspace(
            long userId)
        {
            DTO_UserWorkspace workspace =
                new DTO_UserWorkspace();

             
            // USER
             

            var user =
                _user.GetById(userId);

            if (user == null)
                return null;

            workspace.User =
                new ET_Users
                {
                    ID = user.ID,
                    GUID = user.GUID,
                    Username = user.Username,
                    Password = user.Password,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    BirthDate = user.BirthDate,
                    Country = user.Country,
                    ProfilePicture = user.ProfilePicture,
                    IsAdmin = user.IsAdmin,
                    CreatedDate = user.CreatedDate,
                    IsActive = user.IsActive
                };

             
            // ROLE FLAGS
             

            workspace.IsAdmin =
                user.IsAdmin;

             
            // GUEST PROFILE
             

            var guestProfile =
                _guest.GetGuestProfile(userId);

            if (guestProfile != null)
            {
                workspace.IsGuest = true;

                workspace.GuestProfile =
                    guestProfile;
            }

             
            // HOST PROFILE
             

            var hostProfile =
                _host.GetHostProfile(userId);

            if (hostProfile != null)
            {
                workspace.IsHost = true;

                workspace.HostProfile =
                    hostProfile;

                 
                // BANK ACCOUNTS
                 

                workspace.BankAccounts =
                    _bank.GetBankAccounts(userId);
            }

             
            // TRANSACTIONS
             

            using (var db = new Seoul_StayDataContext())
            {
                workspace.Transactions =
                    db.Transactions
                    .Where(t => t.UserID == userId)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new ET_Transactions
                    {
                        ID = t.ID,
                        GUID = t.GUID,
                        UserID = t.UserID,
                        TransactionTypeID = t.TransactionTypeID,
                        Amount = t.Amount,
                        TransactionDate = t.TransactionDate,
                        GatewayReturnID = t.GatewayReturnID
                    })
                    .Take(15)
                    .ToList();
            }

             
            // BOOKINGS
             

            if (workspace.IsGuest)
            {
                workspace.Bookings =
                    _booking
                    .GetGuestBookings(userId)
                    .Take(10)
                    .ToList();
            }
            else if (workspace.IsHost)
            {
                workspace.Bookings =
                    _booking
                    .GetHostBookings(userId)
                    .Take(10)
                    .ToList();
            }

             
            // TOTAL BOOKINGS
             

            workspace.TotalBookings =
                workspace.Bookings.Count;

             
            // TOTAL TRANSACTIONS
             

            workspace.TotalTransactions =
                workspace.Transactions.Count;

             
            // TOTAL SPENT
             

            workspace.TotalSpent =
                workspace.Transactions
                .Where(x => x.Amount < 0)
                .Sum(x => x.Amount) * -1;

             
            // TOTAL REVENUE
             

            workspace.TotalRevenue =
                workspace.Transactions
                .Where(x => x.Amount > 0)
                .Sum(x => x.Amount);

            return workspace;
        }

         
        // USER DIRECTORY
         

        public System.Collections.Generic.List<DTO_UserDisplay> GetUsersDirectory()
        {
            return _user.GetAllUsersDisplay();
        }
    }
}
