using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Cards
{
	public partial class PriceEditorPopup : UserControl
	{
		public PriceEditorPopup()
		{
			InitializeComponent();

			cbPolicies.SelectionChanged += cbPolicies_SelectionChanged;
		}
		// DATA
		public ET_ItemPrices CurrentPrice { get; private set; }
		public List<ET_CancellationPolicies> Policies { get; private set; } = new List<ET_CancellationPolicies>();
		public List<ET_CancellationRefundFees> RefundFees { get; private set; } = new List<ET_CancellationRefundFees>();


		// EVENTS
		public event EventHandler SaveClicked;
		public event EventHandler CancelClicked;
		public event EventHandler DeleteClicked;
		public event EventHandler CloseClicked;


		// LOAD


		public void LoadData(ET_ItemPrices price, List<ET_CancellationPolicies> policies, List<ET_CancellationRefundFees> fees)
		{
			CurrentPrice = price;

			Policies = policies ?? new List<ET_CancellationPolicies>();

			RefundFees = fees ?? new List<ET_CancellationRefundFees>();


			// DATE
			if (price != null)
			{
				txtSelectedDate.Text = price.Date.ToString("dddd, MMMM dd yyyy");

				txtPrice.Text = price.Price.ToString("0");
			}
			// POLICIES
			cbPolicies.ItemsSource = Policies;

			if (price != null)
			{
				cbPolicies.SelectedValue = price.CancellationPolicyID;
			}

			if (cbPolicies.SelectedIndex < 0 && cbPolicies.Items.Count > 0)
			{
				cbPolicies.SelectedIndex = 0;
			}

			UpdatePolicyDetails();
		}


		// POLICY DETAILS
		private void UpdatePolicyDetails()
		{
			if (cbPolicies.SelectedItem is ET_CancellationPolicies policy)
			{
				txtCommission.Text = $"{policy.PlatformCommissionRate:0}%";

				var rules =
					RefundFees
					.Where(x => x.CancellationPolicyID == policy.ID)
					.OrderByDescending(x => x.DaysLeft)
					.Select(x => new
					{
						Rule = $"{x.DaysLeft}+ days before check-in",

						Penalty = $"{x.PenaltyPercentage:0}% fee"
					}).ToList();

				icRefundRules.ItemsSource = rules;
			}
		}

		// POLICY CHANGE        
		private void cbPolicies_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdatePolicyDetails();
		}

		// PRESETS       
		private void btnPresetLow_Click(object sender, RoutedEventArgs e)
		{
			txtPrice.Text = "80";
		}

		private void btnPresetMedium_Click(object sender, RoutedEventArgs e)
		{
			txtPrice.Text = "120";
		}

		private void btnPresetHigh_Click(
			object sender, RoutedEventArgs e)
		{
			txtPrice.Text = "200";
		}

		// ACTIONS       
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentPrice == null)
			{
				CurrentPrice = new ET_ItemPrices();
			}

			decimal price;

			if (!decimal.TryParse(txtPrice.Text, out price))
			{
				MessageBox.Show("Invalid price value.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

				return;
			}

			CurrentPrice.Price = price;

			if (cbPolicies.SelectedItem is ET_CancellationPolicies policy)
			{
				CurrentPrice.CancellationPolicyID = policy.ID;
			}

			SaveClicked?.Invoke(this, EventArgs.Empty);
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			CancelClicked?.Invoke(this, EventArgs.Empty);
		}

		private void btnRemovePrice_Click(object sender, RoutedEventArgs e)
		{
			DeleteClicked?.Invoke(this, EventArgs.Empty);
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			CloseClicked?.Invoke(this, EventArgs.Empty);
		}
	}
}