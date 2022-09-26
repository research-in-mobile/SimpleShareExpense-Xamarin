using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Models
{
    public class EntryModel : BindableBase
    {
        public EntryModel(
            Func<string, bool> validationLogic = null,
            string errorMessage = "")
        {
            ValidationLogic = validationLogic;
            ErrorMessage = errorMessage;
        }

        private string _entry;
        public string Entry
        {
            get => _entry;
            set
            {
                SetProperty(ref _entry, value);
                RaisePropertyChanged(nameof(ErrorMessage));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => CanShowErrorMessage ? _errorMessage : string.Empty;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _showErrorMessage;
        public bool ShowErrorMessage
        {
            get => _showErrorMessage;
            set
            {
                SetProperty(ref _showErrorMessage, value);
                RaisePropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsValidEntry => ValidationLogic != null ? ValidationLogic(_entry) : true;
        public bool CanShowErrorMessage => ShowErrorMessage && !IsValidEntry ? true : false;

        public Func<string, bool> ValidationLogic { get; set; }
    }
}
