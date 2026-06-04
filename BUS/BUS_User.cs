using DAL;
using DTO;
using ET;
using Helper;
using System;
using System.Collections.Generic;

namespace BUS
{
    public class BUS_User
    {
        private readonly DAL_User _dal = new DAL_User();

        /// <summary>
        /// Hash password trước khi gửi xuống DAL để so sánh đúng với dữ liệu trong DB.
        /// DB lưu SHA256, nên phải hash cùng thuật toán mới match được.
        /// </summary>
        public ET_Users Login(string username, string password)
        {
            //string hashedPassword = Helper_Security.Hash(password);

            var u = _dal.Login(username.Trim(), password);

            if (u == null) return null;

            return new ET_Users
            {
                ID = u.ID,
                GUID = u.GUID,
                Username = u.Username,
                Password = u.Password,
                FullName = u.FullName,
                Email = u.Email,
                Gender = u.Gender,
                BirthDate = u.BirthDate,
                IsAdmin = u.IsAdmin,
                IsActive = u.IsActive
            };
        }

        public bool Register(string username, string password, string fullName,
                             byte gender, DateTime birthDate)
        {
            if (_dal.CheckUsername(username))
                return false;

            var user = new User
            {
                Username = username.Trim(),
                Password = Helper_Security.Hash(password),
                FullName = fullName.Trim(),
                Email = $"{username.Trim()}@seoulstay.local",
                Gender = gender,
                BirthDate = birthDate,
                IsAdmin = false,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            return _dal.Register(user);
        }

        public bool CheckUsername(string username) => _dal.CheckUsername(username);

        public User GetByID(long id) => _dal.GetById(id);

        public List<DTO_UserDisplay> GetAllUsersDisplay() => _dal.GetAllUsersDisplay();

        public bool ToggleLock(long userId) => _dal.ToggleLock(userId);

        public bool DeleteUser(long userId) => _dal.DeleteUser(userId);

        public bool InsertUser(DTO_User dtoUser, string role)
        {
            User dalUser = new User
            {
                FullName = dtoUser.FullName,
                Email = dtoUser.Email,
                Username = dtoUser.Username,
                Password = dtoUser.Password
            };
            return _dal.InsertUser(dalUser, role);
        }

        public bool UpdateUser(DTO_User dtoUser, string role)
        {
            User dalUser = new User
            {
                ID = dtoUser.ID,
                FullName = dtoUser.FullName,
                Email = dtoUser.Email,
                Username = dtoUser.Username,
                Password = dtoUser.Password
            };
            return _dal.UpdateUser(dalUser, role);
        }
    }
}