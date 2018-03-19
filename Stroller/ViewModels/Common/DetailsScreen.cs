namespace Stroller.ViewModels.Common
{
    public class DetailsScreen<T>:ScreenBase
    {
        private T _context;

        public T Context
        {
            get => _context;
            set
            {
                if (Equals(value, _context)) return;
                _context = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
