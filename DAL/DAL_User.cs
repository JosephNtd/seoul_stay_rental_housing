using DTO;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_User
    {
        public User Login(string username, string password)
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
            {
                return db.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.Password == password &&
                x.IsActive
                );
            }
        }

        public bool CheckUsername(string username)
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
                return db.Users.Any(x => x.Username == username);
        }
        public bool CheckEmail(string email)
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
                return db.Users.Any(x => x.Email == email);
        }

        public bool Register(User newUser)
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
            {
                try
                {
                    newUser.GUID = Guid.NewGuid();
                    newUser.IsAdmin = false;
                    newUser.CreatedDate = DateTime.Now;
                    newUser.IsActive = true;

                    db.Users.InsertOnSubmit(newUser);
                    db.SubmitChanges();
                    Guest guest = new Guest
                    {
                        UserID = newUser.ID,
                        LoyaltyPoints = 0,
                        PreferredLanguage = "en",
                        NationalIDVerified = false
                    };

                    db.Guests.InsertOnSubmit(guest);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public User GetById(long id)
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
                return db.Users.FirstOrDefault(u => u.ID == id);
        }

		public List<DTO_UserDisplay> GetAllUsersDisplay()
		{
			using (Seoul_StayDataContext db = new Seoul_StayDataContext())
			{
				var data =
					(from u in db.Users

					 join h in db.Hosts
						on u.ID equals h.UserID
						into hostJoin

					 from h in hostJoin.DefaultIfEmpty()

					 select new
					 {
						 u.ID,
						 u.FullName,
						 u.Email,
						 u.Username,
						 u.Country,
						 u.IsActive,
						 u.IsAdmin,
						 u.CreatedDate,
						 Host = h,

						 // COUNT BOOKINGS
						 TotalBookings =
							db.Bookings.Count(b =>
								b.GuestUserID == u.ID)
					 })
					 .ToList();

				return data.Select(u =>
					new DTO_UserDisplay
					{
						UserID = u.ID,

						FullName = u.FullName,

						Email = u.Email,

						Username = u.Username,

						Country = u.Country,

						Status =
							u.IsActive
							? "Active"
							: "Locked",

						Role =
							u.IsAdmin
							? "Administrator"
							: u.Host != null
								? "Host"
								: "Guest",

						LastActive =
							u.CreatedDate
							.ToString("MMMM dd, yyyy"),

						// FIX
						TotalBookings =
							u.TotalBookings
					})
					.ToList();
			}
		}

		public bool ToggleLock(long userId)
        {
            try
            {
                using (Seoul_StayDataContext db = new Seoul_StayDataContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.ID == userId);
                    if (user == null) return false;

                    user.IsActive = !user.IsActive;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteUser(long userId)
        {
            try
            {
                using (Seoul_StayDataContext db = new Seoul_StayDataContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.ID == userId);
                    if (user == null) return false;

                    user.IsActive = false; // Soft Delete
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // THÊM MỚI VÀ CẬP NHẬT ĐỒNG BỘ THEO CONFIG CỦA DB
        public bool InsertUser(User user, string role)
        {
            try
            {
                using (Seoul_StayDataContext db = new Seoul_StayDataContext())
                {
                    // 1. Tạo các thông tin mặc định bắt buộc trong DB cho bảng Users
                    user.GUID = Guid.NewGuid();
                    user.CreatedDate = DateTime.Now;
                    user.IsActive = true;
                    user.IsAdmin = (role == "Administrator");

                    // Tạm thời lấy Email làm Username
                    if (string.IsNullOrEmpty(user.Username)) user.Username = user.Email;
                    if (string.IsNullOrEmpty(user.Password))
                        user.Password = Helper_Security.Hash("123456");
                    else
                        user.Password = Helper_Security.Hash(user.Password); // Password mặc định

                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges(); // Lưu trước để sinh ra user.ID tự động (Identity)

                    // 2. Chèn dữ liệu vào bảng phân quyền tương ứng dựa vào Foreign Key [UserID]
                    if (role == "Host")
                    {
                        Host host = new Host { UserID = user.ID, IsVerified = false, TotalReviews = 0 };
                        db.Hosts.InsertOnSubmit(host);
                    }
                    else if (role == "Guest")
                    {
                        Guest guest = new Guest { UserID = user.ID, LoyaltyPoints = 0 };
                        db.Guests.InsertOnSubmit(guest);
                    }

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateUser(User updatedUser, string role)
        {
            try
            {
                using (var context = new Seoul_StayDataContext())
                {
                    var user = context.Users.FirstOrDefault(x => x.ID == updatedUser.ID);
                    if (user == null) return false;

                    // Cập nhật thông tin cơ bản
                    user.FullName = updatedUser.FullName;
                    user.Email = updatedUser.Email;
                    user.IsAdmin = (role == "Administrator");
                    user.Username = updatedUser.Username;
                    user.Password = Helper_Security.Hash(updatedUser.Password);

                    // Xử lý chuyển đổi vai trò (Nếu đổi từ Guest sang Host hoặc ngược lại)
                    var currentHost = context.Hosts.FirstOrDefault(h => h.UserID == user.ID);
                    var currentGuest = context.Guests.FirstOrDefault(g => g.UserID == user.ID);

                    if (role == "Host" && currentHost == null)
                    {
                        if (currentGuest != null) context.Guests.DeleteOnSubmit(currentGuest);
                        context.Hosts.InsertOnSubmit(new Host { UserID = user.ID, IsVerified = false, TotalReviews = 0 });
                    }
                    else if (role == "Guest" && currentGuest == null)
                    {
                        if (currentHost != null) context.Hosts.DeleteOnSubmit(currentHost);
                        context.Guests.InsertOnSubmit(new Guest { UserID = user.ID, LoyaltyPoints = 0 });
                    }
                    else if (role == "Administrator")
                    {
                        // Nếu chuyển thành Admin, xóa các Profile nghiệp vụ cũ nếu có
                        if (currentHost != null) context.Hosts.DeleteOnSubmit(currentHost);
                        if (currentGuest != null) context.Guests.DeleteOnSubmit(currentGuest);
                    }

                    context.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}