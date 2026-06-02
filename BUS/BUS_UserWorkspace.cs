using DAL;
using DTO;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class BUS_UserWorkspace
    {
         
        // DEPENDENCIES
         

        private readonly DAL_UserWorkspace _workspace =
            new DAL_UserWorkspace();

        // GET FULL WORKSPACE


        public DTO_UserWorkspace GetWorkspace(
            long userId)
        {
            return _workspace.GetWorkspace(userId);
        }

         
        // USER DIRECTORY
         

        public List<DTO_UserDisplay> GetUsersDirectory()
        {
            return _workspace.GetUsersDirectory();
        }

         
        // ROLE HELPERS
         

        public bool IsGuest(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return false;

            return data.IsGuest;
        }

        public bool IsHost(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return false;

            return data.IsHost;
        }

        public bool IsAdmin(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return false;

            return data.IsAdmin;
        }

         
        // QUICK STATS
         

        public int GetTotalBookings(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return 0;

            return data.TotalBookings;
        }

        public decimal GetTotalSpent(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return 0;

            return data.TotalSpent;
        }

        public decimal GetTotalRevenue(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return 0;

            return data.TotalRevenue;
        }

        public int GetTotalTransactions(long userId)
        {
            var data =
                _workspace.GetWorkspace(userId);

            if (data == null)
                return 0;

            return data.TotalTransactions;
        }

         
        // SEARCH USERS
         

        public List<DTO_UserDisplay> SearchUsers(
            string keyword)
        {
            var data =
                _workspace.GetUsersDirectory();

            if (string.IsNullOrWhiteSpace(keyword))
                return data;

            keyword =
                keyword.ToLower().Trim();

            return data.FindAll(x =>
                   x.FullName.ToLower().Contains(keyword)
                || x.Email.ToLower().Contains(keyword)
                || x.Username.ToLower().Contains(keyword));
        }

         
        // FILTER USERS BY ROLE
         

        public List<DTO_UserDisplay> FilterByRole(
            string role)
        {
            var data =
                _workspace.GetUsersDirectory();

            if (string.IsNullOrWhiteSpace(role)
                || role == "All")
                return data;

            return data.FindAll(x =>
                x.Role == role);
        }

         
        // FILTER USERS BY STATUS
         

        public List<DTO_UserDisplay> FilterByStatus(
            string status)
        {
            var data =
                _workspace.GetUsersDirectory();

            if (string.IsNullOrWhiteSpace(status)
                || status == "All")
                return data;

            return data.FindAll(x =>
                x.Status == status);
        }

         
        // SORT USERS
         

        public List<DTO_UserDisplay> SortUsers(
            string sortMode)
        {
            var data = _workspace.GetUsersDirectory();

            switch (sortMode)
            {
                case "A-Z":

                    return data.OrderBy(x => x.FullName).ToList();

                case "Newest":

                    return data.OrderByDescending(x => x.UserID).ToList();

                case "Role":

                    return data.OrderBy(x => x.Role).ToList();

                default:
                    return data;
            }
        }
    }
}