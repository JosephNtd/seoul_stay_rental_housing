using DAL;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_CancellationPolicy
    {
        DAL_CancellationPolicy _dal = new DAL_CancellationPolicy();
        public List<ET_CancellationPolicies> GetAll()
        {
            return _dal.GetData()
                       .Select(p => new ET_CancellationPolicies
                       {
                           ID = p.ID,
                           GUID = p.GUID,
                           Name = p.Name,
                           PlatformCommissionRate = p.PlatformCommissionRate
                       })
                       .ToList();
        }
        public List<CancellationRefundFee> GetFees(long policyId) => _dal.GetFees(policyId);
    }
}
