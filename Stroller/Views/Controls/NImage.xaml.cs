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
        private bool _isSelected;

        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NImage));

        public static DependencyProperty BackgroundImageDataProperty =
            DependencyProperty.Register("BackgroundImageData", typeof(byte[]), typeof(NImage));

        public static DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(NImage));

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

        public bool IsSelectable
        {
            get => (bool) GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        public delegate void OnDeleteClicked(ImageListItem data);

        public delegate void OnDownloadZipClicked(ImageListItem data);

        public delegate void OnDownloadJsonClicked(ImageListItem data);

        public delegate void OnItemClicked(ImageListItem data);

        public delegate void OnItemSelectionChanged(ImageListItem data, bool state);

        public event OnDeleteClicked DeleteClicked;

        public event OnDownloadZipClicked DownloadZipClicked;

        public event OnDownloadJsonClicked DownloadJsonClicked;

        public event OnItemClicked ItemClicked;

        public event OnItemSelectionChanged ItemSelectionChanged;


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

            if (IsSelectable)
            {
                OperationsPanel.Visibility = Visibility.Collapsed;
            }
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
            if (!IsSelectable)
                ItemClicked?.Invoke(DataContext as ImageListItem);
            else
            {
                ToggleSelection();
                ItemSelectionChanged?.Invoke(DataContext as ImageListItem, _isSelected);
            }
        }

        private void ToggleSelection()
        {
            _isSelected = !_isSelected;
            CurrentImageItem.BorderThickness = _isSelected ? new Thickness(4) : new Thickness(0);
        }
    }
}
