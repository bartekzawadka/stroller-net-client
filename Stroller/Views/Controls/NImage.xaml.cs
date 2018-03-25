using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Stroller.Contracts.Dto;

namespace Stroller.Views.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy NImage.xaml
    /// </summary>
    public partial class NImage
    {
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NImage));

        public static DependencyProperty BackgroundImageDataProperty =
            DependencyProperty.Register("BackgroundImageData", typeof(byte[]), typeof(NImage));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public byte[] BackgroundImageData
        {
            get => (byte[]) GetValue(BackgroundImageDataProperty);
            set => SetValue(BackgroundImageDataProperty, value);
        }

        public delegate void OnDeleteClicked(ImageListItem data);

        public delegate void OnDownloadZipClicked(ImageListItem data);

        public delegate void OnDownloadJsonClicked(ImageListItem data);

        public delegate void OnItemSelected(ImageListItem data);

        public event OnDeleteClicked DeleteClicked;

        public event OnDownloadZipClicked DownloadZipClicked;

        public event OnDownloadJsonClicked DownloadJsonClicked;

        public event OnItemSelected ItemSelected;

        public NImage()
        {
            InitializeComponent();
            Loaded += NImage_Loaded;
        }

        private void NImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (BackgroundImageData == null)
                return;

            var image = new BitmapImage();
            using (var ms = new MemoryStream(BackgroundImageData))
            {
                ms.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }
            image.Freeze();
            Height = image.Height * 1.0 / (image.Width * 1.0) * ActualWidth;
            

            BackgroundImageBrush.ImageSource = image;
        }

        private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(DataContext as ImageListItem);
        }

        private void BtnDownloadJson_OnClick(object sender, RoutedEventArgs e)
        {
            DownloadJsonClicked?.Invoke(DataContext as ImageListItem);
        }

        private void BtnDownloadZip_OnClick(object sender, RoutedEventArgs e)
        {
            DownloadZipClicked?.Invoke(DataContext as ImageListItem);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ItemSelected?.Invoke(DataContext as ImageListItem);
        }
    }
}
