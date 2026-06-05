using BUS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web_Payment.Hubs;

namespace Web_Payment.Controllers
{
    public class PaymentController : Controller
    {
        private readonly BUS_Invoice _invoiceBus = new BUS_Invoice();
        private readonly IHubContext<InvoiceHub> _hubContext;

        public PaymentController(IHubContext<InvoiceHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // =====================================================
        // GET /payment/{invoiceId}
        // Hiển thị trang thanh toán
        // =====================================================

        [HttpGet("payment/{invoiceId}")]
        public IActionResult Index(long invoiceId)
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var invoice = _invoiceBus.GetInvoiceFull(invoiceId, baseUrl);

            if (invoice == null)
                return View("NotFound");

            return View(invoice);
            //return Content($"Invoice Id = {invoiceId}");
        }

        // =====================================================
        // POST /payment/{invoiceId}/confirm
        // Xác nhận thanh toán
        // =====================================================

        [HttpPost("payment/{invoiceId}/confirm")]
        public async Task<IActionResult> Confirm(long invoiceId)
        {
            string error;
            bool success = _invoiceBus.ConfirmPayment(invoiceId, out error);

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { invoiceId });
            }

            // Gửi SignalR notify tới WPF client
            await _hubContext.Clients.All.SendAsync("PaymentResult", invoiceId, "Paid");

            TempData["Success"] = "Payment confirmed successfully!";
            return RedirectToAction("Result", new { invoiceId, status = "Paid" });
        }

        // =====================================================
        // POST /payment/{invoiceId}/cancel
        // Hủy thanh toán
        // =====================================================

        [HttpPost("payment/{invoiceId}/cancel")]
        public async Task<IActionResult> Cancel(long invoiceId)
        {
            string error;
            bool success = _invoiceBus.CancelPayment(invoiceId, out error);

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { invoiceId });
            }

            // Gửi SignalR notify tới WPF client
            await _hubContext.Clients.All.SendAsync("PaymentResult", invoiceId, "Cancelled");

            TempData["Success"] = "Payment has been cancelled.";
            return RedirectToAction("Result", new { invoiceId, status = "Cancelled" });
        }

        // =====================================================
        // GET /payment/{invoiceId}/result
        // Trang kết quả sau confirm/cancel
        // =====================================================

        [HttpGet("payment/{invoiceId}/result")]
        public IActionResult Result(long invoiceId, string status)
        {
            ViewBag.InvoiceId = invoiceId;
            ViewBag.Status = status;
            return View();
        }
    }
}
