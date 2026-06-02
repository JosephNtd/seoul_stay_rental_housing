using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
	public partial class CalendarDayCell : UserControl
	{
		public CalendarDayCell()
		{
			InitializeComponent();
			Cursor = Cursors.Hand;
		}

		// PUBLIC PROPERTIES
		public DateTime Date { get; private set; }
		public bool IsAvailable { get; private set; }
		public decimal? Price { get; private set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				SetSelected(value);
			}
		}
		private bool _isSelected;

		// EVENTS
		public event EventHandler DayClicked;

		// LOAD
		public void LoadDate(DateTime date, bool isAvailable, decimal? price)
		{
			Date = date;
			IsAvailable = isAvailable;
			Price = price;

			// DAY
			txtDayName.Text = date.ToString("ddd").ToUpper();
			txtDayNumber.Text = date.Day.ToString();
			txtMonth.Text = date.ToString("MMM").ToUpper();

			// HOLIDAY / WEEKEND
			if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
			{
				HolidayBadge.Visibility = Visibility.Visible;
			}
			else
			{
				HolidayBadge.Visibility = Visibility.Collapsed;
			}

			// AVAILABILITY
			if (isAvailable)
			{
				StatusBadge.Background = new SolidColorBrush(Color.FromRgb(234, 248, 238));
				StatusDot.Fill = new SolidColorBrush(Color.FromRgb(46, 173, 103));
				txtAvailability.Text = "Available";
				txtAvailability.Foreground = new SolidColorBrush(Color.FromRgb(35, 133, 83));
				RootBorder.Background = Brushes.White;
			}
			else
			{
				StatusBadge.Background = new SolidColorBrush(Color.FromRgb(255, 236, 236));
				StatusDot.Fill = new SolidColorBrush(Color.FromRgb(220, 38, 38));
				txtAvailability.Text = "Blocked";
				txtAvailability.Foreground = new SolidColorBrush(Color.FromRgb(185, 28, 28));
				RootBorder.Background = new SolidColorBrush(Color.FromRgb(255, 250, 250));
			}

			// CUSTOM PRICE
			if (price.HasValue)
			{
				txtAvailability.Text = $"${price.Value:0}";
			}
		}

		// SELECT
		public void SetSelected(bool selected)
		{
			if (selected)
			{
				SelectedBorder.Visibility = Visibility.Visible;
			}
			else
			{
				SelectedBorder.Visibility = Visibility.Collapsed;
			}
		}

		// CLICK
		private void RootBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			DayClicked?.Invoke(this, EventArgs.Empty);
		}

		// HOVER
		private void RootBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(160, 120, 118));
		}

		private void RootBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(238, 228, 222));
		}
	}
}