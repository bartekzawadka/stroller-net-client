using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Stroller.Views.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy NImagePreview.xaml
    /// </summary>
    public partial class NImagePreview
    {
        public static DependencyProperty ChunksSourceProperty =
            DependencyProperty.Register("ChunksSource", typeof(List<byte[]>), typeof(NImagePreview));

        private bool _isPressed;
        private double _lastX;
        private List<ImageSource> _chunks = new List<ImageSource>();

        private int _currentIndex;

        public List<byte[]> ChunksSource
        {
            get => (List<byte[]>)GetValue(ChunksSourceProperty);
            set
            {
                SetValue(ChunksSourceProperty, value);
                LoadImageSources();
            }
        }

        private int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                CurrentImage.Source = _chunks[_currentIndex];
            }
        }

        public NImagePreview()
        {
            InitializeComponent();
            Loaded += NImagePreview_Loaded;
        }

        private void NImagePreview_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImageSources();
        }

        private void CurrentImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                _isPressed = true;
        }

        private void CurrentImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Reset();
        }

        private void CurrentImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isPressed || ChunksSource == null || ChunksSource.Count == 0)
                return;

            var x = e.GetPosition(CurrentImage).X;
            if (x < _lastX)
            {
                if (CurrentIndex == 0)
                {
                    CurrentIndex = ChunksSource.Count - 1;
                }
                else
                {
                    CurrentIndex -= 1;
                }
            }
            else if (x > _lastX)
            {
                if (CurrentIndex == ChunksSource.Count - 1)
                {
                    CurrentIndex = 0;
                }
                else
                {
                    CurrentIndex += 1;
                }
            }

            _lastX = x;
        }

        private void CurrentImage_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            _isPressed = false;
            _lastX = 0;
        }

        private void LoadImageSources()
        {
            if (ChunksSource == null || ChunksSource.Count == 0)
            {
                CurrentImage.Source = null;
                _currentIndex = 0;
                _lastX = 0;
                _chunks = new List<ImageSource>();
                return;
            }

            var chunks = new List<ImageSource>();

            foreach (var bytese in ChunksSource)
            {
                chunks.Add((ImageSource)new ImageSourceConverter().ConvertFrom(bytese));
            }

            _chunks = chunks;
            _lastX = _chunks[0].Width / 2;
            CurrentIndex = 0;
        }
    }
}
