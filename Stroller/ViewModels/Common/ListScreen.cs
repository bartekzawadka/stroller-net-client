using System.Collections.Generic;
using Caliburn.Micro;
using Stroller.Main;

namespace Stroller.ViewModels.Common
{
    public class ListScreen<T> : ScreenBase
    {
        private IEnumerable<T> _context;
        private T _selectedContextItem;

        public IEnumerable<T> Context
        {
            get => _context;
            set
            {
                if (Equals(value, _context)) return;
                _context = value;
                NotifyOfPropertyChange();
            }
        }

        public T SelectedContextItem
        {
            get => _selectedContextItem;
            set
            {
                if (Equals(value, _selectedContextItem)) return;
                _selectedContextItem = value;
                NotifyOfPropertyChange();
            }
        }

        public ListScreen(ScreenBase parentScreen) : base(parentScreen)
        {
        }

        public virtual void LoadData()
        {

        }
    }
}
