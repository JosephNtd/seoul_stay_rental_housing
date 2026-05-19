using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web_UI.Models;
using Web_UI.Models.ViewModels;

namespace Web_UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SeoulStayContext _context;
        public List<HomeStayCardVM> Listings { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, SeoulStayContext context)
        {
            _logger = logger;
            _context = context;
        }
        public bool IsLoggedIn =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("GuestId"));

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public long? AreaId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckIn { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckOut { get; set; }

        [BindProperty(SupportsGet = true)]
        public int GuestCount { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";
        [BindProperty]
        public long BookingItemId { get; set; }

        [BindProperty]
        public DateTime BookingCheckIn { get; set; }

        [BindProperty]
        public DateTime BookingCheckOut { get; set; }

        [BindProperty]
        public int BookingGuests { get; set; }

        [BindProperty]
        public string CouponCode { get; set; } = "";

        public string LoginError { get; set; } = "";

        public async Task OnGetAsync()
        {
            var query = _context.Items
                .Include(i => i.Area)
                .Include(i => i.ItemPictures)
                .Include(i => i.ItemPrices)
                .Where(i => i.IsActive)
                .AsQueryable();

            // Keyword search
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                query = query.Where(i =>
                    i.Title.Contains(Keyword) ||
                    i.ApproximateAddress.Contains(Keyword) ||
                    i.ExactAddress.Contains(Keyword));
            }

            // Area filter
            if (AreaId.HasValue)
            {
                query = query.Where(i => i.AreaId == AreaId.Value);
            }

            // Capacity filter
            query = query.Where(i => i.Capacity >= GuestCount);

            var data = await query.ToListAsync();

            // Availability filter
            if (CheckIn.HasValue && CheckOut.HasValue)
            {
                var checkInDateOnly = DateOnly.FromDateTime(CheckIn.Value);
                var checkOutDateOnly = DateOnly.FromDateTime(CheckOut.Value);

                data = data.Where(item =>
                {
                    bool hasConflict = _context.Bookings
                        .Any(b =>
                            b.ItemId == item.Id &&
                            checkInDateOnly < b.CheckOutDate &&
                            checkOutDateOnly > b.CheckInDate);

                    return !hasConflict;
                }).ToList();
            }

            Listings = data.Select(i => new HomeStayCardVM
            {
                ItemId = i.Id,
                Title = i.Title,
                ApproximateAddress = i.ApproximateAddress,
                AreaName = i.Area?.Name ?? "",
                Capacity = i.Capacity,
                PricePerNight = i.ItemPrices
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefault()?.Price ?? 0,

                ThumbnailUrl = i.ItemPictures.FirstOrDefault() != null
                    ? "/images/homestays/" +
                      i.ItemPictures.First().PictureFileName
                    : "/images/unnamed.png",

                Rating = 4.8
            }).ToList();

            // Price filter
            if (MinPrice.HasValue)
            {
                Listings = Listings
                    .Where(x => x.PricePerNight >= MinPrice.Value)
                    .ToList();
            }

            if (MaxPrice.HasValue)
            {
                Listings = Listings
                    .Where(x => x.PricePerNight <= MaxPrice.Value)
                    .ToList();
            }
        }
        public async Task<IActionResult> OnPostLoginAsync()
        {
            var guest = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Username == Username &&
                    x.Password != null &&
                    x.Password == Password);

            if (guest == null)
            {
                LoginError = "Invalid username or password";

                await OnGetAsync();

                return Page();
            }

            HttpContext.Session.SetString("GuestId", guest.Id.ToString());

            HttpContext.Session.SetString("GuestName", guest.FullName);

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostCreateBookingAsync()
        {
            var guestIdString =
    HttpContext.Session.GetString("GuestId");

            if (string.IsNullOrEmpty(guestIdString))
            {
                return RedirectToPage();
            }

            long guestId = long.Parse(guestIdString);
            var checkIn =
                DateOnly.FromDateTime(BookingCheckIn);

            var checkOut =
                DateOnly.FromDateTime(BookingCheckOut);
            // DATE VALIDATION
            if (BookingCheckOut <= BookingCheckIn)
            {
                TempData["Error"] =
                    "Check-out must be after check-in";

                return RedirectToPage();
            }



            // ITEM VALIDATION
            var item = await _context.Items
                .FirstOrDefaultAsync(i =>
                    i.Id == BookingItemId);

            if (item == null)
            {
                return RedirectToPage();
            }



            // CAPACITY VALIDATION
            if (BookingGuests > item.Capacity)
            {
                TempData["Error"] =
                    "Guest count exceeds capacity";

                return RedirectToPage();
            }
            bool hasConflict = await _context.Bookings
                .AnyAsync(b =>
                    b.ItemId == BookingItemId &&
                    b.BookingStatus != "Cancelled" &&
                    checkIn < b.CheckOutDate &&
                    checkOut > b.CheckInDate);
            if (hasConflict)
            {
                TempData["Error"] =
                    "This stay is no longer available";

                return RedirectToPage();
            }
            decimal pricePerNight = await _context.ItemPrices
                .Where(p => p.ItemId == BookingItemId)
                .OrderByDescending(p => p.Date)
                .Select(p => p.Price)
                .FirstOrDefaultAsync();

            int nights =
                (BookingCheckOut - BookingCheckIn).Days;

            decimal subtotal =
                pricePerNight * nights;

            decimal total = subtotal;
            if (!string.IsNullOrWhiteSpace(CouponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c =>
                        c.CouponCode == CouponCode &&
                        c.IsActive == true);

                if (coupon != null)
                {
                    total -= coupon.MaximumDiscountAmount;
                }
            }
            var booking = new Booking
            {
                GuestUserId = guestId,
                ItemId = BookingItemId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = BookingGuests,
                TotalPrice = total,
                BookingStatus = "Confirmed",
                FinalPrice = pricePerNight * (checkOut.DayNumber - checkIn.DayNumber),
                CancellationPolicyId = 1
            };

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            return RedirectToPage("/MyBooking");
        }


    }
}
