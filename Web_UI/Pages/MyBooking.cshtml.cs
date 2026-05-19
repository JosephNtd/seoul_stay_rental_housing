using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_UI.Models;

namespace Web_UI.Pages
{
    public class MyBookingModel : PageModel
    {
        private readonly SeoulStayContext _context;
        [BindProperty]
        public int Rating { get; set; }

        [BindProperty]
        public string Comment { get; set; } = "";
        public MyBookingModel(SeoulStayContext context)
        {
            _context = context;
        }

        public List<Booking> Bookings { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var guestIdString =
                HttpContext.Session.GetString("GuestId");

            if (string.IsNullOrEmpty(guestIdString))
            {
                return RedirectToPage("/Index");
            }

            long guestId =
                long.Parse(guestIdString);

            Bookings = await _context.Bookings
                .Include(b => b.Item)
                    .ThenInclude(i => i.ItemPictures)

                .Include(b => b.Item)
                    .ThenInclude(i => i.Area)

                .Include(b => b.Reviews)

                .Where(b => b.GuestUserId == guestId)

                .OrderByDescending(b => b.BookingDate)

                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult>
    OnPostCancelBookingAsync(long bookingId)
        {
            var guestIdString =
                HttpContext.Session.GetString("GuestId");

            if (string.IsNullOrEmpty(guestIdString))
            {
                return RedirectToPage("/Index");
            }

            long guestId =
                long.Parse(guestIdString);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.Id == bookingId &&
                    b.GuestUserId == guestId);

            if (booking == null)
            {
                return RedirectToPage();
            }

            booking.BookingStatus = "Cancelled";

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSubmitReviewAsync(
        long bookingId,
        int rating,
        string comment)
        {
            var guestIdString =
                HttpContext.Session.GetString("GuestId");

            if (string.IsNullOrEmpty(guestIdString))
            {
                return RedirectToPage("/Index");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] =
                    "Please enter a review comment.";

                return RedirectToPage();
            }

            long guestId =
                long.Parse(guestIdString);

            // LOAD BOOKING

            var booking = await _context.Bookings
                .Include(b => b.Item)
                .FirstOrDefaultAsync(b =>
                    b.Id == bookingId &&
                    b.GuestUserId == guestId);

            if (booking == null)
            {
                TempData["Error"] =
                    "Booking not found.";

                return RedirectToPage();
            }

            // BLOCK CANCELLED BOOKING

            if (booking.BookingStatus == "Cancelled")
            {
                TempData["Error"] =
                    "Cancelled bookings cannot be reviewed.";

                return RedirectToPage();
            }

            // ONLY AFTER CHECKOUT

            if (booking.CheckOutDate >
                DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["Error"] =
                    "You can review after checkout only.";

                return RedirectToPage();
            }

            // PREVENT DUPLICATE REVIEW

            bool alreadyReviewed =
                await _context.Reviews
                    .AnyAsync(r =>
                        r.BookingId == bookingId &&
                        r.ReviewerId == guestId);

            if (alreadyReviewed)
            {
                TempData["Error"] =
                    "You already reviewed this booking.";

                return RedirectToPage();
            }

            // VALIDATE RATING

            if (rating < 1 || rating > 5)
            {
                TempData["Error"] =
                    "Invalid rating.";

                return RedirectToPage();
            }

            // CREATE REVIEW

            var review = new Review
            {
                Guid = Guid.NewGuid(),

                BookingId = booking.Id,

                ReviewerId = guestId,

                // OPTIONAL:
                // host / owner user id
                RevieweeId = booking.Item.HostUserId,

                ItemId = booking.ItemId,

                Rating = (byte)rating,

                Comment = comment,

                CreatedDate = DateTime.Now,

                IsActive = true
            };

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Review submitted successfully.";

            return RedirectToPage();
        }
    }
}