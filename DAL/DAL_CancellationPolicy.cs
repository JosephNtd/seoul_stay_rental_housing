using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_CancellationPolicy
    {
        Seoul_StayDataContext db = new Seoul_StayDataContext();

        public List<CancellationPolicy> GetData()
        {
            return db.CancellationPolicies.ToList();
        }

        public List<CancellationRefundFee> GetFees(long policyId)
        {
            return db.CancellationRefundFees.Where(f => f.CancellationPolicyID == policyId).OrderBy(f => f.DaysLeft).ToList();
        }
    }
}
