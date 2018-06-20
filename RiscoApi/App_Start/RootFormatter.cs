using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Converters;
using System.Web.Http;

namespace BasketApi
{
    // based on http://www.west-wind.com/weblog/posts/2012/Apr/02/Creating-a-JSONP-Formatter-for-ASPNET-Web-API

    public class RootFormatter : JsonMediaTypeFormatter
    {
        private string RootFieldName = null;

        public RootFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            // SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, System.Net.Http.HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var formatter = new RootFormatter()
            {
                RootFieldName = GetRootFieldName(type)
            };

            // this doesn't work unfortunately
            //formatter.SerializerSettings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;

            // You have to reapply any JSON.NET default serializer Customizations here    
            formatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            formatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            return formatter;
        }

        public override Task WriteToStreamAsync(Type type, object value,
                                                Stream stream,
                                                HttpContent content,
                                                TransportContext transportContext)
        {
            if (string.IsNullOrEmpty(RootFieldName))
                return base.WriteToStreamAsync(type, value, stream, content, transportContext);

            StreamWriter writer = null;

            // write the pre-amble
            try
            {
                writer = new StreamWriter(stream);
                writer.Write("{\"" + RootFieldName + "\":");
                writer.Flush();
            }
            catch (Exception ex)
            {
                try
                {
                    if (writer != null)
                        writer.Dispose();
                }
                catch { }

                var tcs = new TaskCompletionSource<object>();
                tcs.SetException(ex);
                return tcs.Task;
            }

            return base.WriteToStreamAsync(type, value, stream, content, transportContext)
                       .ContinueWith(innerTask =>
                       {
                           if (innerTask.Status == TaskStatus.RanToCompletion)
                           {
                               writer.Write("}");
                               writer.Flush();
                           }
                       }, TaskContinuationOptions.ExecuteSynchronously)
                        .ContinueWith(innerTask =>
                        {
                            writer.Dispose();
                            return innerTask;

                        }, TaskContinuationOptions.ExecuteSynchronously)
                        .Unwrap();
        }

        protected string GetRootFieldName(Type type)
        {
            var attrs =
                from x in type.CustomAttributes
                where x.AttributeType == typeof(Newtonsoft.Json.JsonObjectAttribute)
                select x;
            if (attrs.Count() < 1)
            {
                return null;
            } // if

            var titles =
                from arg in attrs.First().NamedArguments
                where arg.MemberName == "Title"
                select arg.TypedValue.Value.ToString();
            if (titles.Count() < 1)
            {
                return null;
            } // if
            return titles.First();
        }
    }
}