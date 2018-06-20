using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class WebDashboardStatsViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalStores { get; set; }
        public int TotalUsers { get; set; }
        public int TodayOrders { get; set; }
        public List<DeviceStats> DeviceUsage { get; set; }
    }

    public class DeviceStats
    {
        public int Count { get; set; }
        public int Percentage { get; set; }
    }
}