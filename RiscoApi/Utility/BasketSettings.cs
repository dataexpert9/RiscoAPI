using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi
{
    public static class BasketSettings
    {
        
        public static Settings Settings { get; set; }

        public static void LoadSettings()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var setting = ctx.Settings.FirstOrDefault();
                    if (setting != null)
                    {
                        Settings = setting;
                        Settings.Interests = ctx.Interests.Where(x => x.IsDeleted == false).ToList();
                        //Settings.BannerImages = ctx.BannerImages.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        public static string GetAdminEmailForOrders()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var setting = ctx.Settings.FirstOrDefault();
                    if (setting != null)
                    {
                        return setting.AdminEmailForOrders;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {

                Utility.LogError(ex);
                return "";
            }
        }
    }
}