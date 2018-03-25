using System.Windows;
using Stroller.Contracts.Dto;

namespace Stroller.Common.Events
{
    public class ImageListItemEventArgs : RoutedEventArgs
    {
        public ImageListItem ImageListItem { get; set; }
    }
}
