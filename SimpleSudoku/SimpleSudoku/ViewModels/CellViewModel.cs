using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SC.SimpleSudoku.Model;

namespace SC.SimpleSudoku.ViewModels
{
    class CellViewModel : INotifyPropertyChanged
    {
        private bool _is1Visible;

        private bool _is2Visible;

        private bool _is3Visible;

        private bool _is4Visible;

        private bool _is5Visible;

        private bool _is6Visible;

        private bool _is7Visible;

        private bool _is8Visible;

        private bool _is9Visible;
        private byte? _content;

        public int Row { get; }
        public int Column { get; }

        public byte? Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public ICommand CellClickedCommand => new DelegateCommand(obj => OnCellSelected());
        private bool _isSelected;

        private OptionsViewModel Options { get; }

        public CellViewModel(OptionsViewModel options, int row, int column, CellSelectedEventHandler cellSelectedEventHandler, byte? content = null )
        {
            Options = options;
            Column = column;
            Row = row;
            Content = content;
            CellSelected += cellSelectedEventHandler;
        }

        public bool ReadOnly { get; set; }

        public bool Is1Visible
        {
            get { return _is1Visible && Content == null; }
            set
            {
                _is1Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is2Visible
        {
            get { return _is2Visible && Content == null; }
            set
            {
                _is2Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is3Visible
        {
            get { return _is3Visible && Content == null; }
            set
            {
                _is3Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is4Visible
        {
            get { return _is4Visible && Content == null; }
            set
            {
                _is4Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is5Visible
        {
            get { return _is5Visible && Content == null; }
            set
            {
                _is5Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is6Visible
        {
            get { return _is6Visible && Content == null; }
            set
            {
                _is6Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is7Visible
        {
            get { return _is7Visible && Content == null; }
            set
            {
                _is7Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is8Visible
        {
            get { return _is8Visible && Content == null; }
            set
            {
                _is8Visible = value;
                OnPropertyChanged();
            }
        }

        public bool Is9Visible
        {
            get { return _is9Visible && Content == null; }
            set
            {
                _is9Visible = value;
                OnPropertyChanged();
            }
        }


        public byte? Number
        {
            get { return Content; }
            set
            {
                Content = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event CellSelectedEventHandler CellSelected;

        protected virtual void OnCellSelected()
        {
            CellSelected?.Invoke(this, Row, Column);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal delegate void CellSelectedEventHandler(object sender, int row, int column);
}