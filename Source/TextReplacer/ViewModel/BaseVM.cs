using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextReplacer.ViewModel
{
    public class BaseVM : INotifyPropertyChanged
    {
        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Methods

        protected static void OnPropertyChangedAction(PropertyChangedEventArgs a, string expectedName, Action action)
        {
            if (a.PropertyName == expectedName)
                action?.Invoke();
        }

        // Create the OnPropertyChanged method to raise the event
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}