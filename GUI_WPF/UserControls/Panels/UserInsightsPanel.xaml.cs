using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Panels
{
    public partial class UserInsightsPanel : UserControl
    {
        public UserInsightsPanel()
        {
            InitializeComponent();
        }

        // TRUST SCORE
        public string TrustScore
        {
            get => txtTrustScore.Text;
            set => txtTrustScore.Text = value;
        }
        public string TrustLabel
        {
            get => txtTrustLabel.Text;
            set => txtTrustLabel.Text = value;
        }

        // KPI
        public string AverageRating
        {
            get => txtRating.Text;
            set => txtRating.Text = value;
        }

        public string Loyalty
        {
            get => txtLoyalty.Text;
            set => txtLoyalty.Text = value;
        }

        public string TotalNights
        {
            get => txtNights.Text;
            set => txtNights.Text = value;
        }

        public string CancellationRate
        {
            get => txtCancellation.Text;
            set => txtCancellation.Text = value;
        }

        // BEHAVIOR
        public string BehaviorSummary
        {
            get => txtBehavior.Text;
            set => txtBehavior.Text = value;
        }


        // TAGS
        public string Tag1
        {
            get => txtTag1.Text;
            set => txtTag1.Text = value;
        }

        public string Tag2
        {
            get => txtTag2.Text;
            set => txtTag2.Text = value;
        }

        public string Tag3
        {
            get => txtTag3.Text;
            set => txtTag3.Text = value;
        }
        // RISK
        public string RiskLevel
        {
            get => txtRisk.Text;
            set
            {
                txtRisk.Text = value;

                switch (value)
                {
                    case "Low Risk":

                        txtRisk.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#47A87B"));

                        txtRiskIcon.Text = "✔";

                        txtRiskIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#47A87B"));

                        break;

                    case "Medium Risk":

                        txtRisk.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D29B22"));

                        txtRiskIcon.Text = "!";

                        txtRiskIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D29B22"));

                        break;

                    case "High Risk":

                        txtRisk.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D96A6A"));

                        txtRiskIcon.Text = "⚠";

                        txtRiskIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D96A6A"));

                        break;
                }
            }
        }
    }
}
