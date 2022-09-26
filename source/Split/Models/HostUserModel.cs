using Split.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Split.Models
{
    public class HostUserModel : User, INotifyPropertyChanged
    {
        public HostUserModel(User user) : base(user)
        {

        }

        public HostUserModel(string name, UserEntityType entityType = UserEntityType.Person)
            : base(name, entityType)
        {

        }

        private bool _isHost;
        public bool IsHost
        {
            get => _isHost;
            set => SetField(ref _isHost, value, nameof(IsHost));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


    }
}
