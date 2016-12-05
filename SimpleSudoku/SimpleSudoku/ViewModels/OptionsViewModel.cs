using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SC.SimpleSudoku.Model;

namespace SC.SimpleSudoku.ViewModels
{
    internal class OptionsViewModel : INotifyPropertyChanged
    {
        private readonly User _currentUser;

        public OptionsViewModel(User currentUser)
        {
            _currentUser = currentUser;
        }

        public bool IsMistakeHighlightingOn
        {
            get { return _currentUser.IsMistakeHighlightingOn; }
            set
            {
                if (_currentUser.IsMistakeHighlightingOn == value)
                    return;
                _currentUser.IsMistakeHighlightingOn = value; 
                OnPropertyChanged();
            }
        }

        public bool IsLeaderboardVisible
        {
            get { return _currentUser.IsLeaderboardVisible; }
            set
            {
                if (_currentUser.IsLeaderboardVisible == value)
                    return;
                _currentUser.IsLeaderboardVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsPuzzleTimerVisible
        {
            get { return _currentUser.IsPuzzleTimerVisible; }
            set
            {
                if (_currentUser.IsPuzzleTimerVisible == value)
                    return;
                _currentUser.IsPuzzleTimerVisible = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}