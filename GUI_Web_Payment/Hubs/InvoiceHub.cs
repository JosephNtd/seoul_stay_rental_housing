using Microsoft.AspNet.SignalR;

namespace GUI_Web_Payment.Hubs
{
    public class InvoiceHub : Hub
    {
        /// <summary>
        /// WPF client lắng nghe event "paymentResult".
        /// </summary>
        public void NotifyPaymentResult(long invoiceId, string status)
        {
            Clients.All.paymentResult(invoiceId, status);
        }
    }
}