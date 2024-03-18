using System.Web;
using System.Web.Mvc;

namespace WebBanSach_62133314
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
