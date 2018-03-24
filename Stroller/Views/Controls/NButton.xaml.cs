using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace Stroller.Views.Controls
{
    /// <summary>
    /// Interaction logic for NButton.xaml
    /// </summary>
    public partial class NButton : Button
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(NButton));

        public static DependencyProperty IsAccentedProperty =
            DependencyProperty.Register("IsAccented", typeof(bool), typeof(NButton));

        public static DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(PackIconModernKind), typeof(NButton));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsAccented
        {
            get => (bool) GetValue(IsAccentedProperty);
            set => SetValue(IsAccentedProperty, value);
        }

        public PackIconModernKind Icon
        {
            get => (PackIconModernKind) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }


        public NButton()
        {
            InitializeComponent();

            Loaded += NButton_Loaded;
        }

        private void NButton_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
            IconControl.Visibility = Icon == PackIconModernKind.None ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LoadStyle()
        {
            CurrentButton.Style = Application.Current.Resources["SquareButtonStyle"] as Style;
            if (IsAccented)
            {
                CurrentButton.Style = Application.Current.Resources["AccentedSquareButtonStyle"] as Style;
            }
        }
    }
}
