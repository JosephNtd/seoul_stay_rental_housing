using DAL;
using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_Guest
    {
        private DAL_Guest _dal = new DAL_Guest();
        public ET_Guest GetGuestProfile(long userID) => _dal.GetGuestProfile(userID);
        public bool UpdateGuestProfile(ET_Guest guest) => _dal.UpdateGuestProfile(guest);
        public List<DTO_RecentStay> GetRecentStays(long userId) => _dal.GetRecentStays(userId);
        public List<DTO_GuestLookup> GetLookupData() => _dal.GetLookupData();
    }
}
