using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
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

        private int Row { get; }
        private int Column { get; }

        private SudokuDataContext Database { get; set; }


        /// <summary>
        /// The number that the cell has been holding, stored as a Nullable byte (byte?).
        /// </summary>
        public byte? Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the cell is selected or not. Only one cell in the grid should be selected at a time.
        /// </summary>
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

        // ReSharper disable once UnusedMember.Global
        public ICommand CellClickedCommand => new DelegateCommand(obj => OnCellSelected());
        private bool _isSelected;
        private bool _isReadOnly;
        private bool _isWrong;

        /// <summary>
        /// A copy of the options that the user has set from the options page. This is so the cell knows how it should display itself, based on these options.
        /// </summary>
        private OptionsViewModel Options { get; }
        
        /// <summary>
        /// The username of the currently signed in user, for looking up the user in the database.
        /// </summary>
        private string Username { get; }

        public CellViewModel(OptionsViewModel options, int row, int column, CellSelectedEventHandler cellSelectedEventHandler, byte? content, bool isReadOnly, DbSet<Mistake> mistakes, bool isWrong, string username, SudokuDataContext database)
        {
            Options = options;
            Column = column;
            Row = row;
            IsReadOnly = isReadOnly;
            Content = content;
            CellSelected += cellSelectedEventHandler;
            Username = username;
            Mistakes = mistakes;
            Database = database;
            IsWrong = isWrong;
        }

        private DbSet<Mistake> Mistakes { get; }

        public bool IsVisiblyWrong => IsWrong && Options.IsMistakeHighlightingOn;

        public bool IsWrong
        {
            get { return _isWrong; }
            set
            {
                _isWrong = value;
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(IsVisiblyWrong));
                if (value == false) return;
                //Add a mistake to the mistakes list.
                if (Mistakes.Any(x => x.Row == Row && x.Column == Column && x.Username == Username))
                    return;
                Mistakes.Add(new Mistake {Row = Row, Column = Column, Username = Username});
                Database.SaveChanges();
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                if (value == _isReadOnly)
                    return;
                _isReadOnly = value; 
                OnPropertyChanged();
            }
        }

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