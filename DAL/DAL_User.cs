using DTO;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DAL_User
    {
        // ---------------------------------------------------------------
        // Tất cả DataContext đều dùng DAL_Settings.ConnectionString thay vì
        // default constructor (đọc app.config cũ) để đảm bảo sau khi user
        // lưu cấu hình DB mới trong DatabaseSettingsWindow, mọi câu query
        // đều dùng đúng connection string mới – không cần restart app.
        // ---------------------------------------------------------------

        public User Login(string username, string password)
        {
            using (var db = Ctx())
            {
                var user = db.Users.FirstOrDefault(x =>
                    x.Username == username && x.IsActive);

                if (user == null) return null;

                string hashed = Helper_Security.Hash(password);

                if (user.Password == hashed)
                {
                    // Đã là hash rồi → đăng nhập OK
                    return user;
                }

                else if (user.Password == password)
                {
                    // Password cũ dạng plain text → migrate sang hash
                    user.Password = hashed;
                    db.SubmitChanges();
                    return user;
                }

                // Sai mật khẩu
                return null;
            }
        }
        //public User Login(string username, string password)
        //{
        //    using (var db = Ctx())
        //    {
        //        var user = db.Users.FirstOrDefault(x =>
        //            x.Username == username && x.IsActive);

        //        if (user == null)
        //        {
        //            System.Diagnostics.Debug.WriteLine("User not found");
        //            return null;
        //        }

        //        string hashed = Helper_Security.Hash(password);

        //        System.Diagnostics.Debug.WriteLine(
        //            $"Input=[{password}]\n" +
        //            $"Hash=[{hashed}]\n" +
        //            $"DB=[{user.Password}]");

        //        if (user.Password == hashed)
        //        {
        //            System.Diagnostics.Debug.WriteLine("Hash match");
        //            return user;
        //        }

        //        if (user.Password == password)
        //        {
        //            System.Diagnostics.Debug.WriteLine("Plain text match");

        //            user.Password = hashed;
        //            db.SubmitChanges();

        //            return user;
        //        }

        //        System.Diagnostics.Debug.WriteLine("Password mismatch");
        //        return null;
        //    }
        //}

        public bool CheckUsername(string username)
        {
            using (var db = Ctx())
                return db.Users.Any(x => x.Username == username);
        }

        public bool CheckEmail(string email)
        {
            using (var db = Ctx())
                return db.Users.Any(x => x.Email == email);
        }

        public bool Register(User newUser)
        {
            try
            {
                using (var db = Ctx())
                {
                    newUser.GUID = Guid.NewGuid();
                    newUser.IsAdmin = false;
                    newUser.CreatedDate = DateTime.Now;
                    newUser.IsActive = true;

                    db.Users.InsertOnSubmit(newUser);
                    db.SubmitChanges();

                    var guest = new Guest
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public User GetById(long id)
        {
            using (var db = Ctx())
                return db.Users.FirstOrDefault(u => u.ID == id);
        }

        public List<DTO_UserDisplay> GetAllUsersDisplay()
        {
            using (var db = Ctx())
            {
                var data = (from u in db.Users
                            join h in db.Hosts on u.ID equals h.UserID into hostJoin
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
                                TotalBookings = db.Bookings.Count(b => b.GuestUserID == u.ID)
                            }).ToList();

                return data.Select(u => new DTO_UserDisplay
                {
                    UserID = u.ID,
                    FullName = u.FullName,
                    Email = u.Email,
                    Username = u.Username,
                    Country = u.Country,
                    Status = u.IsActive ? "Active" : "Locked",
                    Role = u.IsAdmin ? "Administrator"
                                    : u.Host != null ? "Host"
                                    : "Guest",
                    LastActive = u.CreatedDate.ToString("MMMM dd, yyyy"),
                    TotalBookings = u.TotalBookings
                }).ToList();
            }
        }

        public bool ToggleLock(long userId)
        {
            try
            {
                using (var db = Ctx())
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteUser(long userId)
        {
            try
            {
                using (var db = Ctx())
                {
                    var user = db.Users.FirstOrDefault(x => x.ID == userId);
                    if (user == null) return false;

                    user.IsActive = false; // Soft delete
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public bool InsertUser(User user, string role)
        {
            try
            {
                using (var db = Ctx())
                {
                    user.GUID = Guid.NewGuid();
                    user.CreatedDate = DateTime.Now;
                    user.IsActive = true;
                    user.IsAdmin = (role == "Administrator");

                    if (string.IsNullOrEmpty(user.Username))
                        user.Username = user.Email;

                    user.Password = string.IsNullOrEmpty(user.Password)
                        ? Helper_Security.Hash("123456")
                        : Helper_Security.Hash(user.Password);

                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges(); // Lưu trước để lấy user.ID (Identity)

                    if (role == "Host")
                    {
                        db.Hosts.InsertOnSubmit(new Host
                        {
                            UserID = user.ID,
                            IsVerified = false,
                            TotalReviews = 0
                        });
                    }
                    else if (role == "Guest")
                    {
                        db.Guests.InsertOnSubmit(new Guest
                        {
                            UserID = user.ID,
                            LoyaltyPoints = 0
                        });
                    }

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateUser(User updatedUser, string role)
        {
            try
            {
                using (var db = Ctx())
                {
                    var user = db.Users.FirstOrDefault(x => x.ID == updatedUser.ID);
                    if (user == null) return false;

                    user.FullName = updatedUser.FullName;
                    user.Email = updatedUser.Email;
                    user.IsAdmin = (role == "Administrator");
                    user.Username = updatedUser.Username;
                    user.Password = Helper_Security.Hash(updatedUser.Password);

                    // Xử lý chuyển đổi vai trò
                    var currentHost = db.Hosts.FirstOrDefault(h => h.UserID == user.ID);
                    var currentGuest = db.Guests.FirstOrDefault(g => g.UserID == user.ID);

                    if (role == "Host" && currentHost == null)
                    {
                        if (currentGuest != null) db.Guests.DeleteOnSubmit(currentGuest);
                        db.Hosts.InsertOnSubmit(new Host { UserID = user.ID, IsVerified = false, TotalReviews = 0 });
                    }
                    else if (role == "Guest" && currentGuest == null)
                    {
                        if (currentHost != null) db.Hosts.DeleteOnSubmit(currentHost);
                        db.Guests.InsertOnSubmit(new Guest { UserID = user.ID, LoyaltyPoints = 0 });
                    }
                    else if (role == "Administrator")
                    {
                        if (currentHost != null) db.Hosts.DeleteOnSubmit(currentHost);
                        if (currentGuest != null) db.Guests.DeleteOnSubmit(currentGuest);
                    }

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // ---------------------------------------------------------------
        // Factory helper: luôn dùng connection string từ DAL_Settings
        // (đọc từ file config được lưu bởi DatabaseSettingsWindow).
        // ---------------------------------------------------------------
        private static Seoul_StayDataContext Ctx()
            => new Seoul_StayDataContext(DAL_Settings.ConnectionString);
    }
}