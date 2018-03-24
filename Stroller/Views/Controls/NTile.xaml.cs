using System.Windows;
using System.Windows.Controls;
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

        public PackIconModernKind Icon
        {
            get => (PackIconModernKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public int IconSize
        {
            get => (int) GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public NTile()
        {
            InitializeComponent();
        }
    }
}
