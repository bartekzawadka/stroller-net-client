using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Stroller.Common.Events;
using Stroller.Contracts.Dto;

namespace Stroller.Views.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy NImagesList.xaml
    /// </summary>
    public partial class NImagesList
    {
        private List<ImageListItem> _selectedImagesCollection = new List<ImageListItem>();

        public static DependencyProperty ItemsPerRowProperty =
            DependencyProperty.Register("ItemsPerRow", typeof(int), typeof(NImagesList));

        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<ImageListItem>), typeof(NImagesList));

        public static DependencyProperty ImageSelectableProperty =
            DependencyProperty.Register("ImageSelectable", typeof(bool), typeof(NImagesList));

        public int ItemsPerRow
        {
            get => (int) GetValue(ItemsPerRowProperty);
            set
            {
                SetValue(ItemsPerRowProperty, value); 
                LoadTiles();
            }
        }

        public IEnumerable<ImageListItem> ItemsSource
        {
            get => (IEnumerable<ImageListItem>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value); 
                LoadTiles();
            }
        }

        public bool ImageSelectable
        {
            get => (bool) GetValue(ImageSelectableProperty);
            set => SetValue(ImageSelectableProperty, value);
        }

        public event RoutedEventHandler DeleteClicked;
        public event RoutedEventHandler DownloadJsonClicked;
        public event RoutedEventHandler DownloadZipClicked;
        public event RoutedEventHandler ItemClicked;
        public event RoutedEventHandler SelectedImagesChanged;

        public NImagesList()
        {
            InitializeComponent();
            Loaded += NImagesList_Loaded;
        }

        private void NImagesList_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTiles();
        }

        private void LoadTiles()
        {
            if (ItemsSource == null)
                return;

            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();

            for (var i = 0; i < ItemsPerRow; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            var rows = new List<List<ImageListItem>>();
            var row = new List<ImageListItem>();
            var index = 0;
            foreach (var imageListItem in ItemsSource)
            {
                if (index < ItemsPerRow)
                {
                    row.Add(imageListItem);
                    index++;
                }
                else
                {
                    index = 0;
                    rows.Add(row);
                    row = new List<ImageListItem>();
                }
            }

            if (row.Count > 0)
                rows.Add(row);

            if (rows.Count == 0)
                return;

            for (var i = 0; i < rows.Count; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
            }

            for (var i = 0; i < rows.Count; i++)
            {
                for (var j = 0; j < rows[i].Count; j++)
                {
                    var image = new NImage
                    {
                        BackgroundImageData = rows[i][j].Thumbnail,
                        Text = rows[i][j].CreatedAtText,
                        DataContext = rows[i][j],
                        IsSelectable = ImageSelectable
                    };
                    image.DeleteClicked += Image_DeleteClicked;
                    image.DownloadJsonClicked += Image_DownloadJsonClicked;
                    image.DownloadZipClicked += Image_DownloadZipClicked;
                    image.ItemClicked += ImageItemClicked;
                    image.ItemSelectionChanged += Image_ItemSelectionChanged;
                    image.SetValue(Grid.RowProperty, i);
                    image.SetValue(Grid.ColumnProperty, j);
                    MainGrid.Children.Add(image);
                }
            }
        }

        private void Image_ItemSelectionChanged(ImageListItem data, bool state)
        {
            if (data == null)
                return;

            if (state)
            {
                if (_selectedImagesCollection.Any(x => string.Equals(x.DirectoryName, data.DirectoryName))) return;

                _selectedImagesCollection.Add(data);
                SelectedImagesChanged?.Invoke(this, new SelectedImagesChangedEventArgs
                {
                    SelectedItems = _selectedImagesCollection
                });
            }
            else
            {
                if (!_selectedImagesCollection.Any(x => string.Equals(x.DirectoryName, data.DirectoryName))) return;

                _selectedImagesCollection = _selectedImagesCollection
                    .Where(x => !string.Equals(x.DirectoryName, data.DirectoryName)).ToList();
                SelectedImagesChanged?.Invoke(this, new SelectedImagesChangedEventArgs
                {
                    SelectedItems = _selectedImagesCollection
                });
            }
        }

        private void ImageItemClicked(ImageListItem data)
        {
            ItemClicked?.Invoke(this, new ImageListItemEventArgs
            {
                ImageListItem = data
            });
        }

        private void Image_DownloadZipClicked(ImageListItem data)
        {
            DownloadZipClicked?.Invoke(this, new ImageListItemEventArgs
            {
                ImageListItem = data
            });
        }

        private void Image_DownloadJsonClicked(ImageListItem data)
        {
            DownloadJsonClicked?.Invoke(this, new ImageListItemEventArgs
            {
                ImageListItem = data
            });
        }

        private void Image_DeleteClicked(ImageListItem data)
        {
            DeleteClicked?.Invoke(this, new ImageListItemEventArgs
            {
                ImageListItem = data
            });
        }
    }
}
