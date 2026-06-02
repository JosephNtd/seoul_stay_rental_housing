using ET;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GUI_WPF.UserControls.Cards
{
    public partial class PictureTile : UserControl
    {
        public PictureTile()
        {
            InitializeComponent();

            Cursor = Cursors.Hand;
        }
        // DATA
        public ET_ItemPictures Picture { get; private set; }
        // PROPERTIES
        public bool IsCover { get; set; }


        // EVENTS
        public event EventHandler DeleteClicked;
        public event EventHandler SetCoverClicked;

        // LOAD
        public void LoadPicture(ET_ItemPictures picture, bool isCover = false)
        {
            if (picture == null)
                return;
            Picture = picture;
            IsCover = isCover;

            // FILE NAME
            txtFileName.Text = string.IsNullOrWhiteSpace(picture.FileName) ? "picture" : picture.FileName;

            // DISPLAY ORDER
            txtOrder.Text = (picture.DisplayOrder + 1).ToString();

            // STATUS
            txtPictureStatus.Text = picture.IsNew ? "New picture" : "Saved picture";

            // COVER BADGE
            CoverBadge.Visibility = IsCover ? Visibility.Visible : Visibility.Collapsed;

            // IMAGE
            try
            {
                string imagePath = picture.FullPath;

                if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    imgPicture.Source = bitmap;
                }
                else
                {
                    // Debug
                    System.Diagnostics.Debug.WriteLine($"Image not found: {imagePath}");
                    imgPicture.Source = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load image error: {ex.Message}");
                imgPicture.Source = null;
            }
        }


        // DELETE
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(this, EventArgs.Empty);
        }
        // COVER         
        private void btnSetCover_Click(object sender, RoutedEventArgs e)
        {
            SetCoverClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}