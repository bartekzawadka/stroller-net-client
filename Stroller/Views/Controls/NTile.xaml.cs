using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace Stroller.Views.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy NTile.xaml
    /// </summary>
    public partial class NTile : Button
    {
        public static DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(PackIconModernKind), typeof(NTile));

        public static DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(int), typeof(NTile));

        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NTile));

        public static DependencyProperty IsDangerProperty =
            DependencyProperty.Register("IsDanger", typeof(bool), typeof(NTile));

        public PackIconModernKind Icon
        {
            get => (PackIconModernKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsDanger
        {
            get => (bool)GetValue(IsDangerProperty);
            set => SetValue(TextProperty, value);
        }

        public NTile()
        {
            InitializeComponent();
            Loaded += NTile_Loaded;
            CurrentTile.IsEnabledChanged += CurrentTile_IsEnabledChanged;
        }

        private void CurrentTile_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (IsDanger)
                {
                    TileBorder.BorderBrush = Brushes.Red;
                    TileText.Foreground = Brushes.Red;
                    IconControl.Foreground = Brushes.Red;
                }
                else
                {
                    TileBorder.BorderBrush = Application.Current.Resources["AccentColorBrush"] as Brush;
                    TileText.Foreground = Application.Current.Resources["AccentColorBrush"] as Brush;
                    IconControl.Foreground = Application.Current.Resources["AccentColorBrush"] as Brush;
                }
            }
            else
            {
                TileBorder.BorderBrush = Application.Current.Resources["GrayBrush2"] as Brush;
                TileText.Foreground = Application.Current.Resources["GrayBrush2"] as Brush;
                IconControl.Foreground = Application.Current.Resources["GrayBrush2"] as Brush;
            }
        }

        private void NTile_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsDanger) return;

            TileBorder.BorderBrush = Brushes.Red;
            TileText.Foreground = Brushes.Red;
            IconControl.Foreground = Brushes.Red;
        }
    }
}
