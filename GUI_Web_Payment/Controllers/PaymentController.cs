using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using BUS;
using Microsoft.AspNet.SignalR;
using GUI_Web_Payment.Hubs;

namespace GUI_Web_Payment.Controllers
{
    public class PaymentController : Controller
    {
        private readonly BUS_Invoice _invoiceBus = new BUS_Invoice();

        // GET: /Payment/{invoiceId}
        [HttpGet]
        [Route("payment/{invoiceId}")]
        public ActionResult Index(long invoiceId)
        {
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var invoice = _invoiceBus.GetInvoiceFull(invoiceId, baseUrl);

            if (invoice == null)
                return View("NotFound");

            return View(invoice);
        }

        // POST: /Payment/{invoiceId}/confirm
        [HttpPost]
        [Route("payment/{invoiceId}/confirm")]
        public ActionResult Confirm(long invoiceId)
        {
            string error;
            bool success = _invoiceBus.ConfirmPayment(invoiceId, out error);

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { invoiceId });
            }

            // Gửi SignalR notify tới WPF client
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<InvoiceHub>();
            hubContext.Clients.All.paymentResult(invoiceId, "Paid");

            TempData["Success"] = "Payment confirmed successfully!";
            return RedirectToAction("Result", new { invoiceId, status = "Paid" });
        }

        // POST: /Payment/{invoiceId}/cancel
        [HttpPost]
        [Route("payment/{invoiceId}/cancel")]
        public ActionResult Cancel(long invoiceId)
        {
            string error;
            bool success = _invoiceBus.CancelPayment(invoiceId, out error);

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { invoiceId });
            }

            // Gửi SignalR notify tới WPF client
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<InvoiceHub>();
            hubContext.Clients.All.paymentResult(invoiceId, "Cancelled");

            TempData["Success"] = "Payment has been cancelled.";
            return RedirectToAction("Result", new { invoiceId, status = "Cancelled" });
        }

        // GET: /Payment/{invoiceId}/result
        [HttpGet]
        [Route("payment/{invoiceId}/result")]
        public ActionResult Result(long invoiceId, string status)
        {
            ViewBag.InvoiceId = invoiceId;
            ViewBag.Status = status;
            return View();
        }
    }
}