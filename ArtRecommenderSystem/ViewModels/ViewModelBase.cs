using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;

namespace ArtRecommenderSystem.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public class SnackBarMessageDisplayEventArgs : EventArgs
        {
            public string Message { get; set; }
        }

        public SnackbarMessageQueue MessageQueue { get; set; }

        public event EventHandler<SnackBarMessageDisplayEventArgs>
            SnackBarMessageDisplayRequested;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnSnackBarMessageDisplayRequest(string message)
        {
            SnackBarMessageDisplayRequested?.Invoke(this,
                new SnackBarMessageDisplayEventArgs { Message = message });
        }
        
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
