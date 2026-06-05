using Microsoft.AspNetCore.SignalR;

namespace Web_Payment.Hubs
{
    public class InvoiceHub : Hub
    {
        /// <summary>
        /// WPF client sẽ lắng nghe event "PaymentResult".
        /// Khi user confirm/cancel trên web → server gọi method này broadcast tới tất cả client.
        /// </summary>
        public async Task NotifyPaymentResult(long invoiceId, string status)
        {
            await Clients.All.SendAsync("PaymentResult", invoiceId, status);
        }
    }
}