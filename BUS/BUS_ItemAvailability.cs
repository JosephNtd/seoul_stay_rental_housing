using System;
using System.Collections.Generic;
using DAL;
using ET;

namespace BUS
{
    public class BUS_ItemAvailability
    {
        private readonly DAL_ItemAvailability _dalAvailability = new DAL_ItemAvailability();

        public List<ET_ItemAvailability> GetAvailabilities(long itemId)
        {
            if (itemId <= 0) return new List<ET_ItemAvailability>();
            return _dalAvailability.GetAvailabilities(itemId);
        }

        public bool SetAvailabilityForDates(long itemId, List<DateTime> selectedDates, bool isAvailable)
        {
            if (itemId <= 0 || selectedDates == null || selectedDates.Count == 0)
                return false;

            return _dalAvailability.SetAvailabilities(itemId, selectedDates, isAvailable);
        }
    }
}