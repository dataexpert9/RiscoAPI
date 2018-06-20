using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static BasketApi.Global;
using System.Data.Entity;
using WebApplication1.ViewModels;
using System.Configuration;

namespace BasketApi
{
    public static class ExtensionMethods
    {
        public static void CreateSignature(this string StringToSha, PayFortConfiguration configuration, CaptureBindingModel captureModel, VoidAuthorizationBindingModel VoidModel, SDKTokenModel tokenModel)
        {
            if (captureModel != null)
            {
                var SignatureToSha = configuration.sha_request_phrase + "access_code=" + captureModel.access_code + "amount=" + captureModel.amount + "command=" + captureModel.command + "currency=" + captureModel.currency + "fort_id=" + captureModel.fort_id + "language=" + captureModel.language + "merchant_identifier=" + captureModel.merchant_identifier + "merchant_reference=" + captureModel.merchant_reference + configuration.sha_request_phrase;
                captureModel.signature = Utility.sha256_hash(SignatureToSha);
            }
            else if (VoidModel != null)
            {
                var SignatureToShaVoid = configuration.sha_request_phrase + "access_code=" + VoidModel.access_code + "command=" + VoidModel.command + "fort_id=" + VoidModel.fort_id + "language=" + VoidModel.language + "merchant_identifier=" + VoidModel.merchant_identifier + "merchant_reference=" + VoidModel.merchant_reference + configuration.sha_request_phrase;
                VoidModel.signature = Utility.sha256_hash(SignatureToShaVoid);
            }
            else
            {
                var SignatureToShaSDK = configuration.sha_request_phrase + "access_code=" + tokenModel.access_code + "device_id=" + tokenModel.device_id + "language=" + tokenModel.language + "merchant_identifier=" + tokenModel.merchant_identifier + "service_command=" + tokenModel.service_command + configuration.sha_request_phrase;
                tokenModel.signature = Utility.sha256_hash(SignatureToShaSDK);
            }
        }

        //public static void GetPayFortConfiguration(this PayFortConfiguration model, CaptureBindingModel captureModel)
        //{
        //    try
        //    {
        //        BasketSettings.LoadSettings();

        //        model.access_code = BasketSettings.Settings.Payfort_AccessCode;
        //        model.merchant_identifier = BasketSettings.Settings.Payfort_MerchantId;
        //        model.sha_type = BasketSettings.Settings.SHA_TYPE;
        //        model.sha_request_phrase = BasketSettings.Settings.Payfort_RequestPhrase;
        //        model.sha_response_phrase = BasketSettings.Settings.Payfort_ResponsePhrase;
        //        model.language = "en";
        //        model.currency = "AED";
                
        //        captureModel.access_code = model.access_code;
        //        captureModel.merchant_identifier = model.merchant_identifier;

        //        captureModel.currency = model.currency;
        //        captureModel.language = model.language;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static void GetPayFortConfigurationForMobileSDK(this PayFortConfiguration model, SDKTokenModel sdkModel)
        //{
        //    try
        //    {
        //        BasketSettings.LoadSettings();

        //        model.access_code = BasketSettings.Settings.Payfort_AccessCode;
        //        model.merchant_identifier = BasketSettings.Settings.Payfort_MerchantId;
        //        model.sha_type = BasketSettings.Settings.SHA_TYPE;
        //        model.sha_request_phrase = BasketSettings.Settings.Payfort_RequestPhrase;
        //        model.sha_response_phrase = BasketSettings.Settings.Payfort_ResponsePhrase;
        //        model.language = "en";
        //        model.currency = "AED";

        //        sdkModel.access_code = model.access_code;
        //        sdkModel.merchant_identifier = model.merchant_identifier;
        //        sdkModel.language = model.language;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public static void CalculateAverageRating(this Store store)
        {
            try
            {
                if (store.StoreRatings.Count > 0)
                {
                    store.AverageRating = store.StoreRatings.Average(x => x.Rating);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void SetOrderItem(this Order_Items orderItem, CartItemViewModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    switch (model.ItemType)
                    {
                        case (int)CartItemTypes.Product:
                            orderItem.Product_Id = model.ItemId;
                            var product = ctx.Products.FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                            orderItem.Name = product.Name;
                            orderItem.Price = product.Price * model.Qty;
                            orderItem.Description = product.Description;
                            product.OrderedCount = product.OrderedCount + model.Qty;
                            ctx.SaveChanges();
                            break;
                        case (int)CartItemTypes.Package:
                            orderItem.Package_Id = model.ItemId;
                            var package = ctx.Packages.FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                            orderItem.Name = package.Name;
                            orderItem.Price = package.Price * model.Qty;
                            orderItem.Description = package.Description;
                            break;
                        case (int)CartItemTypes.Offer_Product:
                            orderItem.Offer_Product_Id = model.ItemId;
                            var offerProduct = ctx.Offer_Products.Include(x => x.Product).FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                            orderItem.Name = offerProduct.Product.Name;
                            orderItem.Price = offerProduct.DiscountedPrice * model.Qty;
                            orderItem.Description = offerProduct.Description;
                            break;
                        case (int)CartItemTypes.Offer_Package:
                            orderItem.Offer_Package_Id = model.ItemId;
                            var offerPackage = ctx.Offer_Packages.Include(x => x.Package).FirstOrDefault(x => x.Id == model.ItemId && x.IsDeleted == false);
                            orderItem.Name = offerPackage.Package.Name;
                            orderItem.Price = offerPackage.DiscountedPrice * model.Qty;
                            orderItem.Description = offerPackage.Description;
                            break;
                        default:
                            throw new Exception("Invalid CartItemType");
                    }
                }

                orderItem.Qty = model.Qty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AddNewStoreOrder(this Order order, CartItemViewModel model)
        {
            try
            {
                StoreOrder storeOrder = new StoreOrder();
                storeOrder.Store_Id = model.StoreId;
                storeOrder.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
                storeOrder.AddNewOrderItem(model);
                order.StoreOrders.Add(storeOrder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AddNewOrderItem(this StoreOrder storeOrder, CartItemViewModel model)
        {
            try
            {
                Order_Items orderItem = new Order_Items();
                orderItem.SetOrderItem(model);
                storeOrder.Order_Items.Add(orderItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SetPaymentDetails(this Order order, OrderViewModel model)
        {
            try
            {

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SetOrderDetails(this Order order, OrderViewModel model)
        {
            try
            {
                order.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
                TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"); DateTime utc = DateTime.UtcNow;
                DateTime UAE = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);
                order.OrderDateTime = UAE;
                order.Status = (int)OrderStatuses.Initiated;
                order.DeliveryTime_From = model.DeliveryDateTime_From;
                order.DeliveryTime_To = model.DeliveryDateTime_To;
                order.PaymentMethod = model.PaymentMethodType;
                order.User_ID = model.UserId;
                order.DeliveryAddress = model.DeliveryAddress;
                order.AdditionalNote = model.AdditionalNote;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void MakeOrder(this Order order, OrderViewModel model)
        {
            try
            {
                order.User_ID = model.UserId;
                foreach (var cartItem in model.Cart.CartItems)
                {
                    if (order.StoreOrders.Count == 0)
                    {
                        order.AddNewStoreOrder(cartItem);
                    }
                    else
                    {
                        var existingStoreOrder = order.StoreOrders.FirstOrDefault(x => x.Store_Id == cartItem.StoreId);

                        if (existingStoreOrder == null)
                        {
                            order.AddNewStoreOrder(cartItem);
                        }
                        else
                        {
                            existingStoreOrder.AddNewOrderItem(cartItem);
                        }
                    }
                }

                if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery)
                {
                    order.SetPaymentDetails(model);
                }

                order.SetOrderDetails(model);

                order.CalculateSubTotal();
                order.CalculateTotal();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CalculateSubTotal(this StoreOrder storeOrder)
        {
            storeOrder.Subtotal = storeOrder.Order_Items.Sum(x => x.Price);
        }

        public static void CalculateSubTotal(this Order order)
        {
            foreach (var storeOrder in order.StoreOrders)
            {
                storeOrder.CalculateSubTotal();
            }
            order.Subtotal = order.StoreOrders.Sum(x => x.Subtotal);
        }

        public static void CalculateTotal(this Order order)
        {
            order.CalculateSubTotal();
            order.ServiceFee = 0;
            order.DeliveryFee = order.Subtotal >= BasketSettings.Settings.FreeDeliveryThreshold ? 0 : BasketSettings.Settings.DeliveryFee;
            //order.Subtotal = order.Subtotal + order.DeliveryFee;
            //order.Tax = (order.Subtotal / 100) * BasketSettings.Tax; //VAT in %
            order.Total = order.Subtotal + order.Tax + order.DeliveryFee;
        }

        public static async Task GetRandomActivationCode(this UserSubscriptions userSubscription)
        {
            var crypto = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var bytes = new byte[5];
            crypto.GetBytes(bytes);
            userSubscription.ActivationCode = BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
    }
}