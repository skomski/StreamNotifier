using System.Diagnostics.Contracts;
using Prowlin;

namespace Helper.Extensions
{
    static class ProwlinExtensions
    {
        public static void SendNotification(this ProwlClient prowlClient,string eventTitle, string description, string url, string apikey)
        {
            Contract.Requires(prowlClient != null);
            Contract.Requires(eventTitle.NotEmpty());
            Contract.Requires(description.NotEmpty());
            Contract.Requires(url.IsUri());
            Contract.Requires(apikey.NotEmpty());

            var notification = new Notification
            {
                Application = System.Windows.Forms.Application.ProductName,
                Description = description,
                Event = eventTitle,
                Priority = NotificationPriority.High,
                Url = url
            };
            notification.AddApiKey(apikey);
            prowlClient.SendNotification(notification);
        }
    }
}
