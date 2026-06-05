using System.Web;
using System.Web.Mvc;

namespace GUI_Web_Payment
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
