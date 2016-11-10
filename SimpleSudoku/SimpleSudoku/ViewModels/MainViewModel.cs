using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SC.SimpleSudoku.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public CellViewModel[][] CurrentSudokuPuzzle { get; set; }
        public NavigationState CurrentNavState { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class NavigationState
        {
            //TODO: Add list of variables holding navigation state

        }

        internal class CellViewModel : INotifyPropertyChanged
        {
            public byte Content { get; set; }
            public bool ReadOnly { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
