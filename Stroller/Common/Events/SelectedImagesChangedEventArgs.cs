using System.Collections.Generic;
using System.Windows;
using Stroller.Contracts.Dto;

namespace Stroller.Common.Events
{
    public class SelectedImagesChangedEventArgs : RoutedEventArgs
    {
        public IEnumerable<ImageListItem> SelectedItems { get; set; }
    }
}
