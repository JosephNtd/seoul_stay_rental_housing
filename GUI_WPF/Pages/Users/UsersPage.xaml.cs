using BUS;
using DTO;
using ET;
using GUI_WPF.UserControls.Cards;
using GUI_WPF.UserControls.Panels;
using GUI_WPF.UserControls.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GUI_WPF.UserControls.Drawers;
using GUI_WPF.Pages.Bookings;
using GUI_WPF.Pages.Transactions;

namespace GUI_WPF.Pages.Users
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {

        // BUS
        private readonly BUS_UserWorkspace _busWorkspace = new BUS_UserWorkspace();

        // LEFT DIRECTORY
        private List<DTO_UserDisplay> _users = new List<DTO_UserDisplay>();

        // CURRENT WORKSPACE
        private DTO_UserWorkspace _selectedUser;
        private ET_HostBankAccount _editingBank;

        private bool _isCreatingBank = false;

        // PAGINATION
        private int _currentPage = 1;

        private readonly int _pageSize = 4;

        private int _totalPages = 1;

        // FILTERS
        private string _searchText = "";

        private string _roleFilter = "All Roles";

        private string _statusFilter = "All Status";

        // CTOR
        public UsersPage()
        {
            InitializeComponent();
            InitializeDrawers();
            Loaded += UsersPage_Loaded;
        }
        private void InitializeDrawers()
        {
            // USER DRAWER
            userEditorDrawer.SaveClicked += UserEditorDrawer_SaveClicked;
            userEditorDrawer.CancelClicked += Drawer_CloseRequested;
            userEditorDrawer.CloseClicked += Drawer_CloseRequested;

            // BANK DRAWER
            bankEditorDrawer.SaveClicked += BankEditorDrawer_SaveClicked;
            bankEditorDrawer.CancelClicked += Drawer_CloseRequested;
            bankEditorDrawer.CloseClicked += Drawer_CloseRequested;
        }
        private void cbBookingFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedUser == null)
                return;

            IEnumerable<DTO_UserBookingSummary> query = _selectedUser.Bookings;

            if (cbBookingFilter.SelectedItem is ComboBoxItem item)
            {
                string value = item.Content.ToString();

                switch (value)
                {
                    case "Upcoming":
                        query = query.Where(x => x.BookingStatus == "Upcoming");
                        break;

                    case "Completed":
                        query = query.Where(x => x.BookingStatus == "Completed");
                        break;

                    case "Cancelled":
                        query = query.Where(x => x.BookingStatus == "Cancelled");
                        break;
                }
            }

            spBookings.Children.Clear();

            foreach (var booking in query)
            {
                UserBookingCard card = new UserBookingCard();

                card.PropertyTitle = booking.ItemTitle;
                card.Area = booking.AreaName;
                card.GuestName = booking.GuestName;
                card.Status = booking.BookingStatus;
                card.StayDates = booking.StayDateDisplay;
                card.Price = booking.FinalPriceDisplay;
                card.GuestCount = booking.GuestCount.ToString();
                card.Nights = booking.Nights.ToString();
                card.BookingID = $"#{booking.BookingID}";
                card.ViewClicked += Booking_ViewClicked;
                card.ActionClicked += Booking_ActionClicked;
                spBookings.Children.Add(card);
            }
        }
        private void BankEditorDrawer_SaveClicked(object sender, RoutedEventArgs e)
        {
            ET_HostBankAccount bank = bankEditorDrawer.BuildEntity();

            if (_isCreatingBank)
            {
                bank.ID = DateTime.Now.Ticks;
                bank.CreatedDate = DateTime.Now;
                _selectedUser.BankAccounts.Add(bank);
            }
            else
            {
                if (_editingBank != null)
                {
                    _editingBank.BankName = bank.BankName;
                    _editingBank.AccountHolder = bank.AccountHolder;
                    _editingBank.AccountNumber = bank.AccountNumber;
                    _editingBank.IsPrimary = bank.IsPrimary;
                    _editingBank.IsVerified = bank.IsVerified;
                    _editingBank.IsActive = bank.IsActive;
                }
            }

            LoadBankAccounts();

            CloseOverlay();
        }
        private void Bank_EditClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is BankAccountCard card))
                return;

            ET_HostBankAccount bank = _selectedUser.BankAccounts.FirstOrDefault(x => x.AccountNumber == card.AccountNumber);

            if (bank == null)
                return;

            _editingBank = bank;
            _isCreatingBank = false;

            OpenOverlay();

            bankEditorDrawer.Visibility = Visibility.Visible;
            bankEditorDrawer.IsCreateMode = false;
            bankEditorDrawer.LoadBank(bank);
        }
        private void Bank_RemoveClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is BankAccountCard card))
                return;

            MessageBoxResult result = MessageBox.Show("Remove this bank account?", "Confirm", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            ET_HostBankAccount bank = _selectedUser.BankAccounts.FirstOrDefault(x => x.AccountNumber == card.AccountNumber);

            if (bank != null)
            {
                _selectedUser.BankAccounts.Remove(bank);

                LoadBankAccounts();
            }
        }
        private void btnAddBank_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
                return;

            _editingBank = null;
            _isCreatingBank = true;

            OpenOverlay();

            bankEditorDrawer.Visibility = Visibility.Visible;
            bankEditorDrawer.IsCreateMode = true;
            bankEditorDrawer.ClearForm();
        }
        private void Booking_ActionClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is UserBookingCard card))
                return;
            MessageBox.Show($"Manage booking {card.BookingID}");
        }
        private void Booking_ViewClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is UserBookingCard card))
                return;

            DTO_UserBookingSummary booking = _selectedUser.Bookings.FirstOrDefault(x => $"#{x.BookingID}" == card.BookingID);

            if (booking == null)
                return;

            BookingDetailWindow window = new BookingDetailWindow(booking);

            window.ShowDialog();
        }
        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
                return;

            MessageBoxResult result = MessageBox.Show("Delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            MessageBox.Show("User deleted.");

            LoadUsers();
        }
        private void btnLockUser_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
                return;

            string next = _selectedUser.User.IsActive ? "lock" : "unlock";

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to {next} this user?", "Confirm", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            _selectedUser.User.IsActive = !_selectedUser.User.IsActive;

            MessageBox.Show($"User {next}ed.");

            LoadWorkspace(_selectedUser);
        }
        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
                return;

            MessageBox.Show($"Password reset link sent to:\n{_selectedUser.User.Email}", "Reset Password");
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            OpenOverlay();

            userEditorDrawer.Visibility = Visibility.Visible;
            userEditorDrawer.IsCreateMode = true;
            userEditorDrawer.ClearForm();
        }
        private void UserEditorDrawer_SaveClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("User updated successfully.");
            CloseOverlay();
            LoadUsers();
        }
        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
                return;

            OpenOverlay();

            userEditorDrawer.Visibility = Visibility.Visible;
            userEditorDrawer.IsCreateMode = false;
            userEditorDrawer.LoadUser(_selectedUser);
        }
        private void OpenOverlay()
        {
            OverlayLayer.Visibility = Visibility.Visible;
        }

        private void CloseOverlay()
        {
            OverlayLayer.Visibility = Visibility.Collapsed;
            userEditorDrawer.Visibility = Visibility.Collapsed;
            bankEditorDrawer.Visibility = Visibility.Collapsed;
        }

        private void Drawer_CloseRequested(object sender, RoutedEventArgs e)
        {
            CloseOverlay();
        }

        // PAGE LOAD
        private void UsersPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }


        // LOAD USERS DIRECTORY
        private void LoadUsers()
        {
            try
            {
                _users = _busWorkspace.GetUsersDirectory();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Users Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // APPLY FILTERS


        private void ApplyFilters()
        {
            IEnumerable<DTO_UserDisplay> query = _users;

            // SEARCH
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                query = query.Where(x =>
                       x.FullName.ToLower().Contains(_searchText.ToLower())
                    || x.Email.ToLower().Contains(_searchText.ToLower())
                    || x.Username.ToLower().Contains(_searchText.ToLower()));
            }

            // ROLE
            switch (_roleFilter)
            {
                case "Guests":

                    query = query.Where(x => x.Role == "Guest");
                    break;

                case "Hosts":
                    query = query.Where(x => x.Role == "Host");
                    break;

                case "Admins":
                    query = query.Where(x => x.Role == "Administrator");
                    break;
            }

            // STATUS
            switch (_statusFilter)
            {
                case "Active":
                    query = query.Where(x => x.Status == "Active");
                    break;

                case "Locked":
                    query = query.Where(x => x.Status == "Locked");
                    break;
            }

            List<DTO_UserDisplay> filtered = query.ToList();

            // COUNT
            txtDirectoryCount.Text = $"{filtered.Count} users";

            // PAGINATION
            _totalPages = (int)Math.Ceiling((double)filtered.Count / _pageSize);

            if (_totalPages <= 0)
                _totalPages = 1;

            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            txtPagination.Text = $"Page {_currentPage} / {_totalPages}";

            List<DTO_UserDisplay> pageData = filtered.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();

            RenderUserDirectory(pageData);

            // AUTO SELECT FIRST
            if (_selectedUser == null && filtered.Count > 0)
            {
                _selectedUser = _busWorkspace.GetWorkspace(filtered.First().UserID);

                LoadWorkspace(_selectedUser);
            }
        }

        // RENDER USER DIRECTORY
        private void RenderUserDirectory(List<DTO_UserDisplay> users)
        {
            spUsers.Children.Clear();

            foreach (DTO_UserDisplay user in users)
            {
                UserDirectoryCard card = new UserDirectoryCard();

                card.FullName = user.FullName;
                card.Email = user.Email;
                card.Country = user.Country;

                card.BookingText = $"{user.TotalBookings} bookings";
                card.Status = user.Status;
                card.Role = user.Role;

                // SELECTED
                if (_selectedUser != null && _selectedUser.User.ID == user.UserID)
                {
                    card.IsSelected = true;
                }

                // CLICK
                card.Selected += (s, e) =>
                {
                    _selectedUser = _busWorkspace.GetWorkspace(user.UserID);

                    LoadWorkspace(_selectedUser);

                    ApplyFilters();
                };

                spUsers.Children.Add(card);
            }
        }

        // LOAD WORKSPACE
        private void LoadWorkspace(DTO_UserWorkspace workspace)
        {
            if (workspace == null)
                return;

            _selectedUser = workspace;
            LoadHeroProfile();
            LoadStats();
            LoadBookings();
            LoadTransactions();
            LoadBankAccounts();
            LoadInsights();
        }

        // HERO PROFILE
        private void LoadHeroProfile()
        {
            if (_selectedUser == null)
                return;

            heroProfile.FullName = _selectedUser.User.FullName;
            heroProfile.Username = "@" + _selectedUser.User.Username;
            heroProfile.Email = _selectedUser.User.Email;
            heroProfile.Phone = _selectedUser.User.PhoneNumber;
            heroProfile.Country = _selectedUser.User.Country;
            heroProfile.JoinedDate = _selectedUser.User.CreatedDate.ToString("dd MMM yyyy");
            heroProfile.Status = _selectedUser.StatusDisplay;
            heroProfile.Role = _selectedUser.RoleDisplay;
            heroProfile.Verification = _selectedUser.VerificationDisplay;

            // HOST
            if (_selectedUser.HostProfile != null)
            {
                heroProfile.HostInfo = $"{_selectedUser.RatingDisplay} • " + $"{_selectedUser.HostProfile.TotalReviews} reviews";

                heroProfile.BusinessInfo = $"Tax Code: {_selectedUser.HostProfile.TaxCode}";
            }

            // GUEST
            if (_selectedUser.GuestProfile != null)
            {
                heroProfile.GuestInfo = $"Loyalty: {_selectedUser.LoyaltyDisplay}";

                heroProfile.LanguageInfo = $"Preferred Language: {_selectedUser.GuestProfile.PreferredLanguage}";
            }
        }

        // LOAD STATS
        private void LoadStats()
        {
            spStatCards.Children.Clear();

            // BOOKINGS
            UserStatCard bookings = new UserStatCard();

            bookings.Title = "Bookings";
            bookings.Value = _selectedUser.TotalBookings.ToString();
            bookings.Subtitle = "Total reservations";
            bookings.Trend = "+12.4%";
            bookings.Period = "vs last month";
            bookings.IsPositiveTrend = true;
            bookings.SetBookingTheme();
            spStatCards.Children.Add(bookings);

            // SPENT
            UserStatCard spent = new UserStatCard();

            spent.Title = "Total Spent";
            spent.Value = $"${_selectedUser.TotalSpent:N0}";
            spent.Subtitle = "Across completed bookings";
            spent.Trend = "+8.2%";
            spent.Period = "vs last month";

            spent.IsPositiveTrend = true;
            spent.SetRevenueTheme();
            spStatCards.Children.Add(spent);

            // REVENUE
            UserStatCard revenue = new UserStatCard();
            revenue.Title = "Revenue";
            revenue.Value = $"${_selectedUser.TotalRevenue:N0}";
            revenue.Subtitle = "Host earnings";
            revenue.Trend = "+15.7%";
            revenue.Period = "vs last month";

            revenue.IsPositiveTrend = true;
            revenue.SetTransactionTheme();
            spStatCards.Children.Add(revenue);

            // TRANSACTIONS
            UserStatCard trx = new UserStatCard();

            trx.Title = "Transactions";
            trx.Value = _selectedUser.TotalTransactions.ToString();
            trx.Subtitle = "Payment activity";
            trx.Trend = "-2.1%";
            trx.Period = "vs last month";
            trx.IsPositiveTrend = false;

            trx.SetUserTheme();

            spStatCards.Children.Add(trx);
        }

        // LOAD BOOKINGS
        private void LoadBookings()
        {
            spBookings.Children.Clear();

            foreach (DTO_UserBookingSummary booking in _selectedUser.Bookings)
            {
                UserBookingCard card = new UserBookingCard();

                card.PropertyTitle = booking.ItemTitle;
                card.Area = booking.AreaName;
                card.GuestName = booking.GuestName;
                card.Status = booking.BookingStatus;
                card.StayDates = booking.StayDateDisplay;
                card.Price = booking.FinalPriceDisplay;
                card.GuestCount = booking.GuestCount.ToString();
                card.Nights = booking.Nights.ToString();
                card.BookingID = $"#{booking.BookingID}";
                card.ActionText = "Manage";

                card.ViewClicked += Booking_ViewClicked;
                card.ActionClicked += Booking_ActionClicked;

                spBookings.Children.Add(card);
            }
        }

        // LOAD TRANSACTIONS
        private void LoadTransactions()
        {

            spTransactions.Children.Clear();

            foreach (ET_Transactions trx in _selectedUser.Transactions)
            {
                TransactionTimelineItem item = new TransactionTimelineItem(); item.MouseLeftButtonUp += (s, e) =>
                {
                    TransactionDetailWindow window = new TransactionDetailWindow(trx);

                    window.ShowDialog();
                };
                item.Title = trx.TransactionTypeName;
                item.Subtitle = trx.Description;
                item.Date = trx.TransactionDate.ToString("dd MMM yyyy");
                item.TransactionType = trx.TransactionTypeName;
                item.Amount = $"{trx.Amount:N0}$";
                item.Status = "Completed";
                item.IsPositive = trx.Amount >= 0;

                item.SetPaymentTheme();
                spTransactions.Children.Add(item);
            }
        }

        // LOAD BANK ACCOUNTS
        private void LoadBankAccounts()
        {
            wpBankAccounts.Children.Clear();

            foreach (ET_HostBankAccount bank in _selectedUser.BankAccounts)
            {
                BankAccountCard card = new BankAccountCard();

                card.BankName = bank.BankName;
                card.AccountHolder = bank.AccountHolder;
                card.AccountNumber = bank.AccountNumber;
                card.CreatedDate = bank.CreatedDate.ToString("dd MMM yyyy");
                card.IsPrimary = bank.IsPrimary;
                card.IsVerified = bank.IsVerified;
                card.IsActive = bank.IsActive;
                card.BankIcon = "🏦";

                card.EditClicked += Bank_EditClicked;
                card.RemoveClicked += Bank_RemoveClicked;
                wpBankAccounts.Children.Add(card);
            }
        }

        // LOAD INSIGHTS
        private void LoadInsights()
        {
            insightsPanel.TrustScore = "92";
            insightsPanel.TrustLabel = "Excellent";
            insightsPanel.AverageRating = _selectedUser.RatingDisplay;
            insightsPanel.Loyalty = _selectedUser.LoyaltyDisplay;
            insightsPanel.TotalNights = (_selectedUser.TotalBookings * 3).ToString();
            insightsPanel.CancellationRate = "2.1%";
            insightsPanel.BehaviorSummary = "This user consistently completes stays and maintains strong financial activity across the platform.";
            insightsPanel.Tag1 = "Trusted";
            insightsPanel.Tag2 = "High Value";
            insightsPanel.Tag3 = "Frequent Traveler";
            insightsPanel.RiskLevel = "Low Risk";
        }

        // SEARCH
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = txtSearch.Text.Trim();
            _currentPage = 1;

            ApplyFilters();
        }

        // ROLE FILTER
        private void cbRoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRoleFilter.SelectedItem is ComboBoxItem item)
            {
                _roleFilter = item.Content.ToString();
                _currentPage = 1;
                ApplyFilters();
            }
        }

        // STATUS FILTER
        private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbStatusFilter.SelectedItem is ComboBoxItem item)
            {
                _statusFilter = item.Content.ToString();

                _currentPage = 1;

                ApplyFilters();
            }
        }

        // PAGINATION         
        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;

                ApplyFilters();
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;

                ApplyFilters();
            }
        }

        // REFRESH
        private void btnRefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

    }
}