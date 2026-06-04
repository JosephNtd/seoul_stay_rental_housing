using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace DAL
{
    public class DAL_Booking
    {

        // GET BOOKING CARDS
        public List<DTO_BookingCard> GetBookingCards()
        {
            using (var db =
                new Seoul_StayDataContext())
            {
                var query = from b in db.Bookings

                            join g in db.Guests
                                on b.GuestUserID
                                equals g.UserID

                            join u in db.Users
                                on g.UserID
                                equals u.ID

                            join i in db.Items
                                on b.ItemID
                                equals i.ID

                            join it in db.ItemTypes
                                on i.ItemTypeID
                                equals it.ID

                            join a in db.Areas
                                on i.AreaID
                                equals a.ID

                            join h in db.Users
                                on i.HostUserID
                                equals h.ID

                            join t in db.Transactions
                                on b.TransactionID
                                equals t.ID
                                into transJoin

                            from trans in
                                transJoin.DefaultIfEmpty()

                            select new DTO_BookingCard
                            {
                                BookingID = b.ID,
                                BookingGUID = b.GUID,
                                GuestUserID = b.GuestUserID,
                                ItemID = b.ItemID,
                                TransactionID = b.TransactionID,

                                // GUEST
                                GuestFullName = u.FullName,
                                GuestEmail = u.Email,
                                GuestPhone = u.PhoneNumber,
                                GuestCountry = u.Country,
                                GuestAvatar = u.ProfilePicture,
                                IsGuestVerified = g.NationalIDVerified,

                                // LISTING
                                ListingTitle = i.Title,
                                ListingType = it.Name,
                                ListingAddress = i.ApproximateAddress,
                                HostName = h.FullName,

                                // BOOKING
                                CheckInDate = b.CheckInDate,
                                CheckOutDate = b.CheckOutDate,
                                NumberOfGuests = b.NumberOfGuests,
                                TotalNights = (int)(b.CheckOutDate - b.CheckInDate).TotalDays,
                                BookingDate = b.BookingDate,
                                SpecialRequests = b.SpecialRequests,

                                // PRICING
                                PricePerNight = b.PricePerNight,
                                TotalPrice = b.TotalPrice,
                                DiscountAmount = b.DiscountAmount,
                                FinalPrice = b.FinalPrice,

                                // STATUS
                                BookingStatus = b.BookingStatus,

                                // IMAGE
                                ListingThumbnail = db.ItemPictures
                                    .Where(x => x.ItemID == i.ID)
                                    .OrderBy(x => x.DisplayOrder)
                                    .Select(x => x.PictureFileName)
                                    .FirstOrDefault()
                            };

                return query.OrderByDescending(x => x.BookingDate).ToList();
            }
        }

        // GET BOOKING DETAILS
        public DTO_BookingDetails GetBookingDetails(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = (from b in db.Bookings

                            join g in db.Guests
                               on b.GuestUserID
                               equals g.UserID

                            join guestUser in db.Users
                               on g.UserID
                               equals guestUser.ID

                            join i in db.Items
                               on b.ItemID
                               equals i.ID

                            join it in db.ItemTypes
                               on i.ItemTypeID
                               equals it.ID

                            join area in db.Areas
                               on i.AreaID
                               equals area.ID

                            join hostUser in db.Users
                               on i.HostUserID
                               equals hostUser.ID

                            join host in db.Hosts
                               on hostUser.ID
                               equals host.UserID
                               into hostJoin

                            from hostData in hostJoin.DefaultIfEmpty()

                            join cp in
                               db.CancellationPolicies
                               on b.CancellationPolicyID
                               equals cp.ID

                            join trans in
                               db.Transactions
                               on b.TransactionID
                               equals trans.ID
                               into transJoin

                            from t in transJoin.DefaultIfEmpty()

                            where b.ID == bookingId

                            select new DTO_BookingDetails
                            {
                                // CORE

                                BookingID = b.ID,
                                BookingGUID = b.GUID,
                                BookingDate = b.BookingDate,

                                // GUEST
                                GuestUserID = guestUser.ID,
                                GuestFullName = guestUser.FullName,
                                GuestEmail = guestUser.Email,
                                GuestPhone = guestUser.PhoneNumber,
                                GuestCountry = guestUser.Country,
                                GuestBirthDate = guestUser.BirthDate,
                                GuestAvatar = guestUser.ProfilePicture,
                                IsGuestVerified = g.NationalIDVerified,
                                LoyaltyPoints = g.LoyaltyPoints,
                                PreferredLanguage = g.PreferredLanguage,
                                NationalID = g.NationalID,

                                // LISTING
                                ItemID = i.ID,
                                ListingTitle = i.Title,
                                ListingType = it.Name,
                                ListingAddress = i.ExactAddress,
                                ListingDescription = i.Description,
                                AreaName = area.Name,
                                Capacity = i.Capacity,
                                NumberOfBeds = i.NumberOfBeds,
                                NumberOfBedrooms = i.NumberOfBedrooms,
                                NumberOfBathrooms = i.NumberOfBathrooms,
                                HostRules = i.HostRules,

                                // HOST
                                HostUserID = hostUser.ID,
                                HostName = hostUser.FullName,
                                HostEmail = hostUser.Email,
                                HostPhone = hostUser.PhoneNumber,
                                HostAvatar = hostUser.ProfilePicture,
                                IsHostVerified = hostData != null && hostData.IsVerified,
                                HostRating = hostData != null ? hostData.Rating : null,

                                // BOOKING
                                CheckInDate = b.CheckInDate,
                                CheckOutDate = b.CheckOutDate,
                                NumberOfGuests = b.NumberOfGuests,
                                TotalNights = (int)(b.CheckOutDate - b.CheckInDate).TotalDays,
                                SpecialRequests = b.SpecialRequests,
                                BookingStatus = b.BookingStatus,

                                // PRICING
                                PricePerNight = b.PricePerNight,
                                TotalPrice = b.TotalPrice,
                                DiscountAmount = b.DiscountAmount,
                                FinalPrice = b.FinalPrice,

                                //PAYMENT
                                TransactionID = b.TransactionID,
                                TransactionDate = t != null ? (DateTime?)t.TransactionDate : null,
                                TransactionAmount = t != null ? (decimal?)t.Amount : null,
                                GatewayReturnID = t != null ? t.GatewayReturnID : null,

                                // POLICY
                                CancellationPolicyID = cp.ID,
                                CancellationPolicyName = cp.Name,
                                PlatformCommissionRate = cp.PlatformCommissionRate,

                                // THUMBNAIL
                                ListingThumbnail = db.ItemPictures
                                   .Where(x => x.ItemID == i.ID)
                                   .OrderBy(x => x.DisplayOrder)
                                   .Select(x => x.PictureFileName)
                                   .FirstOrDefault()
                            }).FirstOrDefault();

                if (data == null)
                    return null;

                // TIMELINE

                data.Timeline = GetBookingTimeline(bookingId);

                // NIGHTS

                data.Nights = GetBookingNights(bookingId);

                // AMENITIES

                data.Amenities = (from ia in db.ItemAmenities
                                  join am in db.Amenities
                                     on ia.AmenityID
                                     equals am.ID
                                  where ia.ItemID ==
                                        data.ItemID
                                  select am.Name)
                    .ToList();

                // ATTRACTIONS

                data.Attractions = (from iad in db.ItemAttractions join at in db.Attractions on iad.AttractionID equals at.ID where iad.ItemID == data.ItemID select at.Name).ToList();

                // PICTURES

                data.Pictures = db.ItemPictures.Where(x => x.ItemID == data.ItemID).OrderBy(x => x.DisplayOrder).Select(x => x.PictureFileName).ToList();

                return data;
            }
        }


        // GET TIMELINE
        public List<DTO_BookingTimeline> GetBookingTimeline(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query = from h in db.BookingStatusHistories

                            join u in db.Users
                                on h.ChangedByUserID
                                equals u.ID
                                into userJoin

                            from usr in userJoin.DefaultIfEmpty()

                            where h.BookingID == bookingId

                            orderby h.ChangedDate descending

                            select new DTO_BookingTimeline
                            {
                                ID = h.ID,
                                GUID = h.GUID,
                                BookingID = h.BookingID,
                                OldStatus = h.OldStatus,
                                NewStatus = h.NewStatus,
                                ChangedDate = h.ChangedDate,
                                ChangedByUserID = h.ChangedByUserID,
                                ChangedByName = usr != null ? usr.FullName : "System",
                                ChangedByAvatar = usr != null ? usr.ProfilePicture : null,
                                Notes = h.Notes
                            };

                return query.ToList();
            }
        }


        // GET BOOKING NIGHTS
        public List<DTO_BookingNight>
    GetBookingNights(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query = from d in db.BookingDetails

                            join ip in db.ItemPrices
                                on d.ItemPriceID
                                equals ip.ID

                            join cp in
                                db.CancellationPolicies
                                on d.RefundCancellationPolicyID
                                equals cp.ID
                                into policyJoin

                            from policy in policyJoin.DefaultIfEmpty()

                            where d.BookingID == bookingId
                            orderby ip.Date

                            select new DTO_BookingNight
                            {
                                BookingDetailID = d.ID,
                                BookingDetailGUID = d.GUID,
                                BookingID = d.BookingID,
                                ItemPriceID = d.ItemPriceID,
                                Date = ip.Date,
                                BasePrice = ip.Price,
                                IsRefund = d.isRefund,
                                RefundDate = d.RefundDate,
                                RefundCancellationPolicyID = d.RefundCancellationPolicyID,
                                RefundPolicyName = policy != null ? policy.Name : null
                            };

                return query.ToList();
            }
        }


        // GET STATS
        public DTO_BookingStats GetBookingStats()
        {
            using (var db = new Seoul_StayDataContext())
            {
                DateTime today = DateTime.Today;
                var bookings = db.Bookings.ToList();
                DTO_BookingStats stats = new DTO_BookingStats();
                stats.TotalBookings = bookings.Count;
                stats.PendingBookings = bookings.Count(x => x.BookingStatus == "Pending");
                stats.ConfirmedBookings = bookings.Count(x => x.BookingStatus == "Confirmed");
                stats.CheckedInBookings = bookings.Count(x => x.BookingStatus == "CheckedIn");
                stats.CompletedBookings = bookings.Count(x => x.BookingStatus == "Completed");
                stats.CancelledBookings = bookings.Count(x => x.BookingStatus == "Cancelled");
                stats.TodayCheckIns = bookings.Count(x => x.CheckInDate.Date == today);
                stats.TodayCheckOuts = bookings.Count(x => x.CheckOutDate.Date == today);
                stats.ActiveStays = bookings.Count(x => x.CheckInDate.Date <= today && x.CheckOutDate.Date >= today && x.BookingStatus != "Cancelled");
                stats.TotalRevenue = bookings.Where(x => x.BookingStatus != "Cancelled").Sum(x => x.FinalPrice);
                stats.TodayRevenue = bookings.Where(x => x.BookingDate.Date == today).Sum(x => x.FinalPrice);
                stats.MonthlyRevenue = bookings.Where(x => x.BookingDate.Month == today.Month && x.BookingDate.Year == today.Year).Sum(x => x.FinalPrice);
                int totalListings = db.Items.Count();
                stats.TotalAvailableListings = totalListings;
                stats.OccupiedListings = bookings.Where(x => x.CheckInDate.Date <= today && x.CheckOutDate.Date >= today && x.BookingStatus != "Cancelled").Select(x => x.ItemID).Distinct().Count();
                stats.RefundedBookings = bookings.Count(x => x.BookingStatus == "Refunded");
                if (totalListings > 0)
                {
                    stats.OccupancyRate = ((decimal)stats.OccupiedListings / totalListings) * 100;
                }

                return stats;
            }
        }

        // SEARCH
        public List<DTO_BookingCard> SearchBookings(string keyword)
        {
            var data = GetBookingCards();

            if (string.IsNullOrWhiteSpace(keyword))
                return data;

            keyword = keyword.ToLower().Trim();

            return data.FindAll(x => x.BookingCode.ToLower().Contains(keyword)
                            || x.GuestFullName.ToLower().Contains(keyword)
                            || x.GuestEmail.ToLower().Contains(keyword)
                            || x.GuestPhone.ToLower().Contains(keyword)
                            || x.ListingTitle.ToLower().Contains(keyword));
        }


        // FILTER
        public List<DTO_BookingCard> FilterBookings(string status)
        {
            var data = GetBookingCards();

            if (string.IsNullOrWhiteSpace(status) || status == "All")
                return data;

            return data.FindAll(x => x.BookingStatus == status);
        }
        public bool UpdateBookingStatus(long bookingId, string newStatus)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var booking = db.Bookings.FirstOrDefault(x => x.ID == bookingId);

                    if (booking == null)
                        return false;

                    booking.BookingStatus = newStatus;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool InsertBookingTimeline(long bookingId, string oldStatus, string newStatus, long? changedByUserId, string notes)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var timeline = new BookingStatusHistory
                    {
                        GUID = Guid.NewGuid(),
                        BookingID = bookingId,
                        OldStatus = oldStatus,
                        NewStatus = newStatus,
                        ChangedDate = DateTime.Now,
                        ChangedByUserID = changedByUserId,
                        Notes = notes
                    };
                    db.BookingStatusHistories.InsertOnSubmit(timeline);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public Booking GetBookingEntity(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Bookings.FirstOrDefault(x => x.ID == bookingId);
            }
        }
        public bool ValidateBookingOverlap(long itemId, DateTime checkIn, DateTime checkOut)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return !db.Bookings.Any(x => x.ItemID == itemId &&
                                        x.BookingStatus != "Cancelled" &&
                                        x.BookingStatus != "Refunded" &&
                                        checkIn < x.CheckOutDate &&
                                        checkOut > x.CheckInDate);
            }
        }
        public long CreateManualBooking(DTO_CreateBooking dto)
        {
            using (var scope = new TransactionScope())
            {
                using (var db = new Seoul_StayDataContext())
                {
                    if (!ValidateBookingOverlap(dto.ItemID, dto.CheckInDate, dto.CheckOutDate))
                    {
                        return 0;
                    }

                    long? transactionId = null;

                    if (dto.IsPaid || dto.IsDeposit)
                    {
                        transactionId = CreateTransaction(db, dto);
                    }

                    var booking = new Booking
                    {
                        GUID = Guid.NewGuid(),
                        GuestUserID = dto.GuestUserID,
                        ItemID = dto.ItemID,
                        CheckInDate = dto.CheckInDate.Date,
                        CheckOutDate = dto.CheckOutDate.Date,
                        NumberOfGuests = dto.NumberOfGuests,
                        PricePerNight = dto.TotalNights == 0 ? 0 : dto.BaseAmount / dto.TotalNights,
                        TotalPrice = dto.BaseAmount,
                        DiscountAmount = dto.DiscountAmount,
                        FinalPrice = dto.FinalAmount,
                        CancellationPolicyID = dto.CancellationPolicyID,
                        BookingStatus = dto.IsPaid ? "Confirmed" : "Pending",
                        BookingDate = DateTime.Now,
                        SpecialRequests = dto.SpecialRequests,
                        TransactionID = transactionId
                    };

                    db.Bookings.InsertOnSubmit(booking);

                    db.SubmitChanges();

                    CreateBookingDetails(db, booking.ID, dto);

                    if (dto.CouponID.HasValue)
                    {
                        CreateBookingCoupon(db, booking.ID, dto);
                    }

                    CreateBookingTimeline(db, booking.ID, null, booking.BookingStatus, dto.CreatedByUserID, "Walk-in booking created");
                    db.SubmitChanges();
                    scope.Complete();
                    return booking.ID;
                }
            }
        }
        private long CreateTransaction(Seoul_StayDataContext db, DTO_CreateBooking dto)
        {
            var transaction = new Transaction
            {
                GUID = Guid.NewGuid(),
                UserID = dto.GuestUserID,
                TransactionTypeID = dto.TransactionTypeID,
                Amount = dto.IsDeposit ? dto.DepositAmount : dto.FinalAmount,
                TransactionDate = DateTime.Today,
                GatewayReturnID = Guid.NewGuid().ToString("N")
            };

            db.Transactions.InsertOnSubmit(transaction);
            db.SubmitChanges();
            return transaction.ID;
        }
        private void CreateBookingDetails(Seoul_StayDataContext db, long bookingId, DTO_CreateBooking dto)
        {
            foreach (var night in dto.Nights)
            {
                var detail = new BookingDetail
                {
                    GUID = Guid.NewGuid(),
                    BookingID = bookingId,
                    ItemPriceID = night.ItemPriceID,
                    isRefund = false
                };

                db.BookingDetails.InsertOnSubmit(detail);
            }
        }
        private void CreateBookingCoupon(Seoul_StayDataContext db, long bookingId, DTO_CreateBooking dto)
        {
            var bookingCoupon = new BookingCoupon
            {
                GUID = Guid.NewGuid(),
                BookingID = bookingId,
                CouponID = dto.CouponID.Value,
                DiscountApplied = dto.DiscountAmount,
                AppliedDate = DateTime.Now
            };

            db.BookingCoupons.InsertOnSubmit(bookingCoupon);

            // Tăng số lần sử dụng coupon trong cùng transaction
            var coupon = db.Coupons.FirstOrDefault(c => c.ID == dto.CouponID.Value);
            if (coupon != null)
            {
                coupon.CurrentUsageCount += 1;
            }
        }
        private void CreateBookingTimeline(Seoul_StayDataContext db, long bookingId, string oldStatus, string newStatus, long changedByUserId, string notes)
        {
            var timeline = new BookingStatusHistory
            {
                GUID = Guid.NewGuid(),
                BookingID = bookingId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedDate = DateTime.Now,
                ChangedByUserID = changedByUserId,
                Notes = notes
            };

            db.BookingStatusHistories.InsertOnSubmit(timeline);
        }
    }
}