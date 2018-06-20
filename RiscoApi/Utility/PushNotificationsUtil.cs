
using DAL;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketApi
{
    public class PushNotificationsUtil : IDisposable
    {
        public static bool IsIOSProductionEnvironment = Convert.ToBoolean(ConfigurationManager.AppSettings["IsIOSProductionEnivronment"]);

        public static PushNotifications pushNotification = new PushNotifications(IsIOSProductionEnvironment);

        public PushNotificationsUtil()
        {

        }

        /// <summary>
        /// Configures devices for push notifications according to their Environment and Application Types
        /// </summary>
        /// <param name="deviceModel">Device to be configured.</param>

        public static void ConfigurePushNotifications(UserDevice deviceModel)
        {
            try
            {
                if (deviceModel.Platform == false) //IOS
                {
                    #region IOS
                    if (deviceModel.ApplicationType == UserDevice.ApplicationTypes.Enterprise)
                    {
                        if (deviceModel.EnvironmentType == UserDevice.ApnsEnvironmentTypes.Production)
                        {
                            deviceModel.iOSPushConfiguration = Enterprise.IOS.ProductionConfig;
                        }
                        else // Sandbox/Development
                        {
                            deviceModel.iOSPushConfiguration = Enterprise.IOS.SandboxConfig;
                        }
                    }
                    else //PlayStore
                    {
                        if (deviceModel.EnvironmentType == UserDevice.ApnsEnvironmentTypes.Production)
                        {
                            deviceModel.iOSPushConfiguration = PlayStore.IOS.ProductionConfig;
                        }
                        else // Sandbox/Development
                        {
                            deviceModel.iOSPushConfiguration = PlayStore.IOS.SandboxConfig;
                        }
                    }
                    #endregion
                }
                else // For Android
                {
                    #region Android
                    if (deviceModel.ApplicationType == UserDevice.ApplicationTypes.Enterprise)
                    {
                        deviceModel.AndroidPushConfiguration = Enterprise.Android.AndroidGCMConfig;
                    }
                    else
                    {
                        deviceModel.AndroidPushConfiguration = PlayStore.Android.AndroidGCMConfig;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        internal static void DeviceRemoved(object sender, EventArgs e)
        {
            try
            {
                //var relevantSet = Global.AllUserDevices.Values.SelectMany(x => x).Where(x => x.AuthToken.Trim() == ((PushNotifications.PushNotificationEventArgs)e).AuthToken).FirstOrDefault();
                //if (relevantSet == null)
                //    return;
                //ISet<CM_DeviceModel> devicesSet;
                //if (Global.AllUserDevices.TryGetValue(relevantSet.UserID.Value, out devicesSet))
                //{
                //    devicesSet.Remove(devicesSet.Where(x => x.AuthToken.Trim() == ((PushNotifications.PushNotificationEventArgs)e).AuthToken).FirstOrDefault());
                //}
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
