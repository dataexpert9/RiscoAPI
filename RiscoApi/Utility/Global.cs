using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BasketApi
{
    public static class Global
    {
        public static PushNotifications objPushNotifications = new PushNotifications(false);
        public static int MaximumImageSize = 1024 * 1024 * 10; // 10 Mb
        public static string ImageSize = "10 MB";
        

        private static int searchStoreRadius = Convert.ToInt32(ConfigurationManager.AppSettings["NearByStoreRadius"]);

        public static double NearbyStoreRadius = searchStoreRadius * 1609.344;

        public enum PushNotificationType
        {
            Announcement = 1,
            OrderAccepted = 2,
            OrderRejected = 3,
            OrderAssignedToDeliverer = 4,
            OrderDispatched = 5,
            OrderCompleted = 6
        }

        public enum NotificationTargetAudienceTypes
        {
            UserAndDeliverer = 1,
            User = 2,
            Deliverer = 3,
            IndividualUser

        }

        public enum BoxCategoryOptions
        {
            Junior = 1,
            Monthly = 2,
            ProBox = 3,
            HallOfFame = 4
        }

        public enum PlatformTypes
        {
            Android = 1,
            Ios = 2,
            Web = 3
        }

        public enum WeightUnits
        {
            gm = 1,
            kg = 2
        }

        public enum StatusCode
        {
            NotVerified = 1,
            Verified = 2
        }

        public enum NotificationStatus
        {
            Unread,
            Read
        }

        public enum DelivererTypes
        {
            Salaried,
            Freelance
        }

        public enum PaymentMethods
        {
            CashOnDelivery,
            CreditCard,
            DebitCard
        }

        public enum PaymentCardTypes
        {
            CreditCard = 1,
            DebitCard = 2
        }

        public enum OrderStatuses
        {
            Initiated,
            Accepted,
            Rejected,
            InProgress,
            ReadyForDelivery,
            AssignedToDeliverer, //equivalent to deliverer initiated 3
            DelivererInProgress,
            Dispatched,
            Completed
        }

        public enum UserAddressTypes
        {
            Residential,
            Business,
            Postal,
            POBox,
            MailTo,
            DeliveryTo
        }

        public enum ApnsEnvironmentTypes
        {
            Sandbox,
            Production
        }

        public enum ApplicationTypes
        {
            PlayStore,
            Enterprise
        }

        public enum CartItemTypes
        {
            Product,
            Package,
            Offer_Product,
            Offer_Package,
            Box
        }

        public enum PostVisibilityTypes
        {
            Public = 1,
            Follower = 2,
            OnlyMe = 3
        }

        public enum RiskLevelTypes
        {
            High = 1,
            Medium = 2,
            Low = 3
        }

        public enum MediaTypes
        {
            Image = 1,
            Video = 2
        }
        
        public enum PostTaggingPrivacyTypes
        {
            Anyone = 1,
            Following = 2,
            Follower = 3,
            NotAllowed = 4
        }

        public enum DirectMessagePrivacyTypes
        {
            Anyone = 1,
            Following = 2,
            Follower = 3,
            NotAllowed = 4
        }

        public enum ReportPostTypes
        {
            Spam = 1,
            HateSpeech = 2,
            Violence = 3,
            Duplicate = 4
        }



        public class ResponseMessages
        {
            public const string Success = "Success";
            public const string NotFound = "NotFound";
            public const string BadRequest = "BadRequest";
            public const string Conflict = "Conflict";

            public static string CannotBeEmpty(params string[] args)
            {
                try
                {
                    string returnString = "";
                    for (int i = 0; i < args.Length; i++)
                        returnString += args[i] + ", ";
                    returnString = returnString.Remove(returnString.LastIndexOf(','), 1);
                    return returnString + "cannot be empty";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateInvalid(params string[] args)
            {
                try
                {
                    string returnString = "";
                    for (int i = 0; i < args.Length; i++)
                        returnString += args[i] + ", ";
                    returnString = returnString.Remove(returnString.LastIndexOf(','), 1);
                    return "Invalid " + returnString;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateAlreadyExists(string arg)
            {
                try
                {
                    return arg + " already exists";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static string GenerateNotFound(string arg)
            {
                try
                {
                    return arg + " not found";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}