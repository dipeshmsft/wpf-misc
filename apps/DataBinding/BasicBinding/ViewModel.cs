using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BasicBinding
{
    public class ViewModel : INotifyPropertyChanged
    {
        public string TextBoxContent
        {
            get => _textBoxContent;
            set
            {
                _textBoxContent = value;
                OnPropertyChanged(nameof(TextBoxContent));
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _textBoxContent = "";
    }
}

